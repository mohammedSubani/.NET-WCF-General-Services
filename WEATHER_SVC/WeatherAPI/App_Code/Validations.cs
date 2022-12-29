/**
Project: BEMS IoT Enabled System V 1.0.0
File Name: Validations.cs
Development Environment: Microsoft Visual Studio v16.8.4 Community Version
Code contributors: Mohammed Ahmed Subani
ASU ID: 1218278997

File Description:
 
The following file was originally developed in CSE598: Distributed Software Development and 
is modified to meet the functional requirments for the BEMS IoT Enbaled system.

Old functions: checkInternetConnection, checkLocationRange
New functions: None
Modified functions: None
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;

// A class to preform validations before making any calls to any other remote parts.
public class Validations
{
	
	/**
	  Function Description:
		A method to check internet conncetion.
		Will try to connect to https://www.google.com/.
			
	  @parameters: void
	  @returns: boolean flag for the internet conncetion state
	*/
	
    public static bool checkInternetConnection()
    {
        try
        {
            using (var client = new WebClient()) 
            using (client.OpenRead("https://www.google.com/"))
                return true;
        }
        catch { return false; }
    }

	/**
	  Function Description:
	  
		    A method for checking the location range
			The range for latitude is : [-90,90]
			The range for longitude is : [-180,180] 
		
	  @parameters: double lat, double lon
	  @returns: boolean flag for location range state 
	*/

    public static bool checkLocationRange(double lat, double lon)
    {
        if (lat < -90 || lat > 90)
            return false;
        if (lon < -180 || lon > 180)
            return false;
        return true;
    }
}