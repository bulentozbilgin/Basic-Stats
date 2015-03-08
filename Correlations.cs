/*********************************************************************
 *  
 * Copyright 2010 B. Bulent Ozbilgin
 * This program is distributed under the terms of the GNU Lesser General Public License (Lesser GPL)
 *********************************************************************
 */

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;



namespace parStats.BasicStats
{
    public class Correlations : BaseC
    {
        public Correlations():base()
        {
            //add constructor logic here
        }

        public double PearsonCorrelationCoefficient(List<double> FirstList, List<double> SecondList)
        {
            //calculate the correlation coefficient
            //correlation formula nominator = n*sigma(x*y) - [sigma(x)*sigma(y)] 
            //denominator = sqrt{[n*sigma(sqr(x)) - sqr(sigma(x))] * [n*sigma(sqr(y)) - sqr(sigma(y))]}

            //check to make sure that the collection lengths are the same
            if (FirstList.Count != SecondList.Count)
            {
                System.ArgumentException argEx = new System.ArgumentException("In correlation coefficient calculation, arrays are not equal length", "FirstList and SecondList");
                throw argEx;
            }

            double dSum1;
            double dSum2;
            double dSum1And2;
            int n, i;
            double dNominator, dDenominator, dSumOfSquares1, dSumOfSquares2, dValue1, dValue2;
            Descriptives desc;
            try
            {
                n = FirstList.Count;
                desc = new Descriptives();
                dSum1 = desc.SumOfValues(FirstList);
                dSum2 = desc.SumOfValues(SecondList);
                dSum1And2 = 0;
                for (i = 0; i < n; i++)
                {
                    dSum1And2 += Convert.ToDouble(FirstList[i]) * Convert.ToDouble(SecondList[i]);
                }
                dNominator = (n * dSum1And2) - (dSum1 * dSum2);

                dSumOfSquares1 = 0;
                for (i = 0; i < n; i++)
                {
                    dSumOfSquares1 += Convert.ToDouble(FirstList[i]) * Convert.ToDouble(FirstList[i]);
                }

                dSumOfSquares2 = 0;
                for (i = 0; i < n; i++)
                {
                    dSumOfSquares2 += Convert.ToDouble(SecondList[i]) * Convert.ToDouble(SecondList[i]);
                }

                dValue1 = n * dSumOfSquares1 - (dSum1 * dSum1);
                dValue2 = n * dSumOfSquares2 - (dSum2 * dSum2);
                dDenominator = Common.Sqrt(dValue1 * dValue2);

                return dNominator / dDenominator;

            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute Pearson correlation coefficient value", e);
            }
        }

        public decimal PearsonCorrelationCoefficient(List<decimal> FirstList, List<decimal> SecondList)
        {
            //calculate the correlation coefficient
            //correlation formula nominator = n*sigma(x*y) - [sigma(x)*sigma(y)] 
            //denominator = sqrt{[n*sigma(sqr(x)) - sqr(sigma(x))] * [n*sigma(sqr(y)) - sqr(sigma(y))]}

            //check to make sure that the collection lengths are the same
            if (FirstList.Count != SecondList.Count)
            {
                System.ArgumentException argEx = new System.ArgumentException("In correlation coefficient calculation, arrays are not equal length", "FirstList and SecondList");
                throw argEx;
            }

            decimal dSum1;
            decimal dSum2;
            decimal dSum1And2;
            int n, i;
            decimal dNominator, dSumOfSquares1, dSumOfSquares2, dValue1, dValue2;
            double dDenominator;
            decimal dReturn;
            Descriptives desc;
            try
            {
                n = FirstList.Count;
                desc = new Descriptives();
                dSum1 = desc.SumOfValues(FirstList);
                dSum2 = desc.SumOfValues(SecondList);
                dSum1And2 = 0;
                for (i = 0; i < n; i++)
                {
                    dSum1And2 += Convert.ToDecimal(FirstList[i]) * Convert.ToDecimal(SecondList[i]);
                }
                dNominator = (n * dSum1And2) - (dSum1 * dSum2);

                dSumOfSquares1 = 0;
                for (i = 0; i < n; i++)
                {
                    dSumOfSquares1 += Convert.ToDecimal(FirstList[i]) * Convert.ToDecimal(FirstList[i]);
                }

                dSumOfSquares2 = 0;
                for (i = 0; i < n; i++)
                {
                    dSumOfSquares2 += Convert.ToDecimal(SecondList[i]) * Convert.ToDecimal(SecondList[i]);
                }

                dValue1 = n * dSumOfSquares1 - (dSum1 * dSum1);
                dValue2 = n * dSumOfSquares2 - (dSum2 * dSum2);
                dDenominator = Common.Sqrt(Convert.ToDouble(dValue1) * Convert.ToDouble(dValue2));

                dReturn = Convert.ToDecimal(dNominator) / Convert.ToDecimal(dDenominator);

                return dReturn;

            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute Pearson correlation coefficient value", e);
            }
        }

