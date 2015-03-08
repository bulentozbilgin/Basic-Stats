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
    public class TTest : BaseC
    {
        public TTest()
            : base()
        {
        }

        public int DegreesOfFreedom(List<double> MyList)
        {
            try
            {
                return MyList.Count - 1;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot obtain degrees of freedom", e);
            }
        }

        public bool Rejected(double TValue, int DegreesOfFreedom, SignificanceLevel Significance)
        {
            //determine whether the null hypothesis of no difference between means
            //can be rejected; if the calculated t-test value is greater than the t-test
            //value in the table for a given degree of freedom, then hypothesis is rejected
            string sSignificance;
            DataTable dtValue;
            double dCriticalValue;

            try
            {
                sSignificance = Common.Significance(Significance);

                //first, get the critical value out of the table
                dtValue = Lookup.TTestTable();
                if (DegreesOfFreedom > 100 && DegreesOfFreedom < 1000)
                {
                    DegreesOfFreedom = 1000;
                }
                else if (DegreesOfFreedom >= 1000)
                {
                    DegreesOfFreedom = 99999;
                }
                dCriticalValue = 0;
                foreach (DataRow r in dtValue.Rows)
                {
                    if ((int)r[0] == DegreesOfFreedom)
                    {
                        dCriticalValue = (double)r[sSignificance];
                        break;
                    }
                }
                return Math.Abs(TValue) > dCriticalValue;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Cannot calculate One sample T-Test rejected value", e);
            }
        }

    }
}
