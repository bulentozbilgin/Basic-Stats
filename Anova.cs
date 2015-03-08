/*********************************************************************
 *  
 * Copyright 2010 B. Bulent Ozbilgin
 * This program is distributed under the terms of the GNU Lesser General Public License (Lesser GPL)
 *********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

/*********************************************************************
 * The purpose of the Analysis of Variance (ANOVA) Technique is to test for significant
differences among two or more groups.

In single classification ANOVA, you are trying to find out if there is any
relationship between a dependent variable (such as student achievement) and
several classifications of one independent variable (such as different
instructional materials).

In multiple classification ANOVA, you are trying to find out the relationship
between one dependent variable (such as student achievement) and classifications
of two or more independent variables (such as several methods of instruction
and different instructional materials).

Therefore, the factor determining whether to use single or multiple
classification ANOVA is the number of independent variables.

Since the variance (or its square root, the standard deviation) is really an
average distance of the raw scores in a distribution of numbers from the mean
of that distribution, this functional relationship between the variance and
the mean can be used to determine mean differences by analyzing variances.

In essence, the ANOVA method is to calculate the variances of each subgroup
being compared. The average variance of these subgroups is then compared to
the variance of the total group (created by artificially combining the
subgroups). If the average variance of the subgroups is about the same as
the variance of the total group, then no significant difference exists among
the means of the subgroups. However, if the average variance of the subgroups
 is smaller than the variance of the total group, then the means of the
 subgroups are significantly different.
*/

namespace parStats.BasicStats
{
    public class Anova : BaseC
    {
        public Anova()
            : base()
        {
        }

        public int DegreeOfFreedomBetweenGroups(List<double> DependentList, List<double> IndependentList)
        {
            DataTable dtFreq;
            Frequency f;
            int iLen;

            //check to make sure that the collection lengths are the same
            if (DependentList.Count != IndependentList.Count)
            {
                System.ArgumentException argEx = new System.ArgumentException("In degree of freedom between groups calculation, collections are not equal length", "DependentList and IndependentList");
                throw argEx;
            }

            try
            {
                //form an array containing dependent values for each independent value
                f = new Frequency();
                dtFreq = f.FrequencyList(IndependentList);
                iLen = dtFreq.Rows.Count;
                return iLen - 1;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute degree of freedom between groups", e);
            }
        }

        public int DegreeOfFreedomWithinGroups(List<double> DependentList, List<double> IndependentList)
        {
            //check to make sure that the collection lengths are the same
            if (DependentList.Count != IndependentList.Count)
            {
                System.ArgumentException argEx = new System.ArgumentException("In degree of freedom within groups calculation, collections are not equal length", "DependentList and IndependentList");
                throw argEx;
            }

            Frequency f;
            DataTable dtIndependent;
            int idfWithinGroups, i,j, iCount;
            double dIndependentValue;

            try
            {
                f = new Frequency();
                idfWithinGroups = 0;
                //form a table containing dependent values for each independent value
                //to do that, get the frequency lists for the independent and dependent variables
                dtIndependent= f.FrequencyList(IndependentList);
                //add one more column to the frequency table, to have the corresponding counts for dependent variable, and initialize values to 0
                dtIndependent.Columns.Add("DependentCount", typeof(int));
                foreach (DataRow r in dtIndependent.Rows)
                {
                    r["DependentCount"] = 0;
                }
                //then, count the unique dependent values for each independent row's value, assuming dependent and independent lists are paired
                for (i = 0; i < dtIndependent.Rows.Count; i++)
                {
                    dIndependentValue = Convert.ToDouble(dtIndependent.Rows[i]["ItemValue"]);
                    iCount = 0;
                    for (j = 0; j < DependentList.Count; j++)
                    {
                        if (dIndependentValue == Convert.ToDouble(IndependentList[j])) { iCount += 1; }
                    }
                    dtIndependent.Rows[i]["DependentCount"] = iCount;
                }
                //TODO: TEST CAREFULLY SINCE THIS IS DIFFERENT THAN sqsAnova.pas

                //calculate degree of freedom within groups
                //this is computed as (n1-1)+(n2-1)+(n3-1)+....
                idfWithinGroups = 0;
                foreach (DataRow r in dtIndependent.Rows)
                {
                    iCount = Convert.ToInt32(r["DependentCount"]);
                    if (iCount != 0) { idfWithinGroups += (iCount - 1); }
                }

                return idfWithinGroups;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute degree of freedom within groups", e);
            }
        }

