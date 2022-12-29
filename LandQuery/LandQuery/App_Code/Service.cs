using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Xml;
using System.Net;
using System.Text;

public class Service : IService
{
	// The service interface implementation the service will recieve a US postal code
	// and will return the data of vacant lands available at that postal code.

	// This service will be using API at https://api.developer.attomdata.com/
	// the service will make a call for the API will retrieve an XML data and process
	// the data to find the address, latitude and longitude of vacant lands within that
	// postal code.
	public string GetVacantLand(string zipCode)
	{
		
		string result = "";

		//Clear any white spaces in the zipCode.
		zipCode = zipCode.Replace(" ", "");

		// Validate that the connection is working fine using class Validations method
		if (!Validations.checkInternetConnection())
			return "Please check your internet connection , No connection !";

		// Validate that zipCode format is correct using class Validations.
		string valMsg = Validations.checkZipCode(zipCode);
		if (valMsg != "valid")
			return valMsg;

		//Check if the postal code belongs to US postal code or not
		// if not return a message notifying the user.
		string[] location = AssistingServices.findZipCodeLatLon(zipCode);
		if (location[1] == null)
			return location[0];

		// Using the GetLandDict at AssistingServices class to retrieve 
		// the actual data and return the data to user.
		result = AssistingServices.GetLandDict(zipCode);

		// Guard against any faulty callbacks.
		if (result == null)
			return "Error retrieving results from API";
		if (result == "")
			return "Sorry no vacant lands are available !";

		return result;
	}

	

}
