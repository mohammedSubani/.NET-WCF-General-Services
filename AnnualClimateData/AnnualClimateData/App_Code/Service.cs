/**
Project: BEMS IoT Enabled System V 1.0.0
File Name: Service.cs
Development Environment: Microsoft Visual Studio v16.8.4 Community Version
Code contributors: Mohammed Ahmed Subani
ASU ID: 1218278997

File Description:
 
The following service was originally developed in CSE598: Distributed Software Development and 
is modified to meet the functional requirments for the BEMS IoT Enbaled system.

Old functions: GetAnnualClimateData, GetAvailableParameters
New functions: GetPointAnnualClimateData, GetPointMonthlyClimateData
Modified functions: None
*/


using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Web;

public class Service : IService
{
	/**
	  Service implementation will be using API provided by NASA at
	  https://power.larc.nasa.gov/docs/services/api/
	  the API provide monthly averages data for many climate factors
	  a dictionary with the needed ones are located in AssistingServices class
	  this dictonary will be used to pass the climate factor parameter to
	  the API and recieve the data in JSON format , the data are treated to 
	  to get the annual averages.
	  
	  @parameters: string zipCode,string param
	  @returns: string with the annual average for a US zipCode
	*/
	
	public string GetAnnualClimateData(string zipCode,string param)
	{
		// Clear any white spaces entered as input.
		zipCode = zipCode.Replace(" ", "");
		param = param.Replace(" ", "");

		// Validated connection using Validations class
		if(!Validations.checkInternetConnection())
						return "There is not internet connection !!";

		// Validate zipCode format.
		string ValMsg= Validations.checkZipCode(zipCode);
		if (ValMsg != "valid")
						return ValMsg;

		// Validate that the parameter passed to the service exists in the dictionary.
		if (!AssistingServices.parameters.ContainsKey(param))
						return "The parameter is not valid , please check its syntax !";

		// Finding the zipCode location if the zipCode doesn't belong 
		// to USA the service will return "," as a string this is caught
		// by findZipCodeLatLon() if location[1] is null then this zipCode
		// doesn't belong to USA and location[0] has the notifying message.
		string[] location = AssistingServices.findZipCodeLatLon(zipCode);
		if (location[1] == null)
			return location[0];

		//Sucessfull latitude,longitude retrieval.
		double Lat = Double.Parse(location[0]);
		double Lon = Double.Parse(location[1]);

		// Retrieving data using getClimateData in the AssistingServices class.
		JObject climateData = AssistingServices.getClimateData(param,Lat,Lon);

		// Guard against failure to retrieve the data.
		if (climateData == null)
			return "Error in downloading data , check your internet connection !";

		// The following part will find the tokens needed to calcluate the 
		//  annual average for the parameter those tokens are the monthly 
		//  averages for number of years.

		JToken data = climateData["features"][0]["properties"]["parameter"][param];

		// Read the data and iterate for 12 months adding total readings and 
		//  dividing by 12 to find the annual average value.
		double totalReadings = 0;

		for (int i = 1; i <= 12; i++)
		{
			try { totalReadings += Double.Parse(data["" + i].ToString()); }
			catch (FormatException) { return "Bad data format !"; }
			catch (ArgumentNullException) { return "Empty data fields !"; }
		}

		double AnnualAverage = totalReadings / 12;

		return "The annual average data for the parameter : "
				+ AssistingServices.parameters[param]
				+" is "+ Math.Round(AnnualAverage,2);
	}

	// This method is used to find the available date parameters and their names
	// in the service , as the dictionary gets updated by service provider the
	// the service user will have more options of climate facotrs.
	public string GetAvailableParameters() {
		string result = "";
		foreach (var kvp in AssistingServices.parameters)
			result += kvp.Key + " : " + kvp.Value + "\n";
		return result;
	}
	
	/**
	  Function description:
	  
	  Service implementation will be using API provided by NASA at
	  https://power.larc.nasa.gov/docs/services/api/
	  
	  This function uses latitude and longitude to get the monthly averages data 
	  for many climate factors a dictionary with the needed ones are located in 
	  AssistingServices class this dictonary will be used to pass the climate
	  factor parameter to the API and recieve the data in JSON format, the data
	  are treated to get the annual averages.
	  
	  @parameters: double lat, double lon, string param
	  @returns: string with the annual average for the parameter
	*/
	
