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
using System.Data;

namespace parStats.BasicStats
{
    public class OneSampleTTest : TTest
    {
        public OneSampleTTest():base()
        {
        }

        public double TValue(List<double> MyList, double AssumedPopulationMean)
        {
            //this is a t-test for a given population mean; it is used to calculate whether
            //the sample mean is significantly different than the (assumed) population mean

            Descriptives desc;
            double dMean, dStandardError, dDiff, dTValue;
            try
            {
                //step 1: calculate the mean for the sample
                desc = new Descriptives();
                dMean = desc.Mean(MyList);
                //step 2: calculate the standard error for the sample
                dStandardError = desc.StandardError(MyList);
                //step 3: run the t-test: dMean - AssumedPopulationMean / Standard Error of the Mean
                dDiff = dMean - AssumedPopulationMean;
                dTValue = dDiff / dStandardError;
                return dTValue;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot obtain T-test value", e);
            }
        }

        //public decimal TValue(List<decimal> MyList, decimal AssumedPopulationMean)
        //{
        //    //this is a t-test for a given population mean; it is used to calculate whether
        //    //the sample mean is significantly different than the (assumed) population mean

        //    Descriptives desc;
        //    decimal dMean, dStandardError, dDiff, dTValue;
        //    try
        //    {
        //        //step 1: calculate the mean for the sample
        //        desc = new Descriptives();
        //        dMean = desc.Mean(MyList);
        //        //step 2: calculate the standard error for the sample
        //        dStandardError = desc.StandardError(MyList);
        //        //step 3: run the t-test: dMean - AssumedPopulationMean / Standard Error of the Mean
        //        dDiff = dMean - AssumedPopulationMean;
        //        dTValue = dDiff / dStandardError;
        //        return dTValue;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new ApplicationException("Cannot obtain T-test value", e);
        //    }
        //}
    }
}
