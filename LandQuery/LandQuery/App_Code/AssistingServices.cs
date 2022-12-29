using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Net;

public class AssistingServices
{
    //The acutal remote call to ATTOM lands data API this service will be recieving an 
    // XML document containing multiple data fields this method will also process the 
    // data fields needed for this service.
    public static string GetLandDict(string zipCode) {

        string result = "";

        XmlDocument vacantLandsDoc = new XmlDocument();

        // Creating web client to call the API.
        using (WebClient client = new WebClient())
        {
            // Using the API access point.
            string urlString = "https://api.gateway.attomdata.com/propertyapi/v1.0.0/property/address";
            
            // Using the needed paramters that are to be passed to the API.
            string parameters = "?postalcode=" + zipCode + "&propertytype=VACANT+LAND+(NEC)";

            // Choosing the returned data format and using the API Key 
            // provided by the ATTOM API Account.
            client.Headers.Add("Accept", "application/xml");
            client.Headers.Add("apikey", "0f6629ba75c3a88e8dadae79b0f182a7");

            // Downloading the data and loading as XML files
            try { vacantLandsDoc.LoadXml(client.DownloadString(urlString + parameters)); }
            
            // Guarding against any errors while downloading the data.
            catch (WebException) { return null; }


            // Getting the elements of data that are to be processed and returned to
            // the method caller.

            XmlNodeList elemAddress = vacantLandsDoc.GetElementsByTagName("oneLine");
            XmlNodeList elemLat = vacantLandsDoc.GetElementsByTagName("latitude");
            XmlNodeList elemLon = vacantLandsDoc.GetElementsByTagName("longitude");

            // Data fields to be retrieved.
            string address;
            double lat;
            double lon;

            // Adding the data fields to the results as lines for each
            // vacant land data.
            for (int i = 0; i < elemAddress.Count; i++)
            {
                address = elemAddress[i].InnerXml.ToString(); 
                lat = Convert.ToDouble(elemLat[i].InnerXml); 
                lon = Convert.ToDouble(elemLon[i].InnerXml);

                //Formatting result and adding them to results.
                string instance =address+", lat : "+lat+", lon : "+lon+"\n";
                result += instance;
            }

        }

        
        return result;

    }

    // This method will make a call for https://graphical.weather.gov/xml/ to find
    // the latitude and longitude for a US postal code if it is not found it will return
    // any XML element with only ',' character in it.
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
    

}