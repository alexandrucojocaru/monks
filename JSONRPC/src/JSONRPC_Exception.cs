using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONRPC
{
	public class JSONRPC_Exception : System.Net.WebException
	{

		private int _nErrorCode;

		private string _strErrorMessage;

		public int getCode()
		{
			return this._nErrorCode;
		}

		public string getMessage()
		{
			return this._strErrorMessage;
		}


		public JSONRPC_Exception(String strMessage, int nCode)
			: base(strMessage + "; Exception code: " + nCode)
		{
			this._nErrorCode = nCode;
			this._strErrorMessage = strMessage;
		}


		/*		*
         * Bad credentials (user, password, signing hash, account does not exist, etc.)
         * Not part of JSON-RPC 2.0 spec.
         */
		public static readonly int NOT_AUTHENTICATED = -1;

		/*		*
        * The authenticated user is not authorized to make any or some requests.
        * Not part of JSON-RPC 2.0 spec.
        */
		public static readonly int NOT_AUTHORIZED = -2;

		/*		*
	    * The request has expired. The requester must create or obtain a new request.
	    * Not part of JSON-RPC 2.0 spec.
	    */
		public static readonly int REQUEST_EXPIRED = -3;

		/*		*
        * Parse error.
        * Invalid JSON was received by the server.
        * An error occurred on the server while parsing the JSON text.
        */
		public static readonly int PARSE_ERROR = -32700;

		/*		*
        * Invalid Request.
        * The JSON sent is not a valid Request object.
        */
		public static readonly int INVALID_REQUEST = -32600;

		/*		*
        * Method not found.
        * The method does not exist / is not available.
        */
		public static readonly int METHOD_NOT_FOUND = -32601;

		/*		*
        * Invalid params.
        * Invalid method parameter(s).
        */
		public static readonly int INVALID_PARAMS = -32602;

		/*		*
        * Internal error.
        * Internal JSON-RPC error.
        */
		public static readonly int INTERNAL_ERROR = -32603;

		//-32000 to -32099 Server error. Reserved for implementation-defined server-errors.

	}

}
