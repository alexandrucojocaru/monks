using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using AustinHarris.JsonRpc;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Security.Cryptography;


namespace JSONRPC
{
	public class JSONRPCClient
	{
		#region Attributes
		/*		*
       * JSON-RPC protocol call ID.
       */
		protected int _nCallID = 0;

		/*		*
         * String that contains the server URL.
         */
		protected string _strUrl;

		/*		*
         * Public Username used for autenthification.
         */
		protected string _strHTTPUsername;

		/*		*
          * Public Password used for autenthification.
          */
		protected string _strHTTPPassword;

		/*		*
         * Filter plugins which extends JSONRPC_client_filter_plugin_base.
         */
		public List<ClientFilterBase> arrFilterPlugins = new List<ClientFilterBase>();

		#endregion

		#region Methods

		/*		*
         * Empty constructor.
         */
		public JSONRPCClient() { }

		/*		*
          * Parameterized constructor.
          */
		public JSONRPCClient(string strURL)
		{
			this._strUrl = strURL;
		}

		/*		*
         * Sets Username and Password. 
         * 
         * @param username
         * @param password
         * 
         */
		public void httpCredentialsSet(string username, string password)
		{
			this._strHTTPUsername = username;
			this._strHTTPPassword = password;

		}

		/*		*
         * Main function which makes a request to the server, gets and parses the response, outputs it through a string.
         * 
         * @param jarrToSendParameters: array that contains parameters for the request function
         * @param strToSendMethodL: string that contains the name of the function
         * 
         * @return strData data received from the server
         */
		public JObject rpc(JArray jarrToSendParameters, string strToSendMethod)
		{
			try
			{
				//accepting any certificates
				ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

				//setting link and key
				Uri uriUrl = new Uri(this._strUrl);

				//creating a JSON request
				JObject jsonRpcObject = new JObject();
				jsonRpcObject["jsonrpc"] = "2.0";
				jsonRpcObject["id"] = ++this._nCallID;
				jsonRpcObject["method"] = strToSendMethod;
				jsonRpcObject["params"] = jarrToSendParameters;

				//serialization of Request object so that it has the form of a JSON Message
				string strSerializedJsonObject = JsonConvert.SerializeObject(jsonRpcObject);
                
                //preparing new URL to acces the server
                Uri newUrl = new Uri(this._strUrl);

				foreach (ClientFilterBase filterPlugin in arrFilterPlugins)
				{
                    IDictionary<string, string> dictParse = new Dictionary<string, string>();
					dictParse.Add("strJSONRequest", strSerializedJsonObject);
                    dictParse.Add("strURL", this._strUrl);

					dictParse = filterPlugin.afterJSONEncode(dictParse);
                    if (dictParse != null)
                    {
                        if (dictParse["strJSONRequest"] != null)
                        {
                            strSerializedJsonObject = dictParse["strJSONRequest"];
                        }
                        if (dictParse["strJSONEndpointURL"] != null)
                        {
                            newUrl = new Uri(dictParse["strJSONEndpointURL"].ToString());
                        }
                    }
				}



				//creating a HTTPReqest
				HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(newUrl);

				//setting HTTPRequest attributes
				httpRequest.Method = "POST";
				httpRequest.ContentType = "application/json-rpc";
				httpRequest.KeepAlive = false;
				httpRequest.SendChunked = true;

				//converting strSerializedJsonObject to an byteAray
				byte[] byteArray = Encoding.UTF8.GetBytes(strSerializedJsonObject);
				httpRequest.ContentLength = byteArray.Length;

				//streaming it to the server
				Stream dataStream = httpRequest.GetRequestStream();
				dataStream.Write(byteArray, 0, byteArray.Length);
				dataStream.Close();

				//Creating a HTTPResponse for the Request.
				HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
				Stream receiveStream = httpResponse.GetResponseStream();
				StreamReader srReadStream = new StreamReader(receiveStream, Encoding.UTF8);

				//Printing the information received through and saving it into a string value.
				Console.WriteLine("Response stream received");
				string strData = srReadStream.ReadToEnd();
				IDictionary<string, object> dictDataDeserialized = JsonConvert.DeserializeObject<IDictionary<string, Object>>(strData);

                JObject objToReturn = processRAWResponse(strData);
                return objToReturn;
			}
			catch (System.Net.WebException ex)
			{

				Stream receiveStream = ex.Response.GetResponseStream();
				StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

				string strData = readStream.ReadToEnd();

				JObject objParsed = JObject.Parse(strData);


				Console.WriteLine(objParsed["error"].ToString());
				JSONRPC_Exception jsonException = new JSONRPC_Exception(objParsed["error"]["message"].ToString(), Convert.ToInt32(objParsed["error"]["code"].ToString()));
				foreach (ClientFilterBase cfb in this.arrFilterPlugins)
				{
					cfb.exceptionCatch(jsonException);
				}

				throw jsonException;

			}
			return null;
		}