        public double FRatio(List<double> DependentList, List<double> IndependentList)
        {
            //check to make sure that the collection lengths are the same
            if (DependentList.Count != IndependentList.Count)
            {
                System.ArgumentException argEx = new System.ArgumentException("In F-ratio calculation, collections are not equal length", "DependentList and IndependentList");
                throw argEx;
            }

            Frequency f;
            Descriptives desc;
            DataTable dtFreq;
            DataRow r;
            double dIndependent, dCompare, dAddValue, dMeanTotal, dSSBetweenGroups, dSSWithinGroups;
            double dVarianceEstimateBetweenGroups, dVarianceEstimateWithinGroups;
            double dFRatio;
            //create a list of lists; each list will contain all corresponding dependent values for one independent value
            List<List<double>> lstDependents;
            List<double> lstDependentValues;
            List<double> lstSumOfSquares;
            List<double> lstMean;
            List<double> lstMeanSquare;
            lstDependents = new List<List<double>>();
            int idfBetweenGroups, idfWithinGroups;
            int i,j, iLen;

            try
            {
                f=new Frequency();
                dtFreq = f.FrequencyList(IndependentList);
                iLen = dtFreq.Rows.Count;
                for (i = 0; i < iLen; i++) {
                    r = dtFreq.Rows[i];
                    dIndependent = Convert.ToDouble(r["ItemValue"]);
                    //form the list of dependent values for the selected independent value
                    lstDependentValues = new List<double>();
                    for (j = 0; j < DependentList.Count; j++) {
                        dCompare = Convert.ToDouble(IndependentList[j]);
                        dAddValue = Convert.ToDouble(DependentList[j]);
                        if (dCompare == dIndependent) {
                            lstDependentValues.Add(dAddValue);
                        }
                    }
                    lstDependents.Add(lstDependentValues);
                    lstDependentValues = null;
                }
                
                //now we have a list of lists, with each sub-list containing all corresponding dependent values for each independent value
                //the connection between this list of lists and dtFreq is done through the index value, ie "i"
                //these dependent values will be tested for ANOVA
                lstSumOfSquares = new List<double>();
                desc = new Descriptives();
                for (i = 0; i < iLen; i++) {
                    dAddValue = desc.SumOfSquareDeviates(lstDependents[i]);
                    lstSumOfSquares.Add(dAddValue);
                }
                //now compute the means of each of these lists
                lstMean = new List<double>();
                for (i = 0; i < iLen; i++) {
                    dAddValue = desc.Mean(lstDependents[i]);
                    lstMean.Add(dAddValue);
                }
                dMeanTotal = desc.Mean(DependentList);
                //then, get a square deviate of these means from the overall group mean and put them in a list
                lstMeanSquare = new List<double>();
                for (i = 0; i < iLen; i++) {
                    dAddValue = Common.Square((double)lstMean[i] - dMeanTotal);
                    lstMeanSquare.Add(dAddValue);
                }
                //then, calculate the corresponding "sum" of square deviates of means by multiplying with number of cases in each group
                //this is an aggregate measure of the degree to which the different means differ from one another
                dSSBetweenGroups = 0;
                for (i = 0; i < iLen; i++)
                {
                    dAddValue = lstDependents[i].Count * lstMeanSquare[i];
                    dSSBetweenGroups += dAddValue;
                }
                //then, calculate the sum of squares within groups
                dSSWithinGroups = 0;
                for (i = 0; i < iLen; i++)
                {
                    dSSWithinGroups += lstSumOfSquares[i];
                }
                  //figure out the degree of freedom between groups
                idfBetweenGroups=iLen-1;
                //calculate an estimate of the source population variance
                try { dVarianceEstimateBetweenGroups = dSSBetweenGroups / idfBetweenGroups; }
                catch (Exception) { dVarianceEstimateBetweenGroups = 0; }
                //calculate degree of freedom within groups
                //this is computed as (n1-1)+(n2-1)+(n3-1)+....
                idfWithinGroups = 0;
                for (i = 0; i < iLen; i++)
                {
                    idfWithinGroups += (lstDependents[i].Count-1);
                }
                //then compute variance estimate within groups
                try { dVarianceEstimateWithinGroups = dSSWithinGroups / idfWithinGroups; }
                catch (Exception) { dVarianceEstimateWithinGroups = 0; }
                //the relationship between these two values is known as the F-ratio
                //F = dVarianceEstimateBetweenGroups / dVarianceEstimateWithinGroups
                try { dFRatio = dVarianceEstimateBetweenGroups / dVarianceEstimateWithinGroups; }
                catch (Exception) { dFRatio = 0; }

                return dFRatio;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute F ratio", e);
            }
        }