        public double CoVariance(List<double> FirstList, List<double> SecondList)
        {
            //check to make sure that the collection lengths are the same
            if (FirstList.Count != SecondList.Count)
            {
                System.ArgumentException argEx = new System.ArgumentException("In covariance calculation, arrays are not equal length", "FirstList and SecondList");
                throw argEx;
            }

            int i, n;
            double dMean1, dMean2;
            double dMeanSumOfMultiplied;
            double dValue, dReturn;
            Descriptives desc;
            List<double> MeansList;

            try
            {
                n = FirstList.Count;
                desc = new Descriptives();
                dMean1 = desc.Mean(FirstList);
                dMean2 = desc.Mean(SecondList);

                //In Excel, covariance is equal to the product of (correlation coefficient)(standard deviation of first variable)(standard deviation of second variable).

                //Excel uses this formula: COV(XY) = Mean(X*Y) - Mean(x) * Mean(y)
                MeansList = new List<double>();
                for (i = 0; i < n; i++)
                {
                    dValue = Convert.ToDouble(FirstList[i]) * Convert.ToDouble(SecondList[i]);
                    MeansList.Add(dValue);
                }
                dMeanSumOfMultiplied = desc.Mean(MeansList);
                dReturn = dMeanSumOfMultiplied - (dMean1 * dMean2);

                return dReturn;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot Compute Covariance", e);
            }
        }

        public decimal CoVariance(List<decimal> FirstList, List<decimal> SecondList)
        {
            //check to make sure that the collection lengths are the same
            if (FirstList.Count != SecondList.Count)
            {
                System.ArgumentException argEx = new System.ArgumentException("In covariance calculation, arrays are not equal length", "FirstList and SecondList");
                throw argEx;
            }

            int i, n;
            decimal dMean1, dMean2;
            decimal dMeanSumOfMultiplied;
            decimal dValue, dReturn;
            List<decimal> MeansList;
            Descriptives desc;

            try
            {
                n = FirstList.Count;
                desc = new Descriptives();
                dMean1 = desc.Mean(FirstList);
                dMean2 = desc.Mean(SecondList);

                //In Excel, covariance is equal to the product of (correlation coefficient)(standard deviation of first variable)(standard deviation of second variable).

                //Excel uses this formula: COV(XY) = Mean(X*Y) - Mean(x) * Mean(y)
                MeansList = new List<decimal>();
                for (i = 0; i < n; i++)
                {
                    dValue = Convert.ToDecimal(FirstList[i]) * Convert.ToDecimal(SecondList[i]);
                    MeansList.Add(dValue);
                }
                dMeanSumOfMultiplied = desc.Mean(MeansList);
                dReturn = dMeanSumOfMultiplied - (dMean1 * dMean2);

                return dReturn;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot Compute Covariance", e);
            }
        }

