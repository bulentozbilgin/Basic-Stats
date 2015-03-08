/*********************************************************************
 *  
 * Copyright 2010 B. Bulent Ozbilgin
 * This program is distributed under the terms of the GNU Lesser General Public License (Lesser GPL)
 *********************************************************************
 *
 */

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;


namespace parStats.BasicStats
{
    public class Crosstab :BaseC
    {
        public Crosstab()
            : base()
        {
        }

        public DataTable CrosstabTable(List<double> RowList, List<double> ColumnList)
        {
            //take a list of row variable, and a list of column variable as parameters, and return the cross-tabulation
            //of rows against columns
            //this function returns only the counts in the cross tab tables, and row and column labels, as doubles
            //it does not compute row totals, column totals, and percentages

            //no need to run checks since they will be ran when Frequency analysis is being ran
            Frequency freq;
            DataTable dtRowFreq, dtColFreq;
            DataTable dtReturn;
            DataRow nr;
            double dRowValue, dColValue;
            int iCount;
            int i, j;

            try
            {
                freq = new Frequency();
                dtRowFreq = freq.FrequencyList(RowList);
                dtColFreq=freq.FrequencyList(ColumnList);
                dtReturn = new DataTable();
                dtReturn.Columns.Add("Rows", typeof(string));
                //add other columns to dtReturn, all as int type since they will contain counts
                foreach (DataRow r in dtColFreq.Rows)
                {
                    dtReturn.Columns.Add((string)r[0], typeof(int));
                }
                //add rows to dtReturn, to the first column
                foreach (DataRow r in dtRowFreq.Rows)
                {
                    nr = dtReturn.NewRow();
                    nr.BeginEdit();
                    nr[0] = Convert.ToString(r[0]);
                    nr.EndEdit();
                    dtReturn.Rows.Add(nr);
                }
                //iterate through each cell in the return table, and get counts for each
                foreach (DataRow Row in dtReturn.Rows)
                {
                    dRowValue = Convert.ToDouble(Row[0]);
                    for (i = 1; i < dtReturn.Columns.Count; i++)
                    {
                        //skip the first column, start from the 2nd
                        DataColumn col = dtReturn.Columns[i];
                        dColValue = Convert.ToDouble(col.ColumnName);
                        //see how many pairings exist for the given row and column values in the lists
                        iCount = 0;
                        for (j = 0; j < RowList.Count; j++)
                        {
                            double RowItem = RowList[j];
                            double ColItem = ColumnList[j];
                            if (RowItem == dRowValue && ColItem == dColValue) { iCount += 1; }
                        }
                        //write the count into the current cell
                        Row[col] = iCount;
                    }
                }
                return dtReturn;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot form the cross-tabulation table", e);
            }
        }

        public void RowAndColumnTotals(ref DataTable XTabTable)
        {
            //take the well formed cross-tab table, and add one column for row totals, and one row for column totals
            int iCol, iRowTotal, iColTotal, iColCount;
            DataRow nr;

            try
            {
                iColCount = XTabTable.Columns.Count;
                XTabTable.Columns.Add("RowTotal", typeof(int));
                //compute the row totals and put them in the new column
                foreach (DataRow r in XTabTable.Rows)
                {
                    iRowTotal = 0;
                    //start iterating columns from one since the first column is the row labels
                    for (iCol = 1; iCol < iColCount; iCol++)
                    {
                        iRowTotal += Convert.ToInt32(r[iCol]);
                    }
                    r["RowTotal"] = iRowTotal;
                }
                XTabTable.AcceptChanges();
                //then, add a row to the table, and start adding up columns,starting from the second column
                iColCount = XTabTable.Columns.Count;
                nr = XTabTable.NewRow();
                nr.BeginEdit();
                nr[0] = "Column Totals";
                for (iCol = 1; iCol < iColCount; iCol++)
                {
                    iColTotal = 0;
                    foreach (DataRow r in XTabTable.Rows)
                    {
                        iColTotal += Convert.ToInt32(r[iCol]);
                    }
                    nr[iCol] = iColTotal;
                }
                nr.EndEdit();
                XTabTable.Rows.Add(nr);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot compute row and column totals in cross-tab table", e);
            }

        }

    }
}
