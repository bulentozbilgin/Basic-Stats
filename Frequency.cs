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

//This unit conducts basic Frequency analysis

namespace parStats.BasicStats
{
    public class Frequency : BaseC
    {
        public  Frequency():base()
        {
        }

        public System.Data.DataTable FrequencyList(List<double> MyList)
        {
            DataTable dtReturn;
            List<object> ProcessedList;
            Boolean bExist;
            DataRow nr;
            DataRow pr;
            List<double> LocalMyList;
            long Count;
            int i;
            long lTotal;
            double dPercent;

            //create the data table with four columns
            try
            {
                dtReturn = new DataTable();
                dtReturn.Columns.Add("ItemValue");
                dtReturn.Columns.Add("Count");
                dtReturn.Columns.Add("Percent");
                dtReturn.Columns.Add("CumulativePercent");

                //create a list to contain processed items; faster than a datatable
                ProcessedList = new List<object>();

                //need to copy parameters to local lists before sorting them; otherwise performing operations on the originating variable changes it since it's passed by reference
                LocalMyList = new List<double>();
                foreach (double item in MyList)
                {
                    LocalMyList.Add(item);
                }

                //sort the initial list
                LocalMyList.Sort();

                //iterate through the list and find count of each item and, insert into the datatable
                foreach(object item in LocalMyList)
                {
                    bExist=false;
                    foreach(object p in ProcessedList)
                    {
                        if (item.ToString()==p.ToString()) { bExist=true; break; }
                    }
                    if(bExist==false)
                    {
                        //insert the item count as 1
                        nr=dtReturn.NewRow();
                        nr.BeginEdit();
                        nr.SetField<string>(0, item.ToString());
                        nr.SetField<int>(1, 1);
                        //initialize the remaining two columns, which will be processed later
                        nr.SetField<int>(2, 0);
                        nr.SetField<int>(3, 0);
                        nr.EndEdit();
                        dtReturn.Rows.Add(nr);
                        nr=null;
                        dtReturn.AcceptChanges();
                        ProcessedList.Add(item);
                    }
                    else
                    {
                        //add 1 to the item count in the datatable
                        foreach(DataRow r in dtReturn.Rows)
                        {
                            if (r.ItemArray[0].ToString() == item.ToString())
                            {
                                Count = Convert.ToInt64(r.ItemArray[1]);
                                Count += 1;
                                r[1] = Count;
                            }
                        }
                    }
                }
                dtReturn.AcceptChanges();

                //after that, compute the percentage for each row, and the cumulative percentage
                lTotal = 0;
                foreach (DataRow r in dtReturn.Rows)
                {
                    lTotal += Convert.ToInt64(r[1]);
                }
                //compute percentages for each row and insert
                foreach (DataRow r in dtReturn.Rows)
                {
                    dPercent = Convert.ToDouble(r[1]) / (double)lTotal;
                    r[2] = dPercent;
                }
                //compute cumulative percentages for each row
                nr = null;
                for (i = 0; i < dtReturn.Rows.Count; i++)
                {
                    nr = dtReturn.Rows[i];
                    if(i==0)
                    {
                        nr[3]=nr[2]; 
                    }
                    else
                    {
                        pr = dtReturn.Rows[i - 1];
                        nr[3] = Convert.ToDouble(pr[3]) + Convert.ToDouble(nr[2]);
                    }
                }
                return dtReturn;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot obtain the frequency list table", e);
            }
        }

        public List<CutoffPoint> CutoffPoints(List<double> PercentileList, List<double> TargetData)
        {
            //this function takes a list of cut-off points as percentiles and returns the
            //cut-off points for the target list
            //for example, if quartiles are wanted, then PercentileList would be 0, 0.25, etc
            //for 10 equal groups, it would be 0.10, 0.20, 0.30 etc; other values would be given

            //NOTE: we are only getting the cut-off points from the given list, not calculating the quartile values here.
            //If we were to compute the quartile etc values, we may end up with values that are not in the given list; that's not going to happen in this calculation.
            //The difference is important, and can be confusing since, for example, SPSS handles quartiles and cut-off points as calculated values that may not exist in the given list
            //ie if the value list has 24 and 25 in it, rather than having 25, for instance, 24.3 may be one of the calculated values.  In this calculation, only the given values are used as limits
            DataTable dtFreq;
            double dPercent,dValue;
            List<CutoffPoint> lstReturn;
            int i;

            try
            {
                //run the frequency analysis and get the data table
                dtFreq = FrequencyList(TargetData);
                lstReturn = new List<CutoffPoint>();
                //now, take the asked-for percent values and compare with the cumulative percent list
                i = 0;
                foreach (object percent in PercentileList)
                {
                    dPercent = (double)percent;
                    foreach (DataRow r in dtFreq.Rows)
                    {
                        //get the cumulative percent value for the row
                        dValue = Convert.ToDouble(r[3]);
                        if (dValue > dPercent)
                        {
                            lstReturn.Add(new CutoffPoint()
                            {
                                ID=i,
                                value=r[0].ToString(),
                                PointValue=Convert.ToString(percent)
                            });
                            i += 1;
                            break;
                        }
                    }
                }
                return lstReturn;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot obtain the cut-off points table", e);
            }
        }

        public List<ReturnRow> Outliers(List<double> MyList)
        {
            List<ReturnRow> ReturnRow;
            List<double> LocalMyList;
            int i, iCount;
            bool bExist;

            //return the five lowest and five largest data values

            try
            {
                ReturnRow = new List<ReturnRow>();
                //need to copy parameters to local lists before sorting them; otherwise performing operations on the originating variable changes it since it's passed by reference
                LocalMyList = new List<double>();
                foreach (double item in MyList)
                {
                    LocalMyList.Add(item);
                }

                LocalMyList.Sort();
                if (LocalMyList.Count > 10)
                {
                    //get the five lowest values
                    iCount = 0;
                    for (i = 0; i < LocalMyList.Count; i++)
                    {
                        if (iCount < 5)
                        {
                            ReturnRow item = new ReturnRow()
                            {
                                ID = iCount,
                                value = Convert.ToString(LocalMyList[i])
                            };
                            bExist = false;
                            foreach (ReturnRow test in ReturnRow)
                            {
                                if (item.value == test.value)
                                {
                                    bExist = true;
                                    break;
                                }
                            }
                            if (bExist == false)
                            {
                                ReturnRow.Add(item);
                                iCount += 1;
                            }
                        }
                    }
                    //get the five highest values
                    iCount = 0;
                    for (i = LocalMyList.Count - 1; i >= 0; i--)
                    {
                        if (iCount < 5)
                        {
                            ReturnRow item = new ReturnRow()
                            {
                                ID = iCount,
                                value = Convert.ToString(LocalMyList[i])
                            };
                            bExist = false;
                            foreach (ReturnRow test in ReturnRow)
                            {
                                if (item.value == test.value)
                                {
                                    bExist = true;
                                    break;
                                }
                            }
                            if (bExist == false)
                            {
                                ReturnRow.Add(item);
                                iCount += 1;
                            }
                        }
                    }
                    return ReturnRow;
                }
                else
                {
                    //return the whole list
                    i = 0;
                    foreach (object d in LocalMyList)
                    {
                        ReturnRow.Add(new ReturnRow() 
                        { 
                            ID=i,
                            value=Convert.ToString(d) 
                        });
                        i += 1;
                    }
                    return ReturnRow;
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot obtain the outliers table", e);
            }
        }

    }
}
