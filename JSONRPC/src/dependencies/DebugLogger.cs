using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JSONRPC
{
	public class DebugLogger : ClientFilterBase
	{
		public static readonly bool LOG_TO_CONSOLE = true;
		public static readonly bool LOG_TO_FILE = false;

		public string strLogPath = "";

		StreamWriter logger;

		public DebugLogger(bool bLogType)
		{
			if (!bLogType)
			{
				throw new Exception("Missing second argument, strLogPath.");
			}
		}

		public DebugLogger(bool bLogType, string strLogPath)
		{
			if (!bLogType)
			{
				if (strLogPath != "")
					this.strLogPath = strLogPath;
				else
					throw new Exception("No log path specified.");
				logger = new StreamWriter(this.strLogPath, true);
			}
		}

		public IDictionary<string, string> afterJSONEncode(IDictionary<string, string> dictParams)
		{
			Console.WriteLine("Sent makeRequest at:" + DateTime.Now.ToString());
			foreach(KeyValuePair<string, string> kvp in dictParams)
			{
				Console.WriteLine(kvp.Key.ToString());
				if (kvp.Value != null)
					Console.WriteLine(kvp.Value.ToString());
			}
			Console.WriteLine("\n");

			return null;
		}

		public string beforeJSONDecode(string strJSONResponse)
		{
			Console.WriteLine("Received response at:" + DateTime.Now.ToString());
			Console.WriteLine(strJSONResponse);
			Console.WriteLine("\n");

			return null;
		}
	}
}
