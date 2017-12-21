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
using LMS.Areas.Canvas.CanvasAPI.canvasApiLib.Base;

namespace LMS.Areas.Canvas.CanvasAPI.canvasApiLib.API
{
    /// <summary>
	/// Implementation of the Canvas Analytics API definition found here:
	/// https://canvas.instructure.com/doc/api/analytics.html
	/// </summary>
	public class clsAnalyticsApi : clsHttpMethods
    {
        ////NLog
        private static Logger _logger = LogManager.GetLogger(typeof(clsAnalyticsApi).FullName);

        /// <summary>
        /// Implementation of Get course-level participation data
        /// https://canvas.instructure.com/doc/api/analytics.html#method.analytics_api.course_participation
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="baseUrl"></param>
        /// <param name="canvasCourseId"></param>
        /// <returns>returns a json object including page views and particpiations, see api documentation for detailed object structure, will throw an exception</returns>
        public static async Task<dynamic> getCourseLevelParticipationData(string accessToken, string baseUrl, int canvasCourseId)
        {
            dynamic rval = null;
            string urlCommand = "/api/v1/courses/:course_id/users";

            urlCommand = urlCommand.Replace(":course_id", canvasCourseId.ToString());

            _logger.Debug("[getCourseLevelParticipationData] command: " + urlCommand);

            using (HttpResponseMessage response = await httpGET(baseUrl, urlCommand, accessToken))
            {
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    rval = JsonConvert.DeserializeObject(result);
                }
                else
                {
                    _logger.Error("[getCourseLevelParticipationData]:[" + urlCommand + "] returned status[" + response.StatusCode + "]: " + response.ReasonPhrase + "\n" + result);
                    throw new Exception(result);
                }
            }

            return rval;
        }


        public static async Task<dynamic> getAllCourse(string accessToken, string baseUrl)
        {
            dynamic rval = null;
            string urlCommand = "/api/v1/courses";

            // urlCommand = urlCommand.Replace(":course_id", canvasCourseId.ToString());

            _logger.Debug("[getCourseLevelParticipationData] command: " + urlCommand);

            using (HttpResponseMessage response = await httpGET(baseUrl, urlCommand, accessToken))
            {
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    rval = JsonConvert.DeserializeObject(result);
                }
                else
                {
                    _logger.Error("[getCourseLevelParticipationData]:[" + urlCommand + "] returned status[" + response.StatusCode + "]: " + response.ReasonPhrase + "\n" + result);
                    throw new Exception(result);
                }
            }

            return rval;
        }

        /// <summary>
        /// Implementation of Get course-level assignment data
        /// https://canvas.instructure.com/doc/api/analytics.html#method.analytics_api.course_assignments
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="baseUrl"></param>
        /// <param name="canvasCourseId"></param>
        /// <returns>returns a dynamic json object, see the api documentation for details, will throw an exception</returns>
        public static async Task<dynamic> getCourseLevelAssignmentData(string accessToken, string baseUrl, int canvasCourseId)
        {
            dynamic rval = null;
            string urlCommand = "/api/v1/courses/:course_id/analytics/assignments";

            urlCommand = urlCommand.Replace(":course_id", canvasCourseId.ToString());

            _logger.Debug("[getCourseLevelAssignmentData] command: " + urlCommand);

            using (HttpResponseMessage response = await httpGET(baseUrl, urlCommand, accessToken))
            {
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    rval = JsonConvert.DeserializeObject(result);
                }
                else
                {
                    _logger.Error("[getCourseLevelAssignmentData]:[" + urlCommand + "] returned status[" + response.StatusCode + "]: " + response.ReasonPhrase + "\n" + result);
                    throw new Exception(result);
                }
            }

            return rval;
        }

        /// <summary>
        /// Implementation of Get course-level student summary data
        /// https://canvas.instructure.com/doc/api/analytics.html#method.analytics_api.course_student_summaries
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="baseUrl"></param>
        /// <param name="canvasCourseId"></param>
        /// <returns>returns dynamic json object, see api document for details, will throw an exception</returns>
        public static async Task<dynamic> getCourseLevelStudentSummaryData(string accessToken, string baseUrl, int canvasCourseId)
        {
            dynamic rval = null;
            string urlCommand = "/api/v1/courses/:course_id/analytics/student_summaries";

            urlCommand = urlCommand.Replace(":course_id", canvasCourseId.ToString());

            _logger.Debug("[getCourseLevelStudentSummaryData] command: " + urlCommand);

            using (HttpResponseMessage response = await httpGET(baseUrl, urlCommand, accessToken))
            {
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    rval = JsonConvert.DeserializeObject(result);
                }
                else
                {
                    _logger.Error("[getCourseLevelStudentSummaryData]:[" + urlCommand + "] returned status[" + response.StatusCode + "]: " + response.ReasonPhrase + "\n" + result);
                    throw new Exception(result);
                }
            }

            return rval;
        }

        /// <summary>
        /// Implementation of Get user-in-a-course-level participation data
        /// https://canvas.instructure.com/doc/api/analytics.html#method.analytics_api.student_in_course_participation
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="baseUrl"></param>
        /// <param name="canvasCourseId"></param>
        /// <param name="canvasUserId"></param>
        /// <returns>returns a dynamic json object, will throw exception</returns>
        public static async Task<dynamic> getUserInACourseParticipationData(string accessToken, string baseUrl, long canvasCourseId, long canvasUserId)
        {
            dynamic rval = null;
            string urlCommand = "/api/v1/courses/:course_id/analytics/users/:student_id/activity";

            urlCommand = urlCommand.Replace(":course_id", canvasCourseId.ToString());
            urlCommand = urlCommand.Replace(":student_id", canvasUserId.ToString());

            _logger.Debug("[getUserInACourseParticipationData] command: " + urlCommand);

            using (HttpResponseMessage response = await httpGET(baseUrl, urlCommand, accessToken))
            {
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    rval = JsonConvert.DeserializeObject(result);
                }
                else
                {
                    _logger.Error("[getUserInACourseParticipationData]:[" + urlCommand + "] returned status[" + response.StatusCode + "]: " + response.ReasonPhrase + "\n" + result);
                    throw new Exception(result);
                }
            }

            return rval;
        }

        /// <summary>
        /// Implementation of Get user-in-a-course-level assignment data
        /// https://canvas.instructure.com/doc/api/analytics.html#method.analytics_api.student_in_course_assignments
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="baseUrl"></param>
        /// <param name="canvasCourseId"></param>
        /// <param name="canvasUserId"></param>
        /// <returns>returns a dynamic json object, will throw an exception</returns>
        public static async Task<dynamic> getUserInACourseAssignmentData(string accessToken, string baseUrl, long canvasCourseId, long canvasUserId)
        {
            dynamic rval = null;
            string urlCommand = "/api/v1/courses/:course_id/analytics/users/:student_id/assignments";

            urlCommand = urlCommand.Replace(":course_id", canvasCourseId.ToString());
            urlCommand = urlCommand.Replace(":student_id", canvasUserId.ToString());

            _logger.Debug("[getUserInACourseAssignmentData] command: " + urlCommand);

            using (HttpResponseMessage response = await httpGET(baseUrl, urlCommand, accessToken))
            {
                string result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    rval = JsonConvert.DeserializeObject(result);
                }
                else
                {
                    _logger.Error("[getUserInACourseAssignmentData]:[" + urlCommand + "] returned status[" + response.StatusCode + "]: " + response.ReasonPhrase + "\n" + result);
                    throw new Exception(result);
                }
            }

            return rval;
        }
    }
}