		/*		*
         * Deserializes the json response string into a JObject.
         * 
         * @param strRAWResponse 
         * 
         * @return resultKey : returns only the ["result"] of the JsonResponse
         */
		public JObject processRAWResponse(string strRAWResponse)
		{
            JObject jResultKey = JObject.Parse(strRAWResponse);
            jResultKey = JObject.Parse(jResultKey["result"].ToString());

			//if (typeof(T) != typeof(string))
			//{
			//toReturn = (T)Convert.ChangeType(resultKey["result"].ToString(), typeof(T));
			//}
			//else
			//{
			//    toReturn = (T)Convert.ChangeType(resultKey["result"].ToString(), typeof(T));
			//}
			return jResultKey;

		}

		/*		*
         * Calls rpc whith the "rpc.functions" call to the server.
         * 
         * @return dictToReturn Dictionary containing all funcitions.
         */
		public JObject rpcFunctions()
		{
			JObject jobjParsedResponse = this.rpc(new JArray(), "rpc.functions");
			return jobjParsedResponse;
		}

		/*		*
         * Calls rpc with the rpc.ReflectionFunction call.
         * 
         * 
         * @return toReturnFunction FuncionDetails object containing information for ONLY ONE function.
         */
		public FunctionDetails rpcReflectionFunction(string strParameter)
		{
			JArray arrRPCReflection = new JArray();
			arrRPCReflection.Add(strParameter);

			JObject objFunctions = this.rpc(arrRPCReflection, "rpc.reflectionFunction");
			FunctionDetails toReturnFunction = new FunctionDetails();
			toReturnFunction.setName(strParameter);

			IDictionary<string, JToken> functionDetails = (IDictionary<string, JToken>)objFunctions;
			JArray arrParams = (JArray)functionDetails["function_params"];
			foreach (JObject functionParameter in arrParams)
			{
				string strTempKey = functionParameter["param_name"].ToString();
				string strTempValue = functionParameter["param_type"].ToString();
				toReturnFunction.addValueToDict(strTempKey, strTempValue);
			}
			return toReturnFunction;
		}

		/*		*
         * Function that based on the response from the "rpc.functions" call returns array of FunctionDetails
         * 
         * @param arrayOfParameters Response from rpc.functions.
         * 
         * @return resultsToReturn 
         */
		public FunctionDetails[] rpcReflectionFunctions(JArray arrayOfParameters)
		{
			//initializing counter for the array
			int nCounter = 0;

			//adding the parameters array into another array
			JArray paramList = new JArray();
			paramList.Add(arrayOfParameters);

			//adding into a JObject the processed response of the rpc.reflectionFunctions call
			JObject functions = this.rpc(paramList, "rpc.reflectionFunctions"); ;

			// counting the number of functions returned by the call
			int nSizeCounter = 0;
			foreach (KeyValuePair<string, JToken> function in functions)
				nSizeCounter++;

			//allocating memory for the new FunctionDetails array
			FunctionDetails[] fdResultsToReturn = new FunctionDetails[nSizeCounter];

			//parsing the information 
			foreach (KeyValuePair<string, JToken> function in functions)
			{
				fdResultsToReturn[nCounter] = new FunctionDetails();
				fdResultsToReturn[nCounter].setName(function.Key.ToString());
				IDictionary<string, JToken> functionDetails = (IDictionary<string, JToken>)function.Value;
				fdResultsToReturn[nCounter].setReturnType(functionDetails["function_return_type"].ToString());
				JArray functionParams = (JArray)functionDetails["function_params"];
				foreach (JObject functionParameter in functionParams)
				{
					string tempKey = functionParameter["param_name"].ToString();
					string tempValue = functionParameter["param_type"].ToString();

					fdResultsToReturn[nCounter].addValueToDict(tempKey, tempValue);
				}
				nCounter++;
			}

			return fdResultsToReturn;
		}

		/*		*
         * This function is used to add filter plugins to an instance of JSONRPC_client.
         * If there is an attempt to add multiple instances of the same filter,
         * an exception is thrown.
         * 
         * @param filter A new instance of a given plugin
         */
		public void addFilterPlugins(ClientFilterBase filter)
		{
			foreach (ClientFilterBase filterPlugin in this.arrFilterPlugins)
			{
				if (filterPlugin.GetType() == filter.GetType())
					throw new Exception("Multiple instances of the same filter not allowed");
			}
			arrFilterPlugins.Add(filter);
		}

		/*		*
         * This function is used to remove client filter plugins.
         * If there is an attempt to remove an unregistred filter plugin,
         * an exception is thrown.
         * 
         * @param filter The instance og the plugin that will be removed
         * 
         * @throws Exception
         */
		public void removeFilterPlugin(ClientFilterBase filter)
		{
			int nIndex = 0;

			foreach (ClientFilterBase filterPlugin in arrFilterPlugins)
			{
				if (filterPlugin.GetType() == filter.GetType())
					break;
				nIndex++;
			}

			if (nIndex == arrFilterPlugins.Capacity)
			{
				throw new Exception("Failed to remove filter plugin object, maybe plugin is not registered.");
			}
			arrFilterPlugins.Remove(filter);

		}

		/*		*
         * Encodes string to 64.
         * @param toEncode
         * 
         * @return returnValue
         */
		public string EncodeTo64(string strToEncode)
		{
			byte[] byteArray = System.Text.ASCIIEncoding.ASCII.GetBytes(strToEncode);

			string strReturnValue = System.Convert.ToBase64String(byteArray);

			return strReturnValue;
		}

		static void Main(string[] args)
		{

		}
		#endregion
	}
}
