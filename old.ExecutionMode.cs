using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace UnderMyHat.ToolChest
{
    /// <summary>
    /// Contains methods, properties and fields that help determine what the current
    /// execution state is inside a method (i.e., Design vs User mode), or what the mode 
    /// is in which an existing method or assembly has been compiled in (i.e., Debug vs
    /// Release build).
    /// </summary>
    public class ExecutionMode
    {
        /// <summary>
        /// Contains the assembly belonging to the current type (i.e., ExecutionMode)
        /// </summary>
        private static readonly AssemblyName currentTypeAssembly = Assembly.GetAssembly(MethodBase.GetCurrentMethod().DeclaringType).GetName();

        /// <summary>
        /// Returns the EntryAssembly that started this assembly. Normally, this 
        /// returns the assembly of the running executable that this assembly 
        /// is referenced in, no matter how deep. In design contexts, this should be
        /// the host assembly, which has this assembly loaded, but not referenced.
        /// </summary>
        /// <remarks>
        /// Can be null for Visual Studio (though this is technically incorrect and may
        /// change in the future, for the workings of this approach, it doesn't matter)
        /// </remarks>
        private static readonly Assembly entryAssembly = Assembly.GetEntryAssembly();


        /// <summary>
        /// Execution or usage mode of the running code is User Mode: it is a run by a 
        /// normal user, not a developer inside an IDE. In other words: the compiled 
        /// executable is run.
        /// </summary>
        public static bool IsUserMode { get; private set; }

        /// <summary>
        /// Execution or usage mode of the running code is Design Mode: it is run inside
        /// a container inside an IDE or a test container while being designed by, for instance
        /// a developer.
        /// </summary>
        public static bool IsDesignMode { get; private set; }

        /// <summary>
        /// True when the current execution mode is in debug mode (DEBUG defined), false otherwise
        /// </summary>
        public static readonly bool IsDebugMode =
#if DEBUG
 true;
#else
                        false;
#endif

        /// <summary>
        /// True when the current execution mode is in release mode (DEBUG not defined), false otherwise;
        /// </summary>
        public static readonly bool IsReleaseMode = !IsDebugMode;

        /// <summary>
        /// Determines the state of the execution mode: whether in design mode or user mode. The
        /// result is stored and can be used subsequently. 
        /// TODO: lazy initialization & thread safety (singleton)
        /// </summary>
        static ExecutionMode()
        {
            //// try to find a matching assembly in the referenced assembly chain
            string typeAssemblyName = currentTypeAssembly.FullName;
            var result = from asmName in entryAssembly.GetRecursiveReferencedAssemblyNames()
                         where asmName.FullName == typeAssemblyName
                         select asmName;

            // if the assembly for the type ExecutionMode is not found
            // the "parent assembly" is the designer and has the assembly loaded
            // but not referenced. Otherwise, if found, we are in user mode.
            IsDesignMode = result.Count() == 0;
            IsUserMode = !IsDesignMode;
        }




    }

    /// <summary>
    /// Contains extension methods for the class Assembly.
    /// TODO: move to Util class?
    /// </summary>
    internal static class AssemblyExtensions
    {
        /// <summary>
        /// Extension method gets all referenced assemblies from a starting assembly
        /// Will keep a list of referenced assemblies to prevent neverending recursive loop
        /// </summary>
        /// <param name="assembly">
        /// The Assembly that must be searched for all the assemblies it references
        /// </param>
        /// <returns>
        /// Dictonary with a flattened list of all referenced assemblies no matter how deep
        /// </returns>
        /// <remarks>
        /// Using extension methods removes the burden of null-checking. The starting
        /// assembly can be null, in which case the extension method just return an 
        /// empty list.
        /// </remarks>
        internal static List<AssemblyName> GetRecursiveReferencedAssemblyNames(this Assembly assembly)
        {
            Dictionary<string, AssemblyName> assemblyList = new Dictionary<string, AssemblyName>();
            return assembly.GetRecursiveReferencedAssemblyNames(assemblyList).Values.ToList();
        }

        /// <summary>
        /// Helper method for the GetRecuriveReferencedAssemblyNames(this Assembly) public method, contains
        /// the core functionality of that method and is called recursively until all referenced assemblies
        /// are retrieved.
        /// </summary>
        /// <param name="assembly">
        /// The Assembly being searched through for assemblies it references
        /// </param>
        /// <param name="assemblyList">
        /// Dictionary containing all the fullname-AssemblyName pairs of the referenced assemblies, 
        /// used to prevent neverending recursion
        /// </param>
        /// <returns>
        /// Dictonary with a flattened list of all referenced assemblies no matter how deep
        /// </returns>
        /// <remarks>
        /// This method should be safe to call without it raising exceptions. Unfortunately, due to a 
        /// bug in Mono, trying to retrieve some of the referenced assemblies raised an Not Found exception.
        /// As a result, the Assembly.Load method is wrapped in a try/catch.
        /// </remarks>
        private static Dictionary<string, AssemblyName> GetRecursiveReferencedAssemblyNames(this Assembly assembly, Dictionary<string, AssemblyName> assemblyList)
        {
            if (assembly == null)
                return assemblyList;

            foreach (AssemblyName assemblyName in assembly.GetReferencedAssemblies())
            {
                if (!assemblyList.ContainsKey(assemblyName.FullName))
                {
                    assemblyList.Add(assemblyName.FullName, assemblyName);

                    // load inside a try/catch because it can raise an exception File Not Found (see below)
                    try
                    {
                        Assembly.Load(assemblyName).GetRecursiveReferencedAssemblyNames(assemblyList);
                    }
                    catch (Exception)
                    {
                        // both WindowsBase, version 3.0.0.0 and vjslib, version 2.0.0.0 (perhaps others, too)
                        // throw an exception on Mono: 
                        // "Could not load file or assembly 'WindowsBase' or one of its dependencies. The system cannot find the file specified.
                        // Log.Write("Error while loading {0}: {1}", assemblyName, e.Message);
                    }
                }
            }
            return assemblyList;
        }

    }
}
