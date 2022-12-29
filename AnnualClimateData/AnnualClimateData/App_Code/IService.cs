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
	// Operation for retrieving the annual climate data.
	[OperationContract]
	[WebGet(UriTemplate = "GetAnnualClimateData?zipCode={zipCode}&Parameter={param}",ResponseFormat = WebMessageFormat.Json)]
	string GetAnnualClimateData(string zipCode,string param);
	// This operation will return the available parameters the service can provide
	//  as the dictionary gets updated more options will be available.
	[OperationContract]
	[WebGet(UriTemplate = "GetAvailableParameters", ResponseFormat = WebMessageFormat.Json)]
	string GetAvailableParameters();
	
	// Retrieve annual average for a parameter
	[OperationContract]
	[WebGet(UriTemplate = "GetPointAnnualClimateData?latitude={lat}&longitude={lon}&parameter={param}", ResponseFormat = WebMessageFormat.Json)]
	string GetPointAnnualClimateData(double lat,double lon,string param);
	
	// Retrieve monthly averages for a parameter
	[OperationContract]
	[WebGet(UriTemplate = "GetPointMonthlyClimateData?latitude={lat}&longitude={lon}&parameter={param}", ResponseFormat = WebMessageFormat.Json)]
	string GetPointMonthlyClimateData(double lat, double lon, string param);

}


