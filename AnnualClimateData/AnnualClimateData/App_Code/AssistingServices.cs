/**
Project: BEMS IoT Enabled System V 1.0.0
File Name: AssistingServices.cs
Development Environment: Microsoft Visual Studio v16.8.4 Community Version
Code contributors: Mohammed Ahmed Subani
ASU ID: 1218278997

File Description:
 
The following file was originally developed in CSE598: Distributed Software Development and 
is modified to meet the functional requirments for the BEMS IoT Enbaled system.

Old functions: findZipCodeLatLon, getClimateData
New functions: None
Modified functions: None
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Xml;

// The static methods in this class  will be assisting the service as follows
//  Static dictionary with available data in the service will retrieve 
//  those are added manually by service provider.

// Static method getClimateData will make the actual call for the POWER NASA API
// and retrieve the data as JSON object.

// Static method findZipCodeLatLon will retrieve the latitude and longitude for a US 
// postal code.
public class AssistingServices
{
    //  Static dictionary with available climate data.
    public static Dictionary<string, string> parameters = new Dictionary<string, string>()
    {

        {"RH2M", "Relative humidity at 2 meters"},
        {"CLD_AMT", "Day cloud amount in percentage %"},
        {"T2M", "Temperature at 2 Meters" },
        {"WS10M","Wind Speed at 10 Meters" },
        {"WD10M", "Wind Direction at 10 Meters"},
        {"SLP", "Sea Level Pressure"}
    };

	/**
	  Function Description:
		A method will use available RESTful service at https://graphical.weather.gov/
		to find latitude and longitude values for a given zipCode.
			
	  @parameters: string zipCode 
	  @returns: string[] with latitude and longitude values
	*/
    public static string[] findZipCodeLatLon(string zipCode)
    {
        //Data field to hold latitude , longitude values.
        string[] latLon = new string[2];

        //XmlDocument for the recieved data from source.
        XmlDocument latLonDoc = new XmlDocument();

        using (WebClient client = new WebClient())
        {
            string urlString = "https://graphical.weather.gov/xml/sample_products/" +
                "browser_interface/ndfdXMLclient.php?listZipCodeList=" + zipCode;

            //Used to allow access to the API at the website.
            client.Headers.Add("User-Agent",
                        "MyApplication/v1.0 (http://foo.bar.baz; foo@bar.baz)");

            try { latLonDoc.LoadXml(client.DownloadString(urlString)); }

            catch (Exception)
            {
                latLon[0] = "Error happended while downlading url,check your internet connection.";
                return latLon;
            }
        }
        // Finding XML element for latitude,longitude.
        XmlNodeList elemList = latLonDoc.GetElementsByTagName("latLonList");

        // If the element is empty this zipCode doesn't belong to US notify the user.
        if (elemList[0].InnerXml == ",")
        {
            latLon[0] = "This service works only for US, Please enter a valid zip code.";
            return latLon;
        }
        // Sucessful lat,lon retrieval record the values.
        latLon = elemList[0].InnerXml.Split(',');

        return latLon;
    }

	/**
	  Function Description:
		The method will call the API using the type of parameter , latitude and longitude.
			
	  @parameters: string param,double lat, double lon 
	  @returns: JObject with climate data retrieved from NASA API
	*/
    public static JObject getClimateData(string param,double lat, double lon)
    {
        // dataStringJSON
        string dataStringJSON = "";

        // Creating web client for calling the service as RESTful call 
        using (WebClient client = new WebClient())
        {
            // The access point for the API.
            string urlString = "https://power.larc.nasa.gov/api/temporal/climatology/point";

            // The list of paramters passed to the API.
            string parametersString =
                "?start=2001&end=2020&latitude="+ lat +"&longitude="+ lon
                + "&community=ag&parameters="+ param +"&format=json&"
                + "user=ANON&header=true";

            // Downloading and parsing the data string into JSON object.
            try { dataStringJSON = client.DownloadString(urlString + parametersString); }
            catch (WebException) { return null; }
        }

        //Parsing the retrieved data and returning this data.

        JObject climateDataJSON = JObject.Parse(dataStringJSON);

        return climateDataJSON;
    }

}