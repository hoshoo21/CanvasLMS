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
	/// Implementation of the Canvas Courses API definition found here:
	/// https://canvas.instructure.com/doc/api/courses.html
	/// /// </summary>
	public class clsCoursesApi : clsHttpMethods
	{
		////NLog
		private static Logger _logger = LogManager.GetLogger(typeof(clsCoursesApi).FullName);

		/// <summary>
		/// Returns the details of a specific course
		/// https://canvas.instructure.com/doc/api/courses.html#method.courses.show
		/// </summary>
		/// <param name="accessToken"></param>
		/// <param name="baseUrl"></param>
		/// <param name="canvasCourseId"></param>
		/// <returns>Returns a json object representing a course object: https://canvas.instructure.com/doc/api/courses.html#Course, throws exception if successful status code is not received </returns>
		public static async Task<dynamic> getCourseDetails(string accessToken, string baseUrl, long canvasCourseId, long sisCourseId = 0)
		{
			string rval = string.Empty;
			string urlCommand = "/api/v1/courses/:id";

			//if we have a valid sis course id, ask canvas to find the course using that identifier
			//  this will be done when we look for an existing course during the cloning request
			urlCommand = (sisCourseId > 0) ? urlCommand.Replace(":id", "sis_course_id:" + sisCourseId.ToString()) : urlCommand.Replace(":id", canvasCourseId.ToString());
			_logger.Debug("[getCourseDetails] " + urlCommand);
			
			using (HttpResponseMessage response = await httpGET(baseUrl, urlCommand, accessToken))
			{
				rval = await response.Content.ReadAsStringAsync();
				_logger.Debug("getCourseDetails API Response: [" + rval + "]");

				if (!response.IsSuccessStatusCode || (rval.Contains("errors") && rval.Contains("message")))
				{
					string msg = "[getCourseDetails]:[" + urlCommand + "] returned status[" + response.StatusCode + "]: " + response.ReasonPhrase;
					_logger.Error(msg);
					throw new HttpRequestException(rval);
				}
			}

			return Newtonsoft.Json.JsonConvert.DeserializeObject(rval);
		}

		/// <summary>
		/// Implementation of Create a new course API
		/// https://canvas.instructure.com/doc/api/courses.html#method.courses.create
		/// Pass in a dictionary of parameters[parameter name, value], use parameter names defined in the API documentation
		/// </summary>
		/// <param name="accessToken"></param>
		/// <param name="baseUrl"></param>
		/// <param name="accountId">the sub-account where the course should be created</param>
		/// <param name="vars">key value pairs representing parameter name and value</param>
		/// <returns>returns a json object representing a course https://canvas.instructure.com/doc/api/courses.html#Course, will throw an exception</returns>
		public static async Task<dynamic> postCreateCourse(string accessToken, string baseUrl, long accountId, Dictionary<string,string> vars)
		{
			dynamic result = null;
			string urlCommand = "/api/v1/accounts/:account_id/courses";

			urlCommand = urlCommand.Replace(":account_id", accountId.ToString());
			urlCommand = concatenateHttpVars(urlCommand, vars);
			_logger.Debug("[postCreateCourse] " + urlCommand);

			using (HttpResponseMessage response = await httpPOST(baseUrl, urlCommand, accessToken, null))
			{
				string rval = await response.Content.ReadAsStringAsync();
				_logger.Debug("[postCreateCourse] Response: [" + rval + "]");

				if (!response.IsSuccessStatusCode || (rval.Contains("errors") && rval.Contains("message")))
				{
					string msg = "[postCreateCourse]:[" + urlCommand + "] returned status[" + response.StatusCode + "]: " + response.ReasonPhrase;
					_logger.Error(msg);
					throw new HttpRequestException(rval);
				}
				result = Newtonsoft.Json.JsonConvert.DeserializeObject(rval);
			}

			return result;
		}


        public static async Task<dynamic> postCreateCourseDiscussion(string accessToken, string baseUrl, long accountId, Dictionary<string, string> vars)
        {
            dynamic result = null;
            string urlCommand = "/api/v1/courses/:course_id/discussion_topics";

            urlCommand = urlCommand.Replace(":course_id", accountId.ToString());
            urlCommand = concatenateHttpVars(urlCommand, vars);
            _logger.Debug("[postCreateCourse] " + urlCommand);

            using (HttpResponseMessage response = await httpPOST(baseUrl, urlCommand, accessToken, null))
            {
                string rval = await response.Content.ReadAsStringAsync();
                _logger.Debug("[postCreateCourse] Response: [" + rval + "]");

                if (!response.IsSuccessStatusCode || (rval.Contains("errors") && rval.Contains("message")))
                {
                    string msg = "[postCreateCourse]:[" + urlCommand + "] returned status[" + response.StatusCode + "]: " + response.ReasonPhrase;
                    _logger.Error(msg);
                    throw new HttpRequestException(rval);
                }
                result = Newtonsoft.Json.JsonConvert.DeserializeObject(rval);
            }

            return result;
        }

        /// <summary>
        /// Implementation of Update a Course
        /// https://canvas.instructure.com/doc/api/courses.html#method.courses.update
        /// Pass in a dictionary of parameters, use parameters names defined in API documnentation
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="baseUrl"></param>
        /// <param name="courseId"></param>
        /// <param name="vars"></param>
        /// <returns>returns a json object representing a course https://canvas.instructure.com/doc/api/courses.html#Course, will throw an exception</returns>
        public static async Task<dynamic> putUpdateCourse(string accessToken, string baseUrl, long courseId, Dictionary<string,string> vars)
		{
			dynamic result = null;
			string urlCommand = "/api/v1/courses/:id";

			urlCommand = urlCommand.Replace(":id", courseId.ToString());
			urlCommand = concatenateHttpVars(urlCommand, vars);
			_logger.Debug("[putUpdateCourse] " + urlCommand);

			using (HttpResponseMessage response = await httpPUT(baseUrl, urlCommand, accessToken, null))
			{
				string rval = await response.Content.ReadAsStringAsync();
				_logger.Debug("[putUpdateCourse] Response: " + rval);

				if (!response.IsSuccessStatusCode || (rval.Contains("errors") && rval.Contains("message")))
				{
					string msg = "[putUpdateCourse]:[" + urlCommand + "] returned status[" + response.StatusCode + "]: " + response.ReasonPhrase;
					_logger.Error(msg);
					throw new HttpRequestException(rval);
				}
				result = Newtonsoft.Json.JsonConvert.DeserializeObject(rval);
			}

			return result;
		}

        public static async Task<dynamic> GetAllCourse(string accessToken, string baseUrl,  Dictionary<string, string> vars)
        {
            dynamic result = null;
            string urlCommand = "/api/v1/courses";

            
            urlCommand = concatenateHttpVars(urlCommand, vars);
            _logger.Debug("[putUpdateCourse] " + urlCommand);

            using (HttpResponseMessage response = await httpGET(baseUrl, urlCommand, accessToken))
            {
                string rval = await response.Content.ReadAsStringAsync();
                _logger.Debug("[putUpdateCourse] Response: " + rval);

                if (!response.IsSuccessStatusCode || (rval.Contains("errors") && rval.Contains("message")))
                {
                    string msg = "[putUpdateCourse]:[" + urlCommand + "] returned status[" + response.StatusCode + "]: " + response.ReasonPhrase;
                    _logger.Error(msg);
                    throw new HttpRequestException(rval);
                }
                result = Newtonsoft.Json.JsonConvert.DeserializeObject(rval);
            }

            return result;
        }
    }
}
