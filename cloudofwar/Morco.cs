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
using JSONRPC;




namespace cloudofwar
{
    class Morco
    {
        JSONRPCClient _JSONRPCClient;




        public Morco() {


        }



        public JObject quickmatch(string strUserID)
		{
			JArray arrParameters = new JArray();
			arrParameters.Add(strUserID);
		    JObject toReturn = _JSONRPCClient.rpc(arrParameters, "quickmatch");
		    return toReturn;
	    }


		public JArray terrainTiles(int nGameID)
		{
			JArray arrParameters = new JArray();
			arrParameters.Add(nGameID);
		    JObject objResponse = _JSONRPCClient.rpc(arrParameters, "terrainTiles");
		    JTokenReader jt = new JTokenReader(objResponse["result"]);
		    JArray arrToReturn = JArray.Parse(jt.ReadAsString());
		    return arrToReturn;
	    }


		public JObject unitStates(string strUserID, int nGameID, JArray arrUnitMoves)
		{
			JArray arrParameters = new JArray();
			arrParameters.Add(strUserID);
			arrParameters.Add(nGameID);
			arrParameters.Add(arrUnitMoves);
		    JObject toReturn = _JSONRPCClient.rpc(arrParameters, "unitStates");
		    return toReturn;
	    }


		public JObject replay(int nGameID)
		{
			JArray arrParameters = new JArray();
			arrParameters.Add(nGameID);
		    JObject toReturn = _JSONRPCClient.rpc(arrParameters, "replay");
		    return toReturn;
	    }


		public JObject gameConfig()
		{
			JArray arrParameters = new JArray();
		    JObject toReturn = _JSONRPCClient.rpc(arrParameters, "gameConfig");
		    return toReturn;
	    }



    }
}
