/**
Project: BEMS IoT Enabled System V 1.0.0
File Name: Service.cs
Development Environment: Microsoft Visual Studio v16.8.4 Community Version
Code contributors: Mohammed Ahmed Subani
ASU ID: 1218278997

File Description:
 
The following service was originally developed in CSE598: Distributed Software Development and 
is modified to meet the functional requirments for the BEMS IoT Enbaled system.

Old functions: findZipCodeLatLon
New functions: findElevation
Modified functions: None
*/

using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Xml;
using System.Net;

public class Service : IService
{	
	/**
	  Function Description:
		A method will use available RESTful service at https://graphical.weather.gov/
		to find latitude and longitude values for a given zipCode.
		
	  @parameters: string zipCode
	  @returns: string[] with latitude and longitude at array elements 0 and 1
	*/
    public string[] findZipCodeLatLon(string zipCode)
    {
        //Data field to hold latitude , longitude values.
        string[] latLon = new string[2];
        XmlDocument latLonDoc = new XmlDocument();

        using (WebClient client = new WebClient())
        {
            string urlString = "https://graphical.weather.gov/xml/sample_products/" +
                "browser_interface/ndfdXMLclient.php?listZipCodeList=" + zipCode;

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
		This service will used methods at class Verification to verify 
		zipCode, latitude , longitude values and to check connectivity.

		This service will wrap a RESTful service at https://api.opentopodata.org/
		which will return the elevation for a location the service allow to enter US zipCode
		and retrieve its location and is aimed to be used only for US zipCodes any other zipCodes
		won't work for this purpose.However , by providing latitude and longitude the service will
		provide elevation for any point globally..
		
	  @parameters: double lat,double lon
	  @returns: string with the elevation for the assoicated latitude and longitude
	*/

    public string findElevation(double lat,double lon)
    {
        //Checking latitude longitude range.
        if (!Verification.checkLocationRange(lat, lon))
            return "lat,lon values out of range (lat : [-90,90], lon : [-180,180])";

        // Checking internet connection.
        if (!Verification.checkInternetConnection())
            return "There is no internet connection !";

        // Using service free API at https://api.opentopodata.org/ to retrieve 
        // the elevation of a location described by latitude and longitude.
        string jsonString;
        using (WebClient client = new WebClient())
        {
            string urlString = "https://api.opentopodata.org/v1/test-dataset?locations="
                                + lat + "," + lon;

            try { jsonString = client.DownloadString(urlString); }
            catch (WebException) { return "Error in downloading data !"; }
        }

        JObject dataJSON = JObject.Parse(jsonString);

        string elevation = dataJSON["results"][0]["elevation"].ToString();

        //Last safety valve to make sure that the results retrieved are not empty
        // if they are empty the user will be notified for the erro.
        if (dataJSON["results"] == null || dataJSON["results"][0] == null ||
                                                    dataJSON["results"][0]["elevation"]==null)
            return "Error in data retrieving  !";

        double eleNum = Double.Parse(elevation);
        eleNum = Math.Round(eleNum,3);

        return eleNum+"";
	}

}
