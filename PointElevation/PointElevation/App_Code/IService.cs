using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

[ServiceContract]
public interface IService
{
	
	// Recieves a point latitude and longitude and returns that point 
	// elevation above sea level
	[OperationContract]
	[WebGet(UriTemplate="findElevation?latitude={lat}&longitude={lon}")]
	string findElevation(double lat,double lon);

	// Recieves a US zipCode gets the latitude and longitude for that zipCode 
	// and returns that point elevation above sea level 
	[OperationContract]
	[WebGet(UriTemplate = "findZipCodeLatLon?zipCode={zipCode}")]
	string[] findZipCodeLatLon(string zipCode);

}
