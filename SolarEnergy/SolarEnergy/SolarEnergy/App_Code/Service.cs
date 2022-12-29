/**
Project: BEMS IoT Enabled System V 1.0.0
File Name: Service.cs
Development Environment: Microsoft Visual Studio v16.8.4 Community Version
Code contributors: Mohammed Ahmed Subani
ASU ID: 1218278997

File Description:
 
The following service was originally developed in CSE598: Distributed Software Development and 
is used to meet the functional requirments for the BEMS IoT Enbaled system.

Old functions: SolarIntensity, checkLocationRange, checkInternetConnection
New functions: None
Modified functions: None
*/

using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Net;
using System.IO;


public class Service : IService
{
    /**
	  Function Description:
		  A method to check internet conncetion.
		  Will try to connect to https://www.google.com/. 
		
	  @parameters: void
	  @returns: boolean flag for connection state
	*/

    public bool checkInternetConnection() {
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
	  @returns: boolean flag
	*/
	
    public bool checkLocationRange(double lat,double lon)
	{
        if (lat < -90 || lat > 90)
            return false;
        if (lon < -180 || lon > 180)
            return false;
        return true;
	}

  
	
	
	/**
	  Function Description:
			Service interface implementation the service will check the range of the 
			latitude and longitude before implementing the service if the values are
			out of range it will return a string notifying the user for the bad input.

			The data provided are monthly SolarIntensity values the service will read the data
			find the summation of the data and divide by the number of months to find
			the annual solar intensity.
		
	  @parameters: double lat, double lon
	  @returns: a string representing either the result or sending an error message
	*/
    public string SolarIntensity(double lat, double lon) {

        //Checking internet connection.
        if (!checkInternetConnection())
            return "There is no internet connection";

        //Checking range.
        if (!checkLocationRange(lat, lon))
            return "lat,lon values out of range (lat : [-90,90], lon : [-180,180])";

        //The data provided by the website contains extra lines in the beggining and ending 
        // that needs to to be skipped in order to read the data.

        const int EXTRA_LINES = 8;          
        const int FIRST_EXTRA_LINE = 5;

        //The data recieved are in html format.

        string htmlDataString=""; // A string to hold the data from the source

        //Creating connection and trying to download the data.
        // if the data downloaded refers to a data at sea which it
        // doesn't have available data a WebException is thrown
        // the service user will be notified for this result.

        using (WebClient client = new WebClient())
        {
            string urlString = "https://re.jrc.ec.europa.eu/api/mrcalc?lat=" 
                + lat + "&lon=" + lon + "&horirrad=1";
            try
            {
                htmlDataString = client.DownloadString(urlString);
            }
            catch (WebException) {
                return "The location is a sea location , no available data";
            }
        }

        // After downloading the data sucessfully reading the string and 
        //  taking solar intensity data is done in this part.

        using (StringReader reader = new StringReader(htmlDataString))
        {
            string readTextLine = "";
            int monthsCounter = 0;
            double solarIntenSum = 0;
            double annualAverage;
            double inten;

            foreach (char c in htmlDataString)
                if (c == '\n') monthsCounter++;         //Counting number of lines.

            monthsCounter = monthsCounter - EXTRA_LINES;//finding number of months.

            for (int i = 0; i < FIRST_EXTRA_LINE; i++) //Skipping first extra lines
                readTextLine = reader.ReadLine();

            for (int i = 0; i < monthsCounter; i++) // reading months lines.
            {
                readTextLine = reader.ReadLine();

                // splitting the lines by their tokens to find the data.
                string[] tokenized = readTextLine.Split(new string[] { "		" }
                                                                    , StringSplitOptions.None);
                //  Parsing the data for the monthly solar intensity
                //   in case of error throw FormatException/ArgumentNullException
                try
                {
                    inten = Double.Parse(tokenized[tokenized.Length - 1]);
                }
                catch (ArgumentNullException e)
                {
                    return "Trying to parse a null value ," +
                        " error message : " + e.Message;
                }
                catch (FormatException e) {
                    return "Trying to parse non-numerice values ," +
                        " error message : "+e.Message;
                }
                solarIntenSum = solarIntenSum + inten;
            }
            // Finding annual solar intensity.

            annualAverage = solarIntenSum / monthsCounter;
            annualAverage = Math.Round(annualAverage, 3);
            return annualAverage.ToString();
        }

    }
}		
