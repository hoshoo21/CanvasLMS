using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace canvasApiTest
{
	class clsApiConfig
	{
		public clsApiConfig()
		{

		}

		public string accessToken
		{
			get; set;
		}

		public string apiUrl
		{
			get; set;
		}

		/// <summary>
		/// strings should be in the following format:
		/// method|paramName,value|paramName,value|...
		/// </summary>
		public string[] apiCalls
		{
			get;
			set;
		}
	}
}
