/**
Project: BEMS IoT Enabled System V 1.0.0
File Name: Validations.cs
Development Environment: Microsoft Visual Studio v16.8.4 Community Version
Code contributors: Mohammed Ahmed Subani
ASU ID: 1218278997

File Description:
 
The following file was originally developed in CSE598: Distributed Software Development and 
is modified to meet the functional requirments for the BEMS IoT Enbaled system.

Old functions: checkInternetConnection, checkZipCode
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
	  @returns: boolean flag 
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
		A method for checking the zipCode validity. 
		Handles cases where zipCode has invalid format or cases of null 
		values or cases of extra or less digits provided by the user.
		
	  @parameters: void
	  @returns: boolean flag 
	*/
	
    public static string checkZipCode(string zipCode)
    {
        try { Double.Parse(zipCode); } // parse zipCode
        catch (FormatException) // Check its format
        {
            return "zip code does not represent" +
                   " a numeric value.";
        }

        catch (ArgumentNullException) { return "zip code is null"; } // guaard for null

        if (zipCode.Length != 5) // check for zipCode length
            return "zip code number of digits doesn't equal 5";

        return "valid";
    }

}