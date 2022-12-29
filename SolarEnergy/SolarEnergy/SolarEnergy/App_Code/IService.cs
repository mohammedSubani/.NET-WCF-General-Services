using System.ServiceModel;
using System.ServiceModel.Web;


[ServiceContract]
public interface IService
{
	//Service interface the implementation for this service depends on the data
	//  provided from the following website https://re.jrc.ec.europa.eu/
	[OperationContract]
	[WebGet(UriTemplate = "GetSolarIndex?latitude={lat}&longitude={lon}", ResponseFormat = WebMessageFormat.Json)]
	string SolarIntensity(double lat, double lon); //Defining the service interface
}



