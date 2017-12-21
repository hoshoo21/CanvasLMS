using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using NLog;

using canvasApiLib.Base;

namespace canvasApiLib.API
{
	/// <summary>
	/// Implementation of the Canvas Enrollments API definition found here:
	/// https://canvas.instructure.com/doc/api/enrollments.html
	/// </summary>
	public class clsEnrollmentsApi : clsHttpMethods
	{
		////NLog
		private static Logger _logger = LogManager.GetLogger(typeof(clsEnrollmentsApi).FullName);

		/// <summary>
		/// enrollUserInCourse 
		/// https://canvas.instructure.com/doc/api/enrollments.html#method.enrollments_api.create
		/// </summary>
		/// <param name="accessToken">private key for API authentication</param>
		/// <param name="baseUrl">URL to the specific site hosting the course</param>
		/// <param name="canvasCourseId">Canvas course ID, found in the URL if you browse to the course in Canvas</param>
		/// <param name="vars">Dictionary of parameters to pass in API call</param>
		/// <returns>A Json Enrollment object https://canvas.instructure.com/doc/api/enrollments.html#Enrollment, will throw exception with full HttpResponseMessage object if status 200 is not received </returns>
		public static async Task<dynamic> postEnrollUserInCourse(string accessToken, string baseUrl, int canvasCourseId, Dictionary<string,string> vars)
		{
			string rval = string.Empty;
			string urlCommand = "/api/v1/courses/:course_id/enrollments";

			urlCommand = urlCommand.Replace(":course_id", canvasCourseId.ToString());
			urlCommand = concatenateHttpVars(urlCommand, vars);
			_logger.Debug("[postEnrollUserInCourse] " + urlCommand);

			using (HttpResponseMessage response = await httpPOST(baseUrl, urlCommand, accessToken, null))
			{
				rval = await response.Content.ReadAsStringAsync();
				_logger.Debug("EnrollUser API Response: [" + rval + "]");

				if (!response.IsSuccessStatusCode || (rval.Contains("errors") && rval.Contains("message")))
				{
					string msg = "[postEnrollUserInCourse]:[" + urlCommand + "] returned status[" + response.StatusCode + "]: " + response.ReasonPhrase;
					_logger.Error(msg);
					throw new HttpRequestException(rval);
				}
			}

			return Newtonsoft.Json.JsonConvert.DeserializeObject(rval);
		}

		/// <summary>
		/// getCourseEnrollments
		/// https://canvas.instructure.com/doc/api/enrollments.html#method.enrollments_api.index
		/// </summary>
		/// <param name="accessToken"></param>
		/// <param name="baseUrl"></param>
		/// <param name="canvasCourseId"></param>
		/// <param name="canvasSectionId"></param>
		/// <returns>a dynamic list of Enrollment objects https://canvas.instructure.com/doc/api/enrollments.html#Enrollment, throws an exception if a successful status code is not received</returns>
		public static async Task<List<dynamic>> getCourseEnrollments(string accessToken, string baseUrl, int canvasCourseId, int canvasSectionId = 0)
		{
			List<dynamic> enrollments = new List<dynamic>();
			string urlCommand = "/api/v1/courses/:course_id/enrollments";
			int pageNumber = 1;

			urlCommand = urlCommand.Replace(":course_id", canvasCourseId.ToString());
			if(canvasSectionId > 0)
			{
				urlCommand = "/api/v1/sections/:section_id/enrollments";
				urlCommand = urlCommand.Replace(":section_id", canvasSectionId.ToString());
			}

			//make sure we handle multiple pages, if more than 100 results are returned
			while (true)
			{
				dynamic objEnrollments = null;
				string pageCommand = urlCommand + "?per_page=" + _maxPageCount + "&page=" + pageNumber.ToString();

				using (HttpResponseMessage response = await httpGET(baseUrl, pageCommand, accessToken))
				{
					string result = await response.Content.ReadAsStringAsync();
					if (response.IsSuccessStatusCode)
					{
						objEnrollments = JsonConvert.DeserializeObject(result);
					}
					else
					{
						string msg = "[postEnrollUserInCourse]:[" + urlCommand + "] returned status[" + response.StatusCode + "]: " + response.ReasonPhrase + "\n" + result;
						throw new Exception(result);
					}

					if (objEnrollments == null || objEnrollments.Count == 0)
					{
						break;
					}
					else
					{
						for (int i = 0; i < objEnrollments.Count; i++)
						{
							enrollments.Add(objEnrollments[i]);
						}
						if (objEnrollments.Count < _maxPageCount)
							break;
						else
							pageNumber++;
					}
				}
			}

			return enrollments;
		}
	}
}
