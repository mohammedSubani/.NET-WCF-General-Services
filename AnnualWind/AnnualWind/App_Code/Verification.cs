using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

public class Verification
{
    //A method to check internet conncetion.
    // Will try to connect to https://www.google.com/.
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
    // A method for checking the zipCode validity.
    // Handles cases where zipCode has invalid format or cases of null 
    // values or cases of extra or less digits provided by the user.
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
}