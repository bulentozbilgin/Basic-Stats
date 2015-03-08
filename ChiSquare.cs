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
    public class ChiSquare : BaseC
    {
        public ChiSquare()
            : base()
        {
        }

        private DataTable ExpectedValues(DataTable XTabTable, int SampleSize)
        {
            /*  get the expected values dataset
             *  {expected col. frequency = col total / n
             *  expected row frequency = row total / n
             *  expected cell frequency = rowtotal * coltotal / n}
             */
            DataTable dtExpected;
            DataRow nr;
            Crosstab xtab;
            List<int> RowTotalList;
            List<int> ColTotalList;
            int iColCount, iRowCount;
            int i, k, iRow, iCol, iValue;
            string sColName;
            double dExpected;

            try
            {
                //get row and column totals
                xtab = new Crosstab();
                xtab.RowAndColumnTotals(ref XTabTable);
                iRowCount = XTabTable.Rows.Count;
                iColCount = XTabTable.Columns.Count;
                //form the list for row and column totals, assuming they are integers
                RowTotalList = new List<int>();
                //do not add the total in the last row, since it will be the total of all values in that column
                for (iRow = 0; iRow < XTabTable.Rows.Count - 1; iRow++)
                {
                    iValue = Convert.ToInt32(XTabTable.Rows[iRow][iColCount - 1]);
                    RowTotalList.Add(iValue);
                }
                //form the list for column totals, assuming they are integers; these totals will be found at the last row of the data table
                ColTotalList = new List<int>();
                foreach (DataRow row in XTabTable.Rows)
                {
                    if (row[0].ToString() == "Column Totals")
                    {
                        iValue = 0;
                        for (iCol = 1; iCol < iColCount-1; iCol++)
                        {
                            iValue = Convert.ToInt32(row[iCol]);
                            ColTotalList.Add(iValue);
                        }
                    }
                }

                //create a new datatable for expected values, using the field definitions of the cross-tab dataset,
                //but without its first and last columns, which are row names and row totals
                dtExpected = new DataTable();
                for (iCol = 1; iCol < iColCount - 1; iCol++)
                {
                    sColName = XTabTable.Columns[iCol].ColumnName;
                    dtExpected.Columns.Add(sColName, typeof(double));
                }

                //calculate and input the expected values into each cell
                for (i = 0; i < RowTotalList.Count; i++)
                {
                    iRow = RowTotalList[i];
                    nr = dtExpected.NewRow();
                    nr.BeginEdit();
                    for (k = 0; k < ColTotalList.Count; k++)
                    {
                        iCol = ColTotalList[k];
                        dExpected = (double)iCol / (double)SampleSize;
                        dExpected = dExpected * (double)iRow;
                        //      dExpected:=(iCol * iRow) / SampleSize;
                        nr[k] = dExpected;
                    }
                    nr.EndEdit();
                    dtExpected.Rows.Add(nr);
                }
                dtExpected.AcceptChanges();
                return dtExpected;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot form the chi-square expected values table", e);
            }
        }

        public double PearsonsChiSquare(List<double> RowList, List<double> ColumnList)
        {
            //call the internal routine to calculate the chi-square value
            int iSampleSize;
            DataTable XTabTable;
            Crosstab xtab;
            try
            {
                iSampleSize = RowList.Count;
                xtab = new Crosstab();
                XTabTable = xtab.CrosstabTable(RowList, ColumnList);
                return CalculatePearsonsChiSquare(XTabTable, iSampleSize,false);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute Pearson's chi square value.", e);
            }
        }

        public int DegreesOfFreedom(List<double> RowList, List<double> ColumnList, int SampleSize)
        {
            Crosstab xtab;
            DataTable XTabTable;
            DataTable dtExpected;
            int iLen, iResult;

            try
            {
                xtab = new Crosstab();
                XTabTable = xtab.CrosstabTable(RowList, ColumnList);
                iLen = RowList.Count;
                dtExpected = ExpectedValues(XTabTable, iLen);
                iResult = dtExpected.Rows.Count - 1;
                iResult = iResult * (dtExpected.Columns.Count - 1);
                return iResult;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute the chi-square degrees of freedom", e);
            }
        }

        public double MantelHaenszelChiSquare(List<double> RowList, List<double> ColumnList)
        {
            /* The Mantel-Haenszel chi-square statistic tests the alternative hypothesis that there is a linear association between the row variable and the
             * column variable. Both variables must lie on an ordinal scale.
             * this can be used only for ordinal values (ordered, but differences not important) and not for nominal (classification) values
             * Result = (n-1) * R^2 (correlation coefficient)
             * this is also known as the linear-by-linear association
             */
            Correlations corr;
            double dCorrelation;
            int iLen;

            try
            {
                corr = new Correlations();
                iLen = RowList.Count;
                dCorrelation = corr.PearsonCorrelationCoefficient(RowList, ColumnList);
                return (double)(Common.Square(dCorrelation) * (iLen - 1));
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute Mantel-Haenszel Chi Square value", e);
            }
        }

        public double FishersExactTest(List<double> RowList, List<double> ColumnList)
        {
            //ie the cross-tab is 2x2, and total sample is less than 20
            //call the internal routine to calculate chi-square value
            int iSampleSize;
            Crosstab x;
            DataTable dtLocal, dtExpected;
            double dTotValue, dExpected;
            bool bYates, bFisher, bBreak;
            int i;

            try
            {
                iSampleSize = RowList.Count;
                x = new Crosstab();
                dtLocal = x.CrosstabTable(RowList, ColumnList);
                //then, see whether this dataset is applicable for Fisher's exact test
                //first, determine the row and column expected frequencies
                dtExpected = ExpectedValues(dtLocal, iSampleSize);
                dTotValue = 0;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute Fisher's exact test", e);
            }

            if (iSampleSize > 20)
            {
                System.ArgumentException argEx = new System.ArgumentException("Fisher's Test is not applicable to this data; Sample size should be smaller than 20.", "FishersExactTest function");
                throw argEx;
            }

            //make sure that the two datasets have the same # of data rows and columns
            //Values dataset should have two more columns since (LABEL and COUNT)
            if (((dtLocal.Columns.Count - 2) == dtExpected.Columns.Count) && ((dtLocal.Rows.Count - 1) == dtExpected.Rows.Count))
            {
                try
                {
                    //see whether Yates or Fisher tests are necessary
                    bYates = ((dtExpected.Columns.Count == 2) && (dtExpected.Rows.Count == 2));
                    bFisher = false;
                    bBreak = false;
                    foreach (DataRow rExp in dtExpected.Rows)
                    {
                        for (i = 0; i < dtExpected.Columns.Count; i++)
                        {
                            dExpected = Convert.ToDouble(rExp[i]);
                            if (dExpected < 5 && bYates)
                            {
                                bBreak = true;
                                bFisher = true;
                            }
                        }
                        if (bBreak) { break; }
                    }
                    if (bFisher)
                    {
                        //apply Fisher's exact test
                        dTotValue = FishersExactTest_Local(dtLocal);
                    }
                }
                catch (Exception e)
                {
                    throw new ApplicationException("Cannot compute Fisher's exact test", e);
                }

            }
            else
            {
                System.ArgumentException argEx = new System.ArgumentException("Fisher's Test is not applicable to this data!", "FishersExactTest function");
                throw argEx;
            }
            return dTotValue;
        }

        public double PearsonsChiSquareWithYatesCorrection(List<double> RowList, List<double> ColumnList)
        {
            //calculate the chi-square, with Yates this
            //this is also called Continuity Correction
            //correction formula is a conservative approach, and usually applied to 2x2
            //tables with expected frequencies of less than 5 in cells; some people also apply it to all 2x2 tables

            Crosstab x;
            DataTable dt;
            int iSampleSize;
            try
            {
                x = new Crosstab();
                dt = x.CrosstabTable(RowList, ColumnList);
                iSampleSize = RowList.Count;
                //TODO: correct the following
                return CalculatePearsonsChiSquare(dt, iSampleSize,true);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute Pearson's chi-square with Yates correction", e);
            }
        }

        private double CalculatePearsonsChiSquare(DataTable XTabTable, int SampleSize, bool YatesCorrection)
        {
            //O: observed value; E: expected value
            //for each cell: (O - E)^2 / E. chi-square: sum of all these values
            //for 2x2, subtract an additional .5 from the |O - E|.
            //that is called the Yates' correction for continuity for Chi-Square
            //if the table is 2x2 and one of the expected values is below 5, then use Fisher's exact test
            DataTable dtExpected;
            DataTable dtLocal;
            double dTotValue, dExpected, dCalc;
            int iObserved;
            int iRow, iCol;

            try
            {
                //first, determine the row and column expected frequencies
                dtExpected = ExpectedValues(XTabTable, SampleSize);
                //use a local copy of the cross tab table that does not have totals information
                dtLocal = StrippedXTab(XTabTable);
                //make sure that the two datasets have the same # of data rows and columns
                //Values dataset should have two more columns since (LABEL and COUNT)
                if (dtLocal.Columns.Count == dtExpected.Columns.Count && (dtLocal.Rows.Count == dtExpected.Rows.Count))
                {
                    dTotValue = 0;
                    for (iRow = 0; iRow < dtLocal.Rows.Count; iRow++)
                    {
                        for (iCol= 0; iCol< dtLocal.Columns.Count; iCol++)
                        {
                            dExpected = Convert.ToDouble(dtExpected.Rows[iRow][iCol]);
                            iObserved = Convert.ToInt32(dtLocal.Rows[iRow][iCol]);
                            if (dExpected != 0)
                            {
                                if (YatesCorrection)
                                {
                                    dCalc = Common.Square((double)(System.Math.Abs(iObserved - dExpected) - 0.5)) / dExpected;
                                }
                                else
                                {
                                    dCalc = Common.Square((double)(iObserved - dExpected)) / dExpected;
                                }
                                dTotValue += dCalc;
                            }
                        }
                    }
                }
                else
                {
                    throw new ApplicationException("Two datasets do not have same # of data rows and columns! CalculatePearsonChiSquare function.");
                }
                return dTotValue;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute the Pearson's chi-square value", e);
            }
        }

        private double FishersExactTest_Local(DataTable XTabTable)
        {
            //calculate Fisher's exact test for small datasets
            Descriptives desc;
            double dTopValue, dBottomValue, dCutOff, dProbability, dTotProbability;
            int i, j;
            int iTotal, iTopRowTotal, iBottomRowTotal, iLeftColTotal, iRightColTotal;
            int[,] Matrix = new int[2, 2];
            int[] iCell;
            List<double> lstProbability;
            DataTable dtLocal;

            try
            {
                i = 0;
                //make sure that the data table passed to this function is stripped of row, column information, and totals
                dtLocal = StrippedXTab(XTabTable);
                foreach (DataRow r in dtLocal.Rows)
                {
                    Matrix[i, 0] = Convert.ToInt32(r[0]);
                    Matrix[i, 1] = Convert.ToInt32(r[1]);
                    i += 1;
                }
                //calculate the cut-off point, which represents the likelihood of having the observed values table
                iCell = new int[4];
                iCell[0] = Matrix[0, 0];
                iCell[1] = Matrix[0, 1];
                iCell[2] = Matrix[1, 0];
                iCell[3] = Matrix[1, 1];
                iTotal = 0;
                //calculate the total sum of values in the matrix
                for (i = 0; i < 4; i++)
                {
                    iTotal += iCell[i];
                }
                //calculate the row and column totals for all
                iTopRowTotal = iCell[0] + iCell[1];
                iBottomRowTotal = iCell[2] + iCell[3];
                iLeftColTotal = iCell[0] + iCell[2];
                iRightColTotal = iCell[1] + iCell[3];
                //then calculate the conditional probability of getting the actual matrix, given the particular row and column sums
                desc = new Descriptives();
                lstProbability = new List<double>();

                dTopValue = desc.Factorial(iTopRowTotal) * desc.Factorial(iBottomRowTotal) * desc.Factorial(iLeftColTotal) * desc.Factorial(iRightColTotal);
                dBottomValue = desc.Factorial(iTotal) * desc.Factorial(iCell[0]) * desc.Factorial(iCell[1]) * desc.Factorial(iCell[2]) * desc.Factorial(iCell[3]);
                dCutOff = dTopValue / dBottomValue;

                //then, calculate the probability value for all other possible matrices, with
                //the same marginal frequencies (ie Row and Column totals)
                for (i = 0; i < +iLeftColTotal; i++)
                {
                    iCell[0] = iLeftColTotal - i;
                    iCell[2] = i;
                    //then, do the same for the right column total
                    for (j = 0; j <= iRightColTotal; j++)
                    {
                        iCell[1] = iRightColTotal - j;
                        iCell[3] = j;
                        //in the resulting matrix, the value
                        //check whether all row and column totals are remaining the same, with the changed values
                        if ((iCell[0] + iCell[1] == iTopRowTotal) && (iCell[2] + iCell[3] == iBottomRowTotal) && (iCell[1] + iCell[3] == iRightColTotal))
                        {
                            //check whether this is different than the original matrix values
                            if ((iCell[0] == Matrix[0, 0]) && (iCell[1] == Matrix[0, 1]) && (iCell[2] == Matrix[1, 0]) && (iCell[3] == Matrix[1, 1]))
                            {
                                continue;
                            }
                            //calculate probability and add to the list
                            dTopValue = desc.Factorial(iTopRowTotal) * desc.Factorial(iBottomRowTotal) * desc.Factorial(iLeftColTotal) * desc.Factorial(iRightColTotal);
                            dBottomValue = desc.Factorial(iTotal) * desc.Factorial(iCell[0]) * desc.Factorial(iCell[1]) * desc.Factorial(iCell[2]) * desc.Factorial(iCell[3]);
                            dProbability = dTopValue / dBottomValue;
                            lstProbability.Add(dProbability);
                        }
                    }
                }
                //then, get the total of all probabilities below the cut-off point for two-tailed probability
                dTotProbability = dCutOff;
                for (i = 0; i < lstProbability.Count; i++)
                {
                    if (Convert.ToDouble(lstProbability[i]) <= dCutOff)
                    {
                        dTotProbability += Convert.ToDouble(lstProbability[i]);
                    }
                }
                return dTotProbability;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute Fisher's exact test", e);
            }
        }

        private bool Rejected(double ChiSquareValue, int DegreesOfFreedom, SignificanceLevel Significance)
        {
            //determine whether null hypothesis is rejected
            string sSignificance;
            DataTable dtChiSquare;
            double dCriticalValue;
            int iValue;

            try
            {
                sSignificance = Common.Significance(Significance);
                //to reject the null hypothesis (ie there is a significant relationship)
                //the calculated chi-square value must be greater than the critical value in
                //the table, corresponding to the level of significance and given df

                //if degree of freedom is larger than 100, then use the 100 values for comparison
                if (DegreesOfFreedom > 100) { DegreesOfFreedom = 100; }
                //get the critical value out of the table for comparison
                dtChiSquare = Lookup.ChiSquareTable();
                dCriticalValue = 0;
                foreach (DataRow r in dtChiSquare.Rows)
                {
                    iValue = Convert.ToInt32(r[0]);
                    if (iValue == DegreesOfFreedom)
                    {
                        dCriticalValue = Convert.ToDouble(r[sSignificance]);
                        break;
                    }
                }
                return Math.Abs(ChiSquareValue) > dCriticalValue;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute whether chi-square null hypothesis is rejected", e);
            }
        }

        private DataTable StrippedXTab(DataTable XTabTable)
        {
            //this function takes a cross tab table, checks for the existence of: "Rows" column, "Row Totals" column, and "Column Totals" row, and return a cross tab table without these values
            DataTable dtReturn;
            int iColCount, iRowCount;

            dtReturn = new DataTable();
            dtReturn = XTabTable.Copy();
            try
            {
                iRowCount = dtReturn.Rows.Count;
                if (dtReturn.Rows[iRowCount - 1][0].ToString() == "Column Totals")
                {
                    dtReturn.Rows.RemoveAt(iRowCount - 1);
                }
                dtReturn.AcceptChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
            try
            {
                if (dtReturn.Columns[0].ColumnName == "Rows")
                {
                    dtReturn.Columns.Remove("Rows");
                }
                dtReturn.AcceptChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
            try
            {
                iColCount = dtReturn.Columns.Count;
                if (dtReturn.Columns[iColCount - 1].ColumnName == "RowTotal")
                {
                    dtReturn.Columns.RemoveAt(iColCount - 1);
                }
                dtReturn.AcceptChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
            return dtReturn;
        }
    }
}
