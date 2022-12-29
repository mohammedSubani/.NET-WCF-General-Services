using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

[ServiceContract]
public interface IService
{
	// Defining the service interface.
	// The service will recieve string zipCode as an input and will return
	// data of annual wind speed and annual wind energy as a string.
	[OperationContract]
	string AnnualWindData(string zipCode);
}


