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
    public class PairwiseOneSampleTTest : TTest
    {
        public PairwiseOneSampleTTest()
            : base()
        {
        }

        public double TValue(List<double> List1, List<double> List2)
        {
            //this function calculates the pairwise T-test values from the same sample
            //it calculates the t ratio for making comparisons between the means of the two groups
            //if differences between various groups need to be compared, use IndependentSamples t-test function

            //no need to check arrays since they will be checked in descriptives calculations
            Descriptives desc;
            Correlations corr;
            double dMean1, dMean2, dDiff;
            double dUnbiasedSD;
            double dUnbiasedStandardError;
            double dTValue;
            int iLen;
            try
            {
                desc = new Descriptives();
                corr = new Correlations();
                dMean1 = desc.Mean(List1);
                dMean2 = desc.Mean(List2);
                dDiff = dMean1 - dMean2;
                //compute the unbiased standard deviation of the difference in values since the same cases gave these answers
                //this is how SPSS calculates the pairwise T-Test results
                dUnbiasedSD = corr.StandardDeviationOfDifferences(List1, List2);
                iLen = List1.Count;
                dUnbiasedStandardError = dUnbiasedSD / Common.Sqrt((double)iLen);
                //step 3: run the t-test: (dMean1-dMean2) / Unbiased standard error
                dTValue = dDiff / dUnbiasedStandardError;
                return dTValue;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot obtain pairwise one-sample T-test value", e);
            }
        }
    }
}
