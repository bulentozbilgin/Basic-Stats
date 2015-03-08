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
    public class IndependentSamplesTTest : TTest
    {
        public IndependentSamplesTTest()
            : base()
        {
        }

        public int DegreesOfFreedom(List<double> List1, List<double> List2)
        {
            //first, check to make sure that the values in the lists are numeric
            if (Common.CheckArray(ArrayType.Numeric, List1) == false)
            {
                System.ArgumentException argEx = new System.ArgumentException("The list is empty, or a value in the List1 parameter is not numeric", "List1");
                throw argEx;
            }
            if (Common.CheckArray(ArrayType.Numeric, List2) == false)
            {
                System.ArgumentException argEx = new System.ArgumentException("The list is empty, or a value in the List2 parameter is not numeric", "List2");
                throw argEx;
            }

            return List1.Count + List2.Count - 2;
        }

        public double TValue(List<double> List1, List<double> List2)
        {
            //calculate the Student's t-test value for independent samples

            //no need to check arrays since they will be checked in descriptives calculations
            Descriptives desc;
            double dMean1, dMean2, dVar1, dVar2, dDiff;
            int n1, n2;
            double dStandardDeviation, dStandardError;
            double dTValue;
            try
            {
                desc = new Descriptives();
                //step 1: calculate the mean for each sample
                dMean1 = desc.Mean(List1);
                dMean2 = desc.Mean(List2);
                //step 2: calculate the variance for each sample
                dVar1 = desc.SampleVariance(List1);
                dVar2 = desc.SampleVariance(List2);
                //step 3: run the t-test: |dMean1-dMean2| / Sqrt((dVar1/n1) + (dvar2/n2))
                dDiff = dMean1 - dMean2;
                n1 = List1.Count;
                n2 = List2.Count;
                dStandardDeviation = (dVar1 / n1) + (dVar2 / n2);
                dStandardError = Common.Sqrt(dStandardDeviation);
                dTValue = dDiff / dStandardError;

                return dTValue;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot obtain Independent Samples T-test value", e);
            }
        }
    }
}