        public Boolean Rejected(double FRatio, int DegreeOfFreedomBetweenGroups, int DegreeOfFreedomWithinGroups, SignificanceLevel Significance)
        {
            //given the degrees of freedom, F ratio, and the wanted significance level,
            //determine whether the null hypothesis should be rejected

            //if F-ratio > 1, then null hypothesis is likely to be rejected; here we are
            //trying to determine the significance of the rejection
            string sSignificance;
            DataTable dtFRatio;
            double dCriticalValue;

            sSignificance = Common.Significance(Significance);

            //here, numerator is dfbetweengroups while denominator is dfwithingroups
            //if denominator is greater than 100 than equate it to 100
            //do the same for the numerator, for 20
            if (DegreeOfFreedomWithinGroups > 100) { DegreeOfFreedomWithinGroups = 100; }
            if (DegreeOfFreedomBetweenGroups > 20) { DegreeOfFreedomBetweenGroups = 20; }

            //get F ratio table into a local datatable
            dtFRatio = Lookup.FRatioCriticalValueTable();
            dCriticalValue = 0;
            foreach (DataRow r in dtFRatio.Rows)
            {
                if ((Convert.ToInt32(r[0]) == DegreeOfFreedomWithinGroups) && (Convert.ToInt32(r[1]) == DegreeOfFreedomBetweenGroups))
                {
                    dCriticalValue = Convert.ToDouble(r[sSignificance]);
                    break;
                }
            }

            return Math.Abs(FRatio) > dCriticalValue;
        }

        public double SumOfSquaresBetweenGroups(List<double> DependentList, List<double> IndependentList)
        {
            //check to make sure that the collection lengths are the same
            if (DependentList.Count != IndependentList.Count)
            {
                System.ArgumentException argEx = new System.ArgumentException("In sum of squares between groups calculation, collections are not equal length", "DependentList and IndependentList");
                throw argEx;
            }

            Frequency f;
            Descriptives desc;
            DataTable dtFreq;
            DataRow r;
            double dIndependent, dCompare, dAddValue, dMeanTotal, dSSBetweenGroups;
            //create a list of lists; each list will contain all corresponding dependent values for one independent value
            List<List<double>> lstDependents;
            List<double> lstDependentValues;
            List<double> lstMean;
            List<double> lstMeanSquare;
            lstDependents = new List<List<double>>();
            int i, j, iLen;

            try
            {
                f = new Frequency();
                desc = new Descriptives();
                dtFreq = f.FrequencyList(IndependentList);
                iLen = dtFreq.Rows.Count;
                for (i = 0; i < iLen; i++)
                {
                    r = dtFreq.Rows[i];
                    dIndependent = Convert.ToDouble(r["ItemValue"]);
                    //form the list of dependent values for the selected independent value
                    //if the independent list is not made up of ordinal values, then the calculation takes much time
                    //especially if independent list is time-series or continuous data, then performance becomes very bad; 
                    //SPSS does not even make the calculation in that case; it will be better to use the t-test for these calculations
                    lstDependentValues = new List<double>();
                    for (j = 0; j < DependentList.Count; j++)
                    {
                        dCompare = Convert.ToDouble(IndependentList[j]);
                        dAddValue = Convert.ToDouble(DependentList[j]);
                        if (dCompare == dIndependent)
                        {
                            lstDependentValues.Add(dAddValue);
                        }
                    }
                    lstDependents.Add(lstDependentValues);
                    lstDependentValues = null;
                }
                //now compute the means of each of these lists
                lstMean = new List<double>();
                for (i = 0; i < iLen; i++)
                {
                    dAddValue = desc.Mean(lstDependents[i]);
                    lstMean.Add(dAddValue);
                }
                dMeanTotal = desc.Mean(DependentList);
                //then, get a square deviate of these means from the overall group mean and put them in a list
                lstMeanSquare = new List<double>();
                for (i = 0; i < iLen; i++)
                {
                    dAddValue = Common.Square((double)lstMean[i] - dMeanTotal);
                    lstMeanSquare.Add(dAddValue);
                }
                //then, calculate the corresponding "sum" of square deviates of means by multiplying with number of cases in each group
                //this is an aggregate measure of the degree to which the different means differ from one another
                dSSBetweenGroups = 0;
                for (i = 0; i < iLen; i++)
                {
                    dAddValue = lstDependents[i].Count * lstMeanSquare[i];
                    dSSBetweenGroups += dAddValue;
                }
                return dSSBetweenGroups;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute sum of squares between groups", e);
            }
        }

