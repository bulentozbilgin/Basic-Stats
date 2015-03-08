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
    public class Regression : BaseC
    {
        public Regression()
            : base()
        {
        }

        public double Slope_RegressionCoefficient(List<double> IndependentList, List<double> DependentList)
        {
            //X-axis is supposed to be the independent axis, Y the dependent axis
            //slope is calculated as: sum of codeviates(x,y) / sum of squares (x)

            //check to make sure that the collection lengths are the same
            if (DependentList.Count != IndependentList.Count)
            {
                System.ArgumentException argEx = new System.ArgumentException("In regression coefficient calculation, collections are not equal length", "DependentList and IndependentList");
                throw argEx;
            }

            Correlations corr;
            Descriptives desc;
            double dSumOfSquares, dSumOfCoDeviates;
            try
            {
                desc = new Descriptives();
                corr = new Correlations();
                dSumOfSquares = desc.SumOfSquareDeviates(IndependentList);
                dSumOfCoDeviates = corr.SumOfCoDeviates(IndependentList, DependentList);

                return dSumOfCoDeviates / dSumOfSquares;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute slope_regression coefficient", e);
            }
        }

        public double StandardErrorOfEstimate(List<double> IndependentList, List<double> DependentList)
        {
            //calculate the standard error of the estimated line
            //SE = sqrt[(SSresidual / (N-2)]

            //check to make sure that the collection lengths are the same
            if (DependentList.Count != IndependentList.Count)
            {
                System.ArgumentException argEx = new System.ArgumentException("In standard error of estimate calculation, collections are not equal length", "DependentList and IndependentList");
                throw argEx;
            }

            int iLen;
            Descriptives desc;
            Correlations corr;
            double dCoEff, dRSquare, dSumOfSquareDeviatesForY, dSSResidual;
            try
            {
                iLen = IndependentList.Count;
                //calculate SSResidual = SS(y) * (1-r^2)
                desc = new Descriptives();
                corr = new Correlations();
                //calculate the correlation coefficient, R
                dCoEff = corr.PearsonCorrelationCoefficient(IndependentList, DependentList);
                dRSquare = dCoEff * dCoEff;
                dSumOfSquareDeviatesForY = desc.SumOfSquareDeviates(DependentList);
                dSSResidual = dSumOfSquareDeviatesForY * (1 - dRSquare);
                return Common.Sqrt((double)(dSSResidual / (iLen - 2)));
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute standard error of estimate in regression", e);
            }
        }

        public double Intercept(List<double> IndependentList, List<double> DependentList)
        {
            //X-axis is supposed to be the independent axis, Y the dependent axis
            //the intercept is calculated as: mean(Y) - slope * mean(x)

            //check to make sure that the collection lengths are the same
            if (DependentList.Count != IndependentList.Count)
            {
                System.ArgumentException argEx = new System.ArgumentException("In regression intercept calculation, collections are not equal length", "DependentList and IndependentList");
                throw argEx;
            }

            Descriptives desc;
            double dMeanX, dMeanY, dSlope;
            try
            {
                desc = new Descriptives();
                dSlope = Slope_RegressionCoefficient(IndependentList, DependentList);
                dMeanX = desc.Mean(IndependentList);
                dMeanY = desc.Mean(DependentList);
                return dMeanY - (dSlope * dMeanX);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute the regression intercept", e);
            }

        }
    }
}