	public string GetPointAnnualClimateData(double lat, double lon, string param) 
	{

		// Validated connection using Validations class
		if (!Validations.checkInternetConnection())
			return "There is not internet connection !!";

		// Validate that the parameter passed to the service exists in the dictionary.
		if (!AssistingServices.parameters.ContainsKey(param))
			return "The parameter is not valid , please check its syntax !";


		// Retrieving data using getClimateData in the AssistingServices class.
		JObject climateData = AssistingServices.getClimateData(param, lat, lon);

		// Guard against failure to retrieve the data.
		if (climateData == null)
			return "Error in downloading data ! , check syntax and internet connection ...";

		// The following part will find the tokens needed to calcluate the 
		//  annual average for the parameter those tokens are the monthly 
		//  averages for number of years.

		JToken data = climateData["properties"]["parameter"][param];

		// Read the data and iterate for 12 months adding total readings and 
		//  dividing by 12 to find the annual average value.
		double totalReadings = 0;
		
		// Months array to iterate over in the JObject to get each month value
		string[] months = {"JAN","FEB","MAR","APR",
						   "MAY","JUN","JUL","AUG",
						   "SEP","OCT","NOV","DEC"};
		
		// Iterating over the months to get the values for each month
		for (int i = 0; i < 12; i++)
		{
			try { totalReadings += Double.Parse(data[months[i]].ToString()); }
			catch (FormatException) { return "Bad data format !"; }
			catch (ArgumentNullException) { return "Empty data fields !"; }
		}
		
		// Return the annual average for the reading
		double AnnualAverage = totalReadings / 12;

		return "The annual average data for the parameter : "
				+ AssistingServices.parameters[param]
				+ " is " + Math.Round(AnnualAverage, 2);
	}

	/**
	  Function description:
	  
	  Service implementation will be using API provided by NASA at
	  https://power.larc.nasa.gov/docs/services/api/
	  
	  This function uses latitude and longitude to get the monthly averages data 
	  for many climate factors a dictionary with the needed ones are located in 
	  AssistingServices class this dictonary will be used to pass the climate
	  factor parameter to the API and recieve the data in JSON format, the data
	  are treated to get the annual averages.
	  
	  @parameters: double lat, double lon, string param
	  @returns: string with the monthly	averages for the parameter 
	*/
	public string GetPointMonthlyClimateData(double lat, double lon, string param) 
	{
		// Result is a string of JSON array which will be read by Cyclotron component
		// to visualize the data.
		string result = "[";

		// Validated connection using Validations class
		if (!Validations.checkInternetConnection())
			return "There is not internet connection !!";

		// Validate that the parameter passed to the service exists in the dictionary.
		if (!AssistingServices.parameters.ContainsKey(param))
			return "The parameter is not valid , please check its syntax !";


		// Retrieving data using getClimateData in the AssistingServices class.
		JObject climateData = AssistingServices.getClimateData(param, lat, lon);

		// Guard against failure to retrieve the data.
		if (climateData == null)
			return "Error in downloading data ! , check syntax and internet connection ...";

		// The following part will find the tokens needed to calcluate the 
		//  annual average for the parameter those tokens are the monthly 
		//  averages for number of years.

		JToken data = climateData["properties"]["parameter"][param];

		// Read the data and iterate for 12 months adding total readings and 
		//  dividing by 12 to find the annual average value.

		string[] months = {"JAN","FEB","MAR","APR",
						   "MAY","JUN","JUL","AUG",
						   "SEP","OCT","NOV","DEC"};
		
		// Iteration over monthly averages and saving results to JSON array result string
		for (int i = 0; i < 12; i++)
		{
			try 
			{
				result += "{";

				result += "\n	\"Month\": \"" + months[i] + "\"," +
						  "\n	\"date\":" + "\"0000-"+ String.Format("{0:00}", i+1) +"-1T00:00:00\"," +
						  "\n	\"value\":" + data[months[i]];

				result += "}";

				if (i != 11)
					result += ",";

				if (i == 11)
					result += "\n]";
			}
			catch (FormatException) { return "Bad data format !"; } // Guard against format change
			catch (ArgumentNullException) { return "Empty data fields !"; } // Guard against null values
		}

		

		return result; // return the results.
	}
}
