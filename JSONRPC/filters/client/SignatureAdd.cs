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
	public class SignatureAdd : ClientFilterBase
	{
		/*		*
         * Private key used for hashed messages sent to the server.
         */
		protected String _key;



		/*		*
         * Private userID used for connection to a infrastructure
         */
		protected IDictionary<string, Object> _arrExtraURLVariables;

		/*		*
         * This function is a constructor function. It initiates only the Key.
         * 
         * @param string strKey. The private key used for hashed messages sent to the server.
         */
		public SignatureAdd(string strKey)
		{
			this._key = strKey;
		}

		/*		*
         * This is the constructor function. It creates a new instance of SignatureAdd.
         * Example: SignatureAdd("secretKey")
         * 
         * @param string strKey. The private key used for hashed messages sent to the server.
         */
		public SignatureAdd(string key, IDictionary<string, Object> arrExtraURLVariables)
		{
			this._key = key;
			this._arrExtraURLVariables = new Dictionary<string, Object>();
			foreach (KeyValuePair<string, Object> kvp in arrExtraURLVariables)
			{
				this._arrExtraURLVariables.Add(kvp.Key, kvp.Value);
			}
		}

		/*		*
         * This function sets an uptime for the request.
         */
		public IDictionary<String, Object> beforeJSONEncode(IDictionary<String, Object> request)
		{
			request.Add("expires", System.DateTime.Now.Millisecond / 1000L + 86400);

			return request;
		}

		/*		*
	     * Method for the calculation of the hey.
         * 
	     * @param String msg. Message to encode.
	     * @param String keyString. The authentification key.
	     * @return sBuilder.ToString()
	     */
		public String hmacDigest(string msg, string key)
		{
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

			//cripting the key into HMACSHA256 standard
			HMACSHA256 hmac256 = new HMACSHA256(encoding.GetBytes(key));

			//create a byte array that converts the input string into hash
			byte[] data = hmac256.ComputeHash(Encoding.UTF8.GetBytes(msg));

			StringBuilder sBuilder = new StringBuilder();

			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}
			return sBuilder.ToString();
		}

		/*		*
          * This function is used for authentication. It alters the Endpoint URL such that it contains
          * a specific signature.
          * 
          * @param string hashedStringToAddToURL
          * @param Uri toEditURL
          *
          * @return Uri toReturnUri
          */
		public override IDictionary<string, string> afterJSONEncode(IDictionary<string, string> dictParams)
		{
			Uri toReturnUri;
			string strHashed = hmacDigest(dictParams["strJSONRequest"], _key);
			string strURL = dictParams["strURL"];

			if (strURL.Contains("?verify=") == true)
			{
				/**
                 * TODO: In case if fails making rpc calls it's maybe from here
                 * 
                 * Use the extractUserID function above for refactoring, if needed
                 */

				toReturnUri = new Uri(strURL + "&verify=" + strHashed);
			}
			else
			{
				//TODO: or here...
				toReturnUri = new Uri(strURL + "?verify=" + strHashed);
			}

			foreach (KeyValuePair<string, Object> kvp in this._arrExtraURLVariables)
			{
				toReturnUri = new Uri(toReturnUri.ToString() + "&" + kvp.Key + "=" + kvp.Value);
			}

			dictParams.Add("strJSONEndpointURL", toReturnUri.ToString());
			return dictParams;
		}
	}
}
