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
	/* Function interface returning boolean flag of weather data retrieval 
	 and parsing states */
	[OperationContract]
	bool GetWeatherJSON(double latitude,double longitude);

	/* getter function returning the state of data retrieval */
	[OperationContract]
	bool SucessRetrieval();
	
	/* returns the current weather condition in string format */
	[OperationContract]
	string GetCondition();

	/* returns the current temperature using string format in centigrades */
	[OperationContract]
	string GetTemp();

	/* returns the current relative humidity using string format in 100% scale */
	[OperationContract]
	string GetHum();

	/* returns the current wind speed using string format in (km/h) */
	[OperationContract]
	string GetWndSpeed();
	
	/* returns the current wind direction in  degrees*/
	[OperationContract]
	string GetWndDir();

	/* returns the current weather condition icon representation URL string */
	[OperationContract]
	string GetIcon();
	
	/* returns the current cloud amounts using string format in 100% scale */
	[OperationContract]
	string GetCloudAmnt();
}

