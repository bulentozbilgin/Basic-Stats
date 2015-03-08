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
    public enum SignificanceLevel
    {
        Percent75, Percent80, Percent85, Percent90, Percent95,
        Percent97_5, Percent98, Percent99, Percent99_5, Percent99_75, Percent99_9
    }

    public class Common
    {

        internal static Boolean CheckArray(ArrayType TypeOfArray, List<double> MyList)
        {
            //check the array to make sure that it is not empty, and is the type given
            if (MyList.Count == 0)
            {
                return false;
            }
            foreach (object d in MyList)
            {
                switch (TypeOfArray)
                {
                    case ArrayType.Numeric:
                        if ((d is Int16 || d is Int32 || d is Int64 || d is Double || d is Decimal || d is Single) == false)
                            return false;
                        break;
                    case ArrayType.DateTime:
                        if ((d is DateTime) == false) return false;
                        break;
                    case ArrayType.String:
                        //any value could be there, as long as it is not null
                        break;
                }
                //check that it is a double value
            }
            return true;
        }

        internal static double Abs(double Value)
        {
            //absolute value of a given value
            return System.Math.Abs(Value);
        }

        internal static double Square(double Value)
        {
            //square of a given value
            return Value * Value;
        }

        internal static double Sqrt(double Value)
        {
            //square root of a given value
            return System.Math.Sqrt(Value);
        }

        internal static double Cube(double Value)
        {
            //cube of a given value
            return Value * Value * Value;
        }

        internal static double FourthPower(double Value)
        {
            //fourth power of a given value
            return Value * Value * Value * Value;
        }

        public static string Significance(SignificanceLevel Significance)
        {
            string sSignificance;

            sSignificance = "0.05";
            switch (Significance)
            {
                case SignificanceLevel.Percent75:
                    sSignificance = "0.25";
                    break;
                case SignificanceLevel.Percent80:
                    sSignificance = "0.2";
                    break;
                case SignificanceLevel.Percent85:
                    sSignificance = "0.15";
                    break;
                case SignificanceLevel.Percent90:
                    sSignificance = "0.1";
                    break;
                case SignificanceLevel.Percent95:
                    sSignificance = "0.05";
                    break;
                case SignificanceLevel.Percent97_5:
                    sSignificance = "0.025";
                    break;
                case SignificanceLevel.Percent98:
                    sSignificance = "0.02";
                    break;
                case SignificanceLevel.Percent99:
                    sSignificance = "0.01";
                    break;
                case SignificanceLevel.Percent99_5:
                    sSignificance = "0.005";
                    break;
                case SignificanceLevel.Percent99_75:
                    sSignificance = "0.0025";
                    break;
                case SignificanceLevel.Percent99_9:
                    sSignificance = "0.001";
                    break;
                default:
                    sSignificance = "0.05";
                    break;
            }

            return sSignificance;

        }
       
    }
}
