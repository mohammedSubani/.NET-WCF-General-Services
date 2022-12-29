using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Activities.Statements;

public class Service : IService
{
    //The service implementation.
    //This service uses two classes AssistingServices and Verfication classes.

    // AssistingServices will be used to find location of a zipCode by wrapping 
    // the location service provided at https://graphical.weather.gov/.

    // AssistingServices will be used to find climate data for a latitude of and 
    // longitude found for the past year the service wrappes a free non-commerical 
    // API for historical climate data at https://api.meteostat.net/v2.

    //Verification class will be used to verify connectivity by checkInternetConnection()
    // and verifying the zipCode by checkZipCode().
    public string AnnualWindData(string zipCode)
	{  
        // Latitude , Longitude data fields.
        double currentLat;
        double currentLon;

        // Checking internet connection.
        if (!Verification.checkInternetConnection())
            return "There is no internet connection !";

        // Removing any white spaces and checking the zipCode validity.
        //  if it is invalid return why it is invalid this is found by checkZipCode.
        zipCode = zipCode.Replace(" ", "");
        string checkStr = Verification.checkZipCode(zipCode);
        if (checkStr != "valid")
            return checkStr;

        // Finding the zipCode location if the zipCode doesn't belong 
        // to USA the service will return "," as a string this is caught
        // by findZipCodeLatLon() if location[1] is null then this zipCode
        // doesn't belong to USA and location[0] has the notifying message.
        string[] location = AssistingServices.findZipCodeLatLon(zipCode);
        if (location[1] == null)
            return location[0];

        //Sucessfull latitude,longitude retrieval.
        currentLat = Double.Parse(location[0]);
        currentLon = Double.Parse(location[1]);

        // Using AssistingServices to retrieve data from meteostat API and processing the
        // given data to find the annual wind speed and annual wind energy.

        JObject climateData = AssistingServices.getAnnualClimate(currentLat, currentLon);

        //Checking the validity of data for processing.
        if (climateData == null)
            return "Error in downloading data , check your internet connection !";
        if (climateData["data"] == null)
            return "there is no available data for the given inputs !";

        double windSpeedSum = 0;
        int dataCount = 0;

        // Iterating over the data to calculate the annual wind speed and
        // annual wind energy , the data is provided for a daily-basis
        // thus counting days to find the average.

        // If the data is not provided the data source provied either null
        // or spaces thus skip any unsucessful parsing and count only sucessful ones.
        foreach (var resultData in climateData["data"]) {
            string windSpeed = resultData["wspd"].ToString();

            try { windSpeedSum += Double.Parse(windSpeed); }
            catch (FormatException) { continue;}
            catch (ArgumentNullException) { continue; }

            dataCount++;
        }

        //Finding annual wind speed.
        double AnnualWindSpeed = windSpeedSum / dataCount;

        //At 0 °C for 1 m³ the air mass is 1.293 
        // Formula : Wind Energy = 0.5 * airMass * Velocity * Velocity. 
        double airSampleMass = 1.293;
        double AnnualWindEnergy = 0.5 * airSampleMass * Math.Pow(AnnualWindSpeed,2);

        //returning final result.
        return " " + Math.Round(AnnualWindSpeed,2)+ " km/h    "+Math.Round(AnnualWindEnergy,2) + " kg.(km/h)² ";
	}
}
