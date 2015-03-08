
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

     public class Descriptives : BaseC
    {
        public Descriptives():base()
        {
            //add constructor logic here
        }

        /// <summary>
        /// Calculates the average (mean) value of a given list of double-precision values.
        /// </summary>
        public double Mean(List<double> MyList)
        {
            //take a list, filled with objects as an argument.  check that all values are numeric in the list
            //then calculate the mean
            try
            {
                int iCount = MyList.Count;
                double value = SumOfValues(MyList);
                if (iCount == 0)
                {
                    return 0;
                }
                else
                {
                    return (double)(value / iCount);
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute mean value using doubles", e);
            }
        }

        public double Mean(List<Int64> MyList)
        {
            //take a list, filled with objects as an argument.  check that all values are numeric in the list
            //then calculate the mean
            try
            {
                int iCount = MyList.Count;
                Int64 value = SumOfValues(MyList);
                if (iCount == 0)
                {
                    return 0;
                }
                else
                {
                    return (double)(value / iCount);
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute mean value using doubles", e);
            }
        }

        public decimal Mean(List<decimal> MyList)
        {
            //take a list, filled with objects as an argument.  check that all values are numeric in the list
            //then calculate the mean
            try
            {
                int iCount = MyList.Count;
                decimal value = SumOfValues(MyList);
                if (iCount == 0)
                {
                    return 0;
                }
                else
                {
                    return (decimal)(value / iCount);
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute mean value using doubles", e);
            }
        }
         //---------------------------------------------------------------------------------------------------------------
        public double SumOfValues(List<double> MyList)
        {
            //returns the sum of all the values in the given list
            try
            {
                double value = 0.00;
                foreach (double d in MyList)
                {
                    value += d;
                }
                return value;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute the double sum of values value", e);
            }
        }

        public Int64 SumOfValues(List<Int64> MyList)
        {
            //returns the sum of all the values in the given list
            try
            {
                Int64 value = 0;
                foreach (Int64 d in MyList)
                {
                    value += d;
                }
                return value;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute the integer sum of values value", e);
            }
        }

        public decimal SumOfValues(List<decimal> MyList)
        {
            //returns the sum of all the values in the given list
            try
            {
                decimal value = 0;
                foreach (decimal d in MyList)
                {
                    value += d;
                }
                return value;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute the decimal sum of values value", e);
            }
        }

        //---------------------------------------------------------------------------------------------------------------

        public double SampleVariance(List<double> MyList)
        {
            //calculate the sample variance = sigma[(value-mean)^2] / n-1

            double dSumSq, dMean, dValue, dReturn;
            int i, iLen;

            try
            {
                dSumSq = 0;
                dMean = Mean(MyList);
                for (i = 0; i < MyList.Count; i++)
                {
                    dValue = Convert.ToDouble(MyList[i]);
                    dValue = dValue - dMean;
                    dSumSq += Common.Square(dValue);
                }
                //calculate n
                iLen = MyList.Count;
                dReturn = dSumSq / (iLen - 1);
                return dReturn;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot Compute Sample Variance", e);
            }
        }

        public double SampleVariance(List<Int64> MyList)
        {
            //calculate the sample variance = sigma[(value-mean)^2] / n-1

            double dSumSq, dMean, dReturn, dValue;
            Int64 iValue;
            int i, iLen;

            try
            {
                dSumSq = 0;
                dMean = Mean(MyList);
                for (i = 0; i < MyList.Count; i++)
                {
                    iValue = Convert.ToInt64(MyList[i]);
                    dValue = (double)iValue - (double)dMean;
                    dSumSq += Common.Square(dValue);
                }
                //calculate n
                iLen = MyList.Count;
                dReturn = dSumSq / (iLen - 1);
                return dReturn;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot Compute Sample Variance", e);
            }
        }

        public decimal SampleVariance(List<decimal> MyList)
        {
            //calculate the sample variance = sigma[(value-mean)^2] / n-1

            decimal dSumSq, dMean, dValue, dReturn;
            int i, iLen;

            try
            {
                dSumSq = 0;
                dMean = Mean(MyList);
                for (i = 0; i < MyList.Count; i++)
                {
                    dValue = Convert.ToDecimal(MyList[i]);
                    dValue = dValue - dMean;
                    dSumSq += Convert.ToDecimal(Common.Square(Convert.ToDouble(dValue)));
                }
                //calculate n
                iLen = MyList.Count;
                dReturn = dSumSq / (iLen - 1);
                return dReturn;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot Compute Sample Variance", e);
            }
        }

        //---------------------------------------------------------------------------------------------------------------
        public double SampleStandardDeviation(List<double> MyList)
        {
            //calculate the Standard Deviation of a given list of numbers
            //SD = Square Root of Variance

            try
            {
                return Common.Sqrt(SampleVariance(MyList));
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot Compute Standard Deviation", e);
            }
        }

        public decimal SampleStandardDeviation(List<decimal> MyList)
        {
            //calculate the Standard Deviation of a given list of numbers
            //SD = Square Root of Variance

            decimal dReturn;
            decimal variance;
            double dValue;
            try
            {
                variance = SampleVariance(MyList);
                dValue = Common.Sqrt(Convert.ToDouble(variance));
                dReturn = Convert.ToDecimal(dValue);
                return dReturn;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot Compute Standard Deviation", e);
            }
        }

        //---------------------------------------------------------------------------------------------------------------
        public double StandardError(List<double> MyList)
        {
            //calculate the standard error for a normal distribution assumption
            //SE = SD / Sqrt(n)
            double dStandardDeviation;
            int n;

            try
            {
                dStandardDeviation = SampleStandardDeviation(MyList);
                n = MyList.Count;
                return dStandardDeviation / Common.Sqrt(n);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot Compute Standard Error", e);
            }
        }

        public decimal StandardError(List<decimal> MyList)
        {
            //calculate the standard error for a normal distribution assumption
            //SE = SD / Sqrt(n)
            decimal dStandardDeviation;
            int n;

            try
            {
                dStandardDeviation = SampleStandardDeviation(MyList);
                n = MyList.Count;
                return dStandardDeviation / Convert.ToDecimal(Common.Sqrt(n));
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot Compute Standard Error", e);
            }
        }
         //---------------------------------------------------------------------------------------------------------------
        public double SampleSkewness(List<double> MyList)
        {
            //Skewness is a measure of symmetry, or more precisely, the lack of symmetry.
            //A distribution, or data set, is symmetric if it looks the same to the left
            //and right of the center point.

            //there are multiple ways of computing skewness; we use the formula that is used by Excel and SPSS
            //skewness = [n / (n-1)(n-2)] * sigma[((dValue-dMean)/SD)^3]

            double dValue, dMean, dStandardDeviation, dReturn;
            double dFirstPortion, dSecondPortion;
            int iLen, i;

            try
            {
                iLen = MyList.Count;
                dMean = Mean(MyList);
                dStandardDeviation = SampleStandardDeviation(MyList);

                dFirstPortion = (double)iLen / ((double)(iLen - 1) * (double)(iLen - 2));
                dSecondPortion = 0;
                for (i = 0; i < MyList.Count; i++)
                {
                    dValue = Convert.ToDouble(MyList[i]);
                    dValue = dValue - dMean;
                    dValue = dValue / dStandardDeviation;
                    dSecondPortion += Common.Cube(dValue);
                }
                dReturn = dFirstPortion * dSecondPortion;
                return dReturn;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot Compute Sample Skewness", e);
            }
        }

        //---------------------------------------------------------------------------------------------------------------
        public double SEofSkewness(List<double> MyList)
        {
            //this function calculates the standard error of skewness
            //SES = Sqrt(6 / n)

            int iLen;
            double dValue;
            try
            {
                iLen = MyList.Count;
                dValue = 6.00;
                dValue = dValue / (double)iLen;
                return Common.Sqrt(dValue);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot Compute Standard Error of Skewness", e);
            }
        }

        //---------------------------------------------------------------------------------------------------------------
        public double SumOfSquareDeviates(List<double> MyList)
        {
            //calculate the sum of all the square deviates in the dataset
            //formula is: s[(x(i) - x)^2]

            double dMean, dTotal, dValue;
            int i;
            try
            {
                dMean = Mean(MyList);
                dTotal = 0;
                for (i = 0; i < MyList.Count; i++)
                {
                    dValue = Convert.ToDouble(MyList[i]);
                    dTotal += Common.Square(dValue - dMean);
                }
                return dTotal;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot Compute Sum of Square Deviates", e);
            }
        }

        public decimal SumOfSquareDeviates(List<decimal> MyList)
        {
            //calculate the sum of all the square deviates in the dataset
            //formula is: s[(x(i) - x)^2]

            decimal dMean, dTotal, dValue, dSquare;
            int i;
            try
            {
                dMean = Mean(MyList);
                dTotal = 0;
                for (i = 0; i < MyList.Count; i++)
                {
                    dValue = MyList[i];
                    dSquare = Convert.ToDecimal(Common.Square(Convert.ToDouble(dValue - dMean)));
                    dTotal += dSquare;
                }
                return dTotal;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot Compute Sum of Square Deviates", e);
            }
        }
        //---------------------------------------------------------------------------------------------------------------
        public double Median(List<double> MyList)
        {
            int iCount,k;
            List<double> LocalMyList;

            //first, sort the list if not already sorted
            //need to copy parameters to local lists before sorting them; otherwise performing operations on the originating variable changes it since it's passed by reference
            LocalMyList = new List<double>();
            foreach (double item in MyList)
            {
                LocalMyList.Add(item);
            }
            LocalMyList.Sort();

            //calculate the median
            iCount = LocalMyList.Count;
            if (iCount == 1)
            {
                return Convert.ToDouble(LocalMyList[0]);
            }
            else if (iCount % 2 == 0)
            {
                //even value
                k = iCount / 2;
                return (Convert.ToDouble(LocalMyList[k-1]) + Convert.ToDouble(LocalMyList[k]))/2;
            }
            else
            {
                //odd value
                //the following returns the integer part of the division
                k = iCount / 2;
                return Convert.ToDouble(LocalMyList[k]);
            }
        }

        public double Mode(List<double> MyList)
        {
            System.Data.DataTable dtFreq;
            Frequency f;
            double dHighValue;
            int iCount, iHighCount;

            //"mode" is the most common recurring value in the array, in other words, the value with the biggest frequency

            try
            {
                if (MyList.Count == 1)
                {
                    return Convert.ToDouble(MyList[0]);
                }
                else
                {
                    f = new Frequency();
                    dtFreq = f.FrequencyList(MyList);
                    iCount = 0;
                    iHighCount = 0;
                    dHighValue = 0;
                    foreach (System.Data.DataRow r in dtFreq.Rows)
                    {
                        iCount = Convert.ToInt32(r["Count"]);
                        if (iCount > iHighCount) 
                        { 
                            dHighValue = Convert.ToDouble(r["ItemValue"]);
                            iHighCount = iCount;
                        }
                    }
                    return dHighValue;
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute mode value", e);
            }
        }

        public double Minimum(List<double> MyList)
        {

            try
            {
                int i;
                double dMin, dValue;

                dMin = Convert.ToDouble(MyList[0]);
                for (i = 0; i < MyList.Count; i++)
                {
                    dValue = Convert.ToDouble(MyList[i]);
                    dMin = Convert.ToDouble(dMin);
                    if (dValue < dMin)
                    {
                        dMin = dValue;
                    }
                }

                return dMin;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot find the minimum value in list", e);
            }

        }

        public double Maximum(List<double> MyList)
        {
            try
            {
                int i;
                double dMax, dValue;

                dMax = Convert.ToDouble(MyList[0]);
                for (i = 0; i < MyList.Count; i++)
                {
                    dValue = Convert.ToDouble(MyList[i]);
                    if (dValue > dMax)
                    {
                        dMax = dValue;
                    }
                }

                return dMax;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot find the maximum value in list", e);
            }
        }

        public double Range(List<double> MyList)
         {
             try
             {
                 return Maximum(MyList) - Minimum(MyList);
             }
             catch (Exception e)
             {
                 throw new ApplicationException("Cannot find the range of the list", e);
             }
         }
        //---------------------------------------------------------------------------------------------------------------
        public double PearsonFirstSkewness(List<double> MyList)
         {
             //Skewness is a measure of symmetry, or more precisely, the lack of symmetry.
             //A distribution, or data set, is symmetric if it looks the same to the left
             //and right of the center point.

             //computed by: 3* (mean-mode) / SD
             double dMean, dStandardDeviation, dMode;
             double dReturn;
             try
             {
                 dMean = Mean(MyList);
                 dStandardDeviation = SampleStandardDeviation(MyList);
                 dMode = Mode(MyList);

                 dReturn = (double)3 * (dMean - dMode);
                 dReturn = dReturn / dStandardDeviation;
                 return dReturn;
             }
             catch (Exception e)
             {
                 throw new ApplicationException("Cannot Compute Pearson First Skewness", e);
             }
         }

        public double PearsonSecondSkewness(List<double> MyList)
        {
            //Skewness is a measure of symmetry, or more precisely, the lack of symmetry.
            //A distribution, or data set, is symmetric if it looks the same to the left
            //and right of the center point.

            //computed by multiplying the difference between the mean and median times 3, and dividing the result by the standard deviation
            double dMean, dMedian, dStandardDeviation;
            double dReturn;
            try
            {
                dMean = Mean(MyList);
                dStandardDeviation = SampleStandardDeviation(MyList);
                dMedian = Median(MyList);

                dReturn = (double)3 * (dMean - dMedian);
                dReturn = dReturn / dStandardDeviation;
                return dReturn;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot Compute Pearson Second Skewness", e);
            }
        }

        private double ExcessKurtosis(List<double> MyList)
         {
             //calculate the Kurtosis value

             //Kurtosis = (sigma((dValue - dMean) ^ 4) / (N-1) * SD^4) - 3

             //3 is the kurtosis value of a standard normal distribution; to figure
             //out the deviation ie excess kurtosis, we subtract 3 from the calculated value

             double dValue, dMean, dStandardDeviation, dTotal;
             int iLen, i;

             try
             {
                 iLen = MyList.Count;
                 dMean = Mean(MyList);
                 dStandardDeviation = SampleStandardDeviation(MyList);

                 dTotal = 0.0;
                 for (i = 0; i < iLen; i++)
                 {
                     dValue = Convert.ToDouble(MyList[i]);
                     dTotal += Common.FourthPower(dValue - dMean);
                 }

                 return (dTotal / ((iLen - 1) * Common.FourthPower(dStandardDeviation))) - 3;
             }
             catch (Exception e)
             {
                 throw new ApplicationException("Cannot Compute Excess Kurtosis", e);
             }
         }

        public double Kurtosis(List<double> MyList)
        {
            //this function calculates Kurtosis value, which characterizes the relative peakedness or flatness of a distribution compared 
            //with the normal distribution. Positive kurtosis indicates a relatively peaked distribution. Negative kurtosis indicates a relatively flat distribution.
            //this function uses the same formula as the function used in Excel

            //Kurtosis = [n*(n+1) / (n-1)(n-2)(n-3)] * sigma[((dValue-dMean)/SD)^4] - [3*(n-1)^2 / (n-2)(n-3)
            double dValue, dMean, dStandardDeviation, dReturn;
            double dFirstPortion, dSecondPortion, dThirdPortion;
            int iLen, i;

            try
            {
                iLen = MyList.Count;
                dMean = Mean(MyList);
                dStandardDeviation = SampleStandardDeviation(MyList);

                dFirstPortion = ((double)iLen * (double)(iLen+1)) / ((double)(iLen - 1) * (double)(iLen - 2) * (double)(iLen-3));
                dSecondPortion = 0;
                for (i = 0; i < MyList.Count; i++)
                {
                    dValue = Convert.ToDouble(MyList[i]);
                    dValue = dValue - dMean;
                    dValue = dValue / dStandardDeviation;
                    dSecondPortion += Common.FourthPower(dValue);
                }
                dThirdPortion = (3 * Common.Square((double)(iLen - 1))) / ((double)(iLen - 2) * (double)(iLen - 3));
                dReturn = (dFirstPortion * dSecondPortion) - dThirdPortion;
                return dReturn;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot Compute Kurtosis", e);
            }

        }

        public double SEofKurtosis(List<double> MyList)
         {
             //calculate the SE of kurtosis distribution
             //SEK = Sqrt(24/n)

             int iLen;
             double dValue;

             try
             {
                 iLen = MyList.Count;
                 dValue = 24;
                 dValue = dValue / (double)iLen;
                 return Common.Sqrt(dValue);
             }
             catch (Exception e)
             {
                 throw new ApplicationException("Cannot Compute Standard Error of Kurtosis distribution", e);
             }
         }

         public double Factorial(int Value)
         {
             //calculate the factorial of a given integer value
             if (Value < 0)
             {
                 System.ArgumentException argEx = new System.ArgumentException("Factorial of negative value not available!", "Value");
                 throw argEx;
             }
             else if (Value == 0)
             {
                 return 1;
             }
             else
             {
                 int i;
                 double dTotal;
                 dTotal = 1;
                 for (i = 0; i <= Common.Abs(Value - 1); i++)
                 {
                     dTotal = dTotal * (Value - i);
                 }
                 return dTotal;
             }
         }

    }
}
