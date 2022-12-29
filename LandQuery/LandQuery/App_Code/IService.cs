using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

[ServiceContract]
public interface IService
{
	// Main service interface will recivev zipCode and return vacant land data as
	// in the following order (Address , Latitude , Longitude)
	[OperationContract]
	string GetVacantLand(string zipCode);
}

