using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using NLog;

using canvasApiLib.Base;

namespace canvasApiLib.API
{
	public class clsLoginsApi : clsHttpMethods
	{
		////NLog
		private static Logger _logger = LogManager.GetLogger(typeof(clsLoginsApi).FullName);
		
		/// <summary>
		/// Implement Create a user login API call
		/// https://canvas.instructure.com/doc/api/logins.html#method.pseudonyms.create
		/// </summary>
		/// <param name="accessToken"></param>
		/// <param name="baseUrl"></param>
		/// <param name="accountId">Represents the Canvas sub-account for the login</param>
		/// <param name="vars">See API documentation for list of variables to pass in, these variables will be parsed into the API call</param>
		/// <returns>a json object representing the newly created login, if the login id alredy exists you will receive an error object, inspect result object for "errors"</returns>
		public static async Task<dynamic> postCreateUserLogin(string accessToken, string baseUrl, int accountId, Dictionary<string, string> vars)
		{
			dynamic result = null;
			string urlCommand = "/api/v1/accounts/:account_id/logins";
			
			urlCommand = urlCommand.Replace(":account_id", accountId.ToString());
			urlCommand = concatenateHttpVars(urlCommand, vars);
			_logger.Debug("[postCreateUserLogin] " + urlCommand);

			using (HttpResponseMessage response = await httpPOST(baseUrl, urlCommand, accessToken, null))
			{
				string json = await response.Content.ReadAsStringAsync();
				_logger.Debug("postCreateUserLogin API Response: [" + json + "]");

				if (response.IsSuccessStatusCode)
				{
					result = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
				}
				else
				{
					string msg = "[postCreateUserLogin]:[" + urlCommand + "] returned status[" + response.StatusCode + "]: " + response.ReasonPhrase;
					_logger.Error(msg + "\n" + json);
					throw new Exception(json);
				}
			}

			return result;
		}
	}
}
