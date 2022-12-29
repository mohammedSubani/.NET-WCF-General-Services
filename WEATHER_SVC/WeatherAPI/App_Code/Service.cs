/**
Project: BEMS IoT Enabled System V 1.0.0
File Name: Service.cs
Development Environment: Microsoft Visual Studio v16.8.4 Community Version
Code contributors: Mohammed Ahmed Subani
ASU ID: 1218278997

File Description:
 
The following service implements the calling to a weather data provider and return
the current weather data for a location in JSON format process the data and make it 
available for the service interface to be used in synchronous calls.

Old functions: None
New functions: GetWeatherJSON, GetCondition, GetTemp, GetHum, GetWndSpeed, GetWndDir, GetIcon
			   GetCloudAmnt, SucessRetrieval
Modified functions: None
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

public class Service : IService
{
    static bool IsSucessRetrieval = false;   // Flag data retrieval condition
    static string dataStringJSON = "";       // returned string of current weather data
    static JObject WeatherDataJSON; 		 // JObject parsed from the returned dataStringJSON
	
	
	
	
	/**
	  Function Description:
		  A method that will call the weather data provider (Openweather API),
		  the retrieved data will be stored in dataStringJSON and parse the 
		  result into JObject WeatherDataJSON.
		  
	  @parameters: double latitude, double longitude
	  @returns: boolean flag for data retrieval state
	*/
    public bool GetWeatherJSON(double latitude, double longitude) 
	{
		// Validating the internet connection. 
		if (!Validations.checkInternetConnection())
			return false;
	
		// Validating the location range.
		if (!Validations.checkLocationRange(latitude, longitude))
			return false;

        // Creating web client for calling the service as RESTful call 
        using (WebClient client = new WebClient())
        {
            // The access point for the API.
            string urlString = "http://api.weatherapi.com/v1/current.json";

            // The list of paramters passed to the API.
            string parametersString =
                "?key=957b914013734eabb4d184313220603" + // API Key
                "&q=" + latitude + ", " + longitude +
                "&aqi=no";


            // Downloading and parsing the data string into JSON object.
            try { dataStringJSON = client.DownloadString(urlString + parametersString); }
            catch (WebException) { return false; } // Failing call to the service
        }

        try { WeatherDataJSON = JObject.Parse(dataStringJSON); }
        catch (Exception) { return false; } // Failing to parse the JSON data

	/* Setting the retrieval state at this point the call and parsing the data 
	 are sucessful */
	 
        IsSucessRetrieval = true; 

        return true; 
	}
	
	/**
	  Function Description:
		  This function returns the current condition of the weather e.g.: cloudy,rainy ...etc 
		  using the parsed WeatherDataJSON JObject.
		  
	  @parameters: void
	  @returns: string with current weather condition
	*/
	
	public string GetCondition() 
    {
		/*Check for the state of data retrieval*/
        if (!IsSucessRetrieval)
            return "No available data !";

		/*Parse the data into JToken to get the data*/
        JToken WeatherCondition = WeatherDataJSON["current"]["condition"]["text"];
		
		/*Return the current weather data in string format*/
        return WeatherCondition.ToString(); 
    }

	/**
	  Function Description:
		  This function returns the current temperature of the weather in centigrades
		  using the parsed WeatherDataJSON JObject.
		  
	  @parameters: void
	  @returns: string with current temperature
	*/
	public string GetTemp()
    {
		/*Check for the state of data retrieval*/
        if (!IsSucessRetrieval)
            return "No available data !";
		
		/*Parse the data into JToken to get the data*/
        JToken Temp = WeatherDataJSON["current"]["temp_c"];
		
		/*Return the current temperature data in string format*/
        return Temp.ToString();
    }
	
	
	/**
	  Function Description:
		  This function returns the current humidity of the weather in 100% scale
		  using the parsed WeatherDataJSON JObject.
		  
	  @parameters: void
	  @returns: string with current relative humidity
	*/
    public string GetHum()
    {
		/*Check for the state of data retrieval*/
        if (!IsSucessRetrieval)
            return "No available data !";

		/*Parse the data into JToken to get the data*/
        JToken Hum = WeatherDataJSON["current"]["humidity"];
		
		/*Return the current humidity data in string format*/
        return Hum.ToString();
    }

	/**
	  Function Description:
		  This function returns the current wind speed of the weather (km/h)
		  using the parsed WeatherDataJSON JObject.
		  
	  @parameters: void
	  @returns: string with current wind speed
	*/
	
	public string GetWndSpeed()
    {
		/*Check for the state of data retrieval*/
        if (!IsSucessRetrieval)
            return "No available data !";
		
		/*Parse the data into JToken to get the data*/
        JToken WndSpeed = WeatherDataJSON["current"]["wind_kph"];

		/*Return the current wind speed data in string format*/
        return WndSpeed.ToString();
    }

	/**
	  Function Description:
		  This function returns the current wind direction of the weather (in degrees)
		  using the parsed WeatherDataJSON JObject.
		  
	  @parameters: void
	  @returns: string with current wind direction
	*/
	
	public string GetWndDir()
    {
		/*Check for the state of data retrieval*/
        if (!IsSucessRetrieval)
            return "No available data !";

		/*Parse the data into JToken to get the data*/
        JToken WndDir = WeatherDataJSON["current"]["wind_dir"];
		
		/*Return the current wind direction data in string format*/
        return WndDir.ToString();
    }


	/**
	  Function Description:
		  This function returns the current weather icon representation
		  using the parsed WeatherDataJSON JObject.
		  
	  @parameters: void
	  @returns: string with URL for current weather state
	*/
	
	public string GetIcon()
    {
		/*Check for the state of data retrieval*/
        if (!IsSucessRetrieval)
            return "No available data !";

		/*Parse the data into JToken to get the data*/
        JToken WeatherIcon = WeatherDataJSON["current"]["condition"]["icon"];

		/*Return the current condition icon URL in string format*/
        return WeatherIcon.ToString();
    }

	
	/**
	  Function Description:
		  This function returns the current cloud amounts in 100% scale
		  using the parsed WeatherDataJSON JObject.
		  
	  @parameters: void
	  @returns: string with URL for current weather state
	*/
	
    public string GetCloudAmnt()
    {
		/*Check for the state of data retrieval*/
        if (!IsSucessRetrieval)
            return "No available data !";
	
		/*Parse the data into JToken to get the data*/
        JToken cloudAmnt = WeatherDataJSON["current"]["cloud"];
		
		/*Return the current condition icon URL in string format*/
        return cloudAmnt.ToString();
    }


	/**
	  Function Description:
		  This function returns the state of data retrieval, getter function for IsSucessRetrieval.
		  
	  @parameters: void
	  @returns: boolean flag for the retrieval state
	*/
    public bool SucessRetrieval() {return IsSucessRetrieval;}

}
