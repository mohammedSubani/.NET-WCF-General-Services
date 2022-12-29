/**
Project: BEMS IoT Enabled System V 1.0.0
File Name: Service.cs
Development Environment: Microsoft Visual Studio v16.8.4 Community Version
Code contributors: Mohammed Ahmed Subani
ASU ID: 1218278997

File Description:
 
The following service was originally developed in CSE598: Distributed Software Development and 
is used to meet the functional requirments for the BEMS IoT Enbaled system.

Old functions: checkZipCode, checkLocationRange, checkInternetConnection
New functions: None
Modified functions: None
*/

using System;
using System.Linq;
using System.Net;

public class Verification
{
	/**
	  Function Description:
		  A method for checking the zipCode validity.
		  Handles cases where zipCode has invalid format or cases of null 
		  values or cases of extra or less digits provided by the user.
		
	  @parameters: string zipCode
	  @returns: string with verification statement
	*/
    public static string checkZipCode(string zipCode)
    {
        try { Double.Parse(zipCode); }
        catch (FormatException)
        {
            return "zip code does not represent" +
                   " a numeric value.";
        }

        catch (ArgumentNullException) { return "zip code is null"; }

        if (zipCode.Length != 5)
            return "zip code number of digits doesn't equal 5";

        return "valid";
    }

	/**
	  Function Description:
		A method for checking the location range
		The range for latitude is : [-90,90]
		The range for longitude is : [-180,180] 
		
	  @parameters: double lat, double lon
	  @returns: boolean flag
	*/

    public static bool checkLocationRange(double lat, double lon)
    {
        if (lat < -90 || lat > 90)
            return false;
        if (lon < -180 || lon > 180)
            return false;
        return true;
    }
	
	/**
	  Function Description:
		  A method to check internet conncetion.
		  Will try to connect to https://www.google.com/. 
		
	  @parameters: void
	  @returns: boolean flag for connection state
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

}