        public double SumOfSquaresWithinGroups(List<double> DependentList, List<double> IndependentList)
        {
            //check to make sure that the collection lengths are the same
            if (DependentList.Count != IndependentList.Count)
            {
                System.ArgumentException argEx = new System.ArgumentException("In sum of squares within groups calculation, collections are not equal length", "DependentList and IndependentList");
                throw argEx;
            }

            Frequency f;
            Descriptives desc;
            DataTable dtFreq;
            DataRow r;
            double dIndependent, dCompare, dAddValue,  dSSWithinGroups;
            //create a list of lists; each list will contain all corresponding dependent values for one independent value
            List<List<double>> lstDependents;
            List<double> lstDependentValues;
            List<double> lstSumOfSquares;
            lstDependents = new List<List<double>>();
            int i, j, iLen;

            try
            {
                f = new Frequency();
                desc = new Descriptives();
                dtFreq = f.FrequencyList(IndependentList);
                iLen = dtFreq.Rows.Count;
                for (i = 0; i < iLen; i++)
                {
                    r = dtFreq.Rows[i];
                    dIndependent = Convert.ToDouble(r["ItemValue"]);
                    //form the list of dependent values for the selected independent value
                    //if the independent list is not made up of ordinal values, then the calculation takes much time
                    //especially if independent list is time-series or continuous data, then performance becomes very bad; 
                    //SPSS does not even make the calculation in that case; it will be better to use the t-test for these calculations
                    lstDependentValues = new List<double>();
                    for (j = 0; j < DependentList.Count; j++)
                    {
                        dCompare = Convert.ToDouble(IndependentList[j]);
                        dAddValue = Convert.ToDouble(DependentList[j]);
                        if (dCompare == dIndependent)
                        {
                            lstDependentValues.Add(dAddValue);
                        }
                    }
                    lstDependents.Add(lstDependentValues);
                    lstDependentValues = null;
                }
                //now we have a list of lists, with each sub-list containing all corresponding dependent values for each independent value
                //the connection between this list of lists and dtFreq is done through the index value, ie "i"
                //these dependent values will be tested for ANOVA
                lstSumOfSquares = new List<double>();
                desc = new Descriptives();
                for (i = 0; i < iLen; i++)
                {
                    dAddValue = desc.SumOfSquareDeviates(lstDependents[i]);
                    lstSumOfSquares.Add(dAddValue);
                }
                //then, calculate the sum of squares within groups
                dSSWithinGroups = 0;
                for (i = 0; i < iLen; i++)
                {
                    dSSWithinGroups += lstSumOfSquares[i];
                }
                return dSSWithinGroups;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute sum of squares within groups", e);
            }
        }

    }
}
