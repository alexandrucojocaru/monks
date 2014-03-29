using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONRPC
{
	public class FunctionDetails
	{
		/*		*
         * Stores function name.
         */
		public string strName;

		/*		*
         * Dictionary with parameter names as key, and parameter type as value.
         */
		public IDictionary<string, string> dictParameters;

		/*		*
         * String that stores return type of function.
         */
		public string strReturnType;

		/*		*
         * Empty constructor.
         */
		public FunctionDetails()
		{
			this.strReturnType = null;
			this.strName = null;
			this.dictParameters = new Dictionary<string, string>();
		}

		/*		*
         * Parameter constructor.
         * 
         * @param name
         * @param comment
         * @param parameters
         */
		public FunctionDetails(string name, string returnType, IDictionary<string, string> parameters)
		{
			this.strReturnType = returnType;
			this.strName = name;
			this.dictParameters = parameters;
		}

		/*		*
         * Returns "name" atribute of the object.
         * 
         * @return strName
         */
		public string getName()
		{
			return strName;
		}

		/*		*
         * Returns the IDictionary atribute of the object.
         * 
         * @return getDictParameters
         */
		public IDictionary<string, string> getDictParameters()
		{
			return dictParameters;
		}

		/*		*
         * Unites all information contained into an object into a string.
         * 
         * @return toReturn
         */
		public String toString()
		{
			string toReturn = "Function Name:  " + strName + "\nParameters:  " + dictParameters.ToString();
			return toReturn;
		}

		/*		*
         * Sets the name of the FunctionDetail object.
         * 
         * @param strName
         */
		public void setName(string strName)
		{
			this.strName = strName;
		}

		/*		*
         * Adds a SINGLE key-value pair to the IDictionary.
         * 
         * @param key
         * @param value
         */
		public void addValueToDict(string key, string value)
		{
			this.dictParameters.Add(key, value);
		}

		/*		*
         * Copies and entire IDictionary into another.
         * 
         * @param dictParameters
         */
		public void setDictionary(IDictionary<string, string> dictParameters)
		{
			foreach (KeyValuePair<string, string> kvp in dictParameters)
			{
				this.dictParameters.Add(kvp.Key, kvp.Value);
			}
		}

		/*		*
         * Sets return type of function.
         * 
         * @param returnType
         */
		public void setReturnType(string returnType)
		{
			this.strReturnType = returnType;
		}

		/*		*
         * Returns return type of funtion.
         * 
         * @return this.strReturnType
         */
		public string getReturnType()
		{
			return this.strReturnType;
		}
	}
}