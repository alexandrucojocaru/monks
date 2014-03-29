using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONRPC
{
	public class ClientFilterBase
	{
		/*		*
         *Should be used to 
         *-add extra request keys;
         *-translate or encode output params into the expected server request object format
         *@param array arrRequest
         *@return HashMap. Key is the name of the received parameter, and the value are their value. Null if no change
         */
		public virtual IDictionary<string, Object> beforeJSONEncode(IDictionary<string, Object> request)
		{
			return null;
		}

		/*		*
         *Should be used to
         *-encrypt, encode or otherwise prepare to JSON request string into the expected server input format
         *-log raw output
         *@param string strJSONRequst.
         *@param string strEndpointURL
         * @return String json string
	     * @throws UnsupportedEncodingException 
    	 * @throws NoSuchAlgorithmException 
	     * @throws InvalidKeyException 
	     * @throws MalformedURLException 
	     * @return HashMap. Key is the name of the received parameter, and the value are their value. Null if no change
         */
		public virtual IDictionary<string, string> afterJSONEncode(IDictionary<string, string> dictParams)
		{
			return null;
		}

		/*		*
         * Should be used to 
         * - decrypt, decode or otherwise prepare the JSON-RPC client format;
         * - log raw input
         * @param string strJSONResponse
         * @return String. Return a Json String, null if no change
         */
		public virtual String beforeJSONDecode(String strJSONResponse)
		{
			return null;
		}

		/*		
         * Should be used to rethrow exceptions as different types.
         * The first plugin to throw an exception will be the last one.
         * If there are no filter plugins registered or none of the plugins have thrown an exception, 
         * then JSONRPC_client will throw the original JSONRPC2SessionException.
         * @param JSONRPC2SessionException exception.
         */
		public virtual void exceptionCatch(Exception exception)
		{
		}

		/*		
         * First plugin to make a requst will be the last one. The respective plugin MUST set &$bCalled to true
         * @return IDictionary. Key is the name of the received parameter, and the value are their value. Null if no change
         */
		public virtual IDictionary<String, Object> makeRequest(IDictionary<String, Object> dictParams)
		{
			return null;
		}

	}
}
