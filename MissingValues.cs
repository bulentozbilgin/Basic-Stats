/*********************************************************************
 *  
 * Copyright 2010 B. Bulent Ozbilgin
 * This program is distributed under the terms of the GNU Lesser General Public License (Lesser GPL)
 *********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace parStats.BasicStats
{
    public class MissingValues : BaseC
    {
        public MissingValues():base()
        {
            //add constructor logic here
        }

        public void EliminatedList(ref List<double> MyList, List<double> ValuesToEliminate) 
        {
            List<double> NewList;
            Boolean bExist;

            NewList = new List<double>();
            try
            {
                for (int i = 0; i < MyList.Count; i++)
                {
                    bExist = false;
                    double d = MyList[i];
                    foreach (double Missing in ValuesToEliminate)
                    {
                        if (d == Missing)
                        {
                            bExist = true;
                        }
                    }
                    if (bExist == true)
                    {
                        MyList.Remove(d);
                        i--;
                    }
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot eliminate missing values", e);
            }
        }

        public void PairwiseEliminatedLists(ref List<double> FirstList, ref List<double> SecondList, List<double> FirstListMissingValues, List<double> SecondListMissingValues)
        {
            //this function takes two arrays that will be used for t-test, correlation, or x-tab analysis
            //and takes a list of missing values; then eliminates the pairs which include missing values
            //for each of the arrays, and then modifies the given arrays
            int i;
            double Value;
            bool bExist;

            //check to make sure that the collection lengths are the same
            if (FirstList.Count != SecondList.Count)
            {
                System.ArgumentException argEx = new System.ArgumentException("In pairwise missing values elimination, arrays are not equal length", "FirstList and SecondList");
                throw argEx;
            }
            
            //we will do two passes from the lists; since the list counts are equal, if the missing value is found in the list, both that list's value and
            //the other list's corresponding value will be deleted
            for (int j = 0; j < FirstList.Count; j++)
            {
                bExist = false;
                double d = FirstList[j];
                foreach (double Missing in FirstListMissingValues)
                {
                    if (d == Missing)
                    {
                        bExist = true;
                    }
                }
                if (bExist == true)
                {
                    FirstList.RemoveAt(j);
                    SecondList.RemoveAt(j);
                    j--;
                }
            }


            //then, do the same for the second list
            for (int j = 0; j < SecondList.Count; j++)
            {
                bExist = false;
                double d = SecondList[j];
                foreach (double Missing in SecondListMissingValues)
                {
                    if (d == Missing)
                    {
                        bExist = true;
                    }
                }
                if (bExist == true)
                {
                    FirstList.RemoveAt(j);
                    SecondList.RemoveAt(j);
                    j--;
                }
            }

            //i = 0;
            //while (FirstList.Count > i)
            //{
            //    Value = FirstList[i];
            //    bExist = false;
            //    foreach (double Missing in FirstListMissingValues)
            //    {
            //        if (Missing == Value)
            //        {
            //            FirstList.RemoveAt(i);
            //            SecondList.RemoveAt(i);
            //            bExist = true;
            //            break;
            //        }
            //    }
            //    i += 1;
            //}
            //the second pass
            //i = 0;
            //while (SecondList.Count > i)
            //{
            //    Value = SecondList[i];
            //    foreach (double Missing in SecondListMissingValues)
            //    {
            //        if (Missing == Value)
            //        {
            //            FirstList.RemoveAt(i);
            //            SecondList.RemoveAt(i);
            //            break;
            //        }
            //    }
            //    i += 1;
            //}
        }
    }
}