        public double SumOfCoDeviates(List<double> FirstList, List<double> SecondList)
        {
            //check to make sure that the collection lengths are the same
            if (FirstList.Count != SecondList.Count)
            {
                System.ArgumentException argEx = new System.ArgumentException("In sum of co-deviates calculation, arrays are not equal length", "FirstList and SecondList");
                throw argEx;
            }

            //calculate the sum of co-deviates between the paired values in the two arrays
            //formula is: sigma[x(i)*y(i)] - (sigma(xi) * sigma(yi) / n)

            int i, iLen;
            double dSum1, dSum2, dMultiply, dSigmaMultiply, dValue;
            Descriptives desc;
            try
            {
                iLen = FirstList.Count;
                desc = new Descriptives();
                //determine sums of values
                dSum1 = desc.SumOfValues(FirstList);
                dSum2 = desc.SumOfValues(SecondList);
                dMultiply = dSum1 * dSum2 / (double)iLen;

                //then, determine sigma[x(i)*y(i)]
                dSigmaMultiply = 0.0;
                for (i = 0; i < iLen; i++)
                {
                    dValue = Convert.ToDouble(FirstList[i]) * Convert.ToDouble(SecondList[i]);
                    dSigmaMultiply += dValue;
                }
                return dSigmaMultiply - dMultiply;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot Compute Sum of Co-Deviates", e);
            }

        }

        public decimal SumOfCoDeviates(List<decimal> FirstList, List<decimal> SecondList)
        {
            //check to make sure that the collection lengths are the same
            if (FirstList.Count != SecondList.Count)
            {
                System.ArgumentException argEx = new System.ArgumentException("In sum of co-deviates calculation, arrays are not equal length", "FirstList and SecondList");
                throw argEx;
            }

            //calculate the sum of co-deviates between the paired values in the two arrays
            //formula is: sigma[x(i)*y(i)] - (sigma(xi) * sigma(yi) / n)

            int i, iLen;
            decimal dSum1, dSum2, dMultiply, dSigmaMultiply, dValue;
            Descriptives desc;
            try
            {
                iLen = FirstList.Count;
                desc = new Descriptives();
                //determine sums of values
                dSum1 = desc.SumOfValues(FirstList);
                dSum2 = desc.SumOfValues(SecondList);
                dMultiply = dSum1 * dSum2 / (decimal)iLen;

                //then, determine sigma[x(i)*y(i)]
                dSigmaMultiply = 0;
                for (i = 0; i < iLen; i++)
                {
                    dValue = Convert.ToDecimal(FirstList[i]) * Convert.ToDecimal(SecondList[i]);
                    dSigmaMultiply += dValue;
                }
                return dSigmaMultiply - dMultiply;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot Compute Sum of Co-Deviates", e);
            }

        }

        public double StandardDeviationOfDifferences(List<double> FirstList, List<double> SecondList)
        {
            //check to make sure that the collection lengths are the same
            if (FirstList.Count != SecondList.Count)
            {
                System.ArgumentException argEx = new System.ArgumentException("In standard deviation of differences calculation, arrays are not equal length", "FirstList and SecondList");
                throw argEx;
            }

            //calculate the standard deviation of the differences

            int i, iLen;
            double dValue;
            List<double> Differences;
            Descriptives desc;
            try
            {
                iLen = FirstList.Count;
                desc = new Descriptives();
                //create and populate the list of differences
                Differences = new List<double>();
                for (i = 0; i < iLen; i++)
                {
                    dValue = Convert.ToDouble(FirstList[i]) - Convert.ToDouble(SecondList[i]);
                    Differences.Add(dValue);
                }
                return desc.SampleStandardDeviation(Differences);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot Compute standard deviation of differences", e);
            }

        }

        public decimal StandardDeviationOfDifferences(List<decimal> FirstList, List<decimal> SecondList)
        {
            //check to make sure that the collection lengths are the same
            if (FirstList.Count != SecondList.Count)
            {
                System.ArgumentException argEx = new System.ArgumentException("In standard deviation of differences calculation, arrays are not equal length", "FirstList and SecondList");
                throw argEx;
            }

            //calculate the standard deviation of the differences

            int i, iLen;
            decimal dValue;
            List<decimal> Differences;
            Descriptives desc;
            try
            {
                iLen = FirstList.Count;
                desc = new Descriptives();
                //create and populate the list of differences
                Differences = new List<decimal>();
                for (i = 0; i < iLen; i++)
                {
                    dValue = Convert.ToDecimal(FirstList[i]) - Convert.ToDecimal(SecondList[i]);
                    Differences.Add(dValue);
                }
                return desc.SampleStandardDeviation(Differences);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot Compute standard deviation of differences", e);
            }

        }
    }
}
