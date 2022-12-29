using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Xml;

public class AssistingServices
{
    // A method will use available RESTful service at https://graphical.weather.gov/
    // to find latitude and longitude values for a given zipCode.
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

    //A method to retrieve data historical climate data for a past year
    // at the https://api.meteostat.net/v2/ free non-commercial RESTful API.
    public static JObject getAnnualClimate(double lat,double lon) {
        // dataStringJSON
        string dataStringJSON = "";
        
        //Finding current date and past year date.
        DateTime currentDate = DateTime.Now;
        DateTime pastYear = currentDate.AddYears(-1);

        //Modifying the format to use in RESTful query.
        string currentDateString= currentDate.ToString("yyyy-MM-dd");
        string pastYearString = pastYear.ToString("yyyy-MM-dd");

        using (WebClient client = new WebClient())
        {
            string urlString = "https://api.meteostat.net/v2/point/daily";

            string parametersString = "?lat="+lat+"&lon="+lon+"&start="+pastYearString+"&end="+currentDateString;              

            // Used to access the api , this api key is recieved by registration at the service.
            client.Headers.Add("x-api-key",
                        "gLRo4OhyQGG93XRT5J9zneNwvztg6sUr");

            try { dataStringJSON = client.DownloadString(urlString+parametersString); }
            catch (WebException) { return null; }
        }

        //Parsing the retrieved data and returning this data.

        JObject climateDataJSON = JObject.Parse(dataStringJSON);

        return climateDataJSON;
    }

}