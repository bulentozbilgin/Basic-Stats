
/*********************************************************************
 *  
 * Copyright 2010 B. Bulent Ozbilgin
 * This program is distributed under the terms of the GNU Lesser General Public License (Lesser GPL)
 *********************************************************************
 *
 */ 


public enum ArrayType
{
    Numeric,
    DateTime,
    String
}

//this class is a generic class for returning an object with ID and value attributes, and can be used as a return type
public class ReturnRow
{
    public int ID { get; set; }
    public string value { get; set; }
}

//for cut-off points, take the generic class as the starter, and add one more value to it, which will be the value of the cut-off point
public class CutoffPoint : ReturnRow
{
    public string PointValue { get; set; }
}