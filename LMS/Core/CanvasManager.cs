using LMS.Areas.Canvas.Models;
using LMS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace LMS.Core
{
    public enum CanvasCommand
    {
        UNKNOWN = 0
        , GET_COURSE_DETAIL = 1
        , GET_COURSE_LIST = 2
        , GET_COURSE_DISCUSSIONS_TOPICS = 3
        , GET_ACCOUNT_LIST = 4
        , CREATE_COURSE = 5
        , UPDATE_COURSE = 6
        , DELETE_COURSE = 7
        , GET_COURSE_DISCUSSION_ENTRIES = 8
        , REPLY_TO_DISCUSSION_ENTRY = 9
        , MARK_ENTRY_AS_READ = 10
    }

    public class CanvasManager : ICanvasManager
    {
        private CanvasCommand CommandToExecute = CanvasCommand.UNKNOWN;
        private string Method = "GET";
        public string CanvasAccessToken
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["CanvasAccessToken"];
            }
        }
        public string CanvasUrl
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["CanvasUrl"];
            }
        }
        private async Task<string> CommandUrl()
        {
            string sCommandUrl = string.Empty;
            switch (this.CommandToExecute)
            {
                case CanvasCommand.GET_COURSE_DETAIL:
                    sCommandUrl = "/api/v1/courses/";
                    this.Method = "GET";
                    break;

                case CanvasCommand.GET_COURSE_LIST:
                    sCommandUrl = "/api/v1/courses";
                    this.Method = "GET";
                    break;

                case CanvasCommand.GET_COURSE_DISCUSSIONS_TOPICS:
                    sCommandUrl = "/api/v1/courses/{0}/discussion_topics";
                    this.Method = "GET";
                    break;

                case CanvasCommand.GET_ACCOUNT_LIST:
                    sCommandUrl = "/api/v1/accounts";
                    this.Method = "GET";
                    break;

                case CanvasCommand.CREATE_COURSE:
                    sCommandUrl = string.Format("/api/v1/accounts/{0}/courses", await this.ParentAccountId());
                    this.Method = "POST";
                    break;

                case CanvasCommand.UPDATE_COURSE:
                    sCommandUrl = "/api/v1/courses/{0}";
                    this.Method = "PUT";
                    break;

                case CanvasCommand.DELETE_COURSE:
                    sCommandUrl = "/api/v1/courses/{0}";
                    this.Method = "DELETE";
                    break;

                case CanvasCommand.GET_COURSE_DISCUSSION_ENTRIES:
                    sCommandUrl = "/api/v1/courses/{0}/discussion_topics/{1}/entries";
                    this.Method = "GET";
                    break;

                case CanvasCommand.REPLY_TO_DISCUSSION_ENTRY:
                    ///api/v1/courses/:course_id/discussion_topics/:topic_id/entries/:entry_id/replies
                    sCommandUrl = "/api/v1/courses/{0}/discussion_topics/{1}/entries/{2}/replies";
                    this.Method = "POST";
                    break;

                case CanvasCommand.MARK_ENTRY_AS_READ:
                    ///api/v1/courses/:course_id/discussion_topics/:topic_id/entries/:entry_id/replies
                    sCommandUrl = "/api/v1/courses/{0}/discussion_topics/{1}/entries/{2}/read";
                    this.Method = "PUT";
                    break;

                default:
                    sCommandUrl = string.Empty;
                    break;
            }
            if (!string.IsNullOrEmpty(sCommandUrl))
            {
                sCommandUrl = string.Concat(this.CanvasUrl, sCommandUrl);
            }
            return sCommandUrl.Trim();
        }

        private JavaScriptSerializer _Serializer = null;
        private JavaScriptSerializer Serializer
        {
            get
            {
                if(_Serializer == null)
                {
                    _Serializer = new JavaScriptSerializer();
                }
                return _Serializer;
            }
        }

        private List<CanvasAccountModel> _AccountList = null;
        private async Task<List<CanvasAccountModel>> AccountList()
        {
            if (_AccountList == null)
            {
                this.CommandToExecute = CanvasCommand.GET_ACCOUNT_LIST;
                string sUrl = await this.CommandUrl();
                object oResponse = await Execute(sUrl);
                if (oResponse != null)
                {
                    string sJson = oResponse as string;
                    if (sJson != null)
                    {
                        _AccountList = sJson.FromJson<List<CanvasAccountModel>>();
                    }
                }
            }
            return _AccountList;
        }

        private CanvasAccountModel _ParentAccount = null;
        private async Task<CanvasAccountModel> ParentAccount()
        {
            if (_ParentAccount == null)
            {
                List<CanvasAccountModel> temp = await AccountList() as List<CanvasAccountModel>;
                if (temp != null && temp.Count > 0)
                {
                    _ParentAccount = (from a in temp
                                      where a.parent_account_id == null
                                      select a).FirstOrDefault();
                }
            }
            return _ParentAccount;
        }
        private async Task<int> ParentAccountId()
        {
            int iAccountId = 0;
            string sAccountId = System.Configuration.ConfigurationManager.AppSettings["CanvasAccountId"];
            if(string.IsNullOrEmpty(sAccountId))
            {
                CanvasAccountModel temp = await ParentAccount();
                if (temp != null)
                {
                    iAccountId = temp.id;
                }
            }
            else
            {
                iAccountId = (int)Convert.ChangeType(System.Configuration.ConfigurationManager.AppSettings["CanvasAccountId"], typeof(int));
            }
            
            return iAccountId;
        }

        public async Task<object> GetCourseDetail(int iCourseId)
        {
            this.CommandToExecute = CanvasCommand.GET_COURSE_DETAIL;
            string sUrl = string.Concat(await CommandUrl(), "/", iCourseId);
            return await Execute(sUrl);
        }
        public async Task<object> GetCourseDiscussionsTopics(int iCourseId)
        {
            this.CommandToExecute = CanvasCommand.GET_COURSE_DISCUSSIONS_TOPICS;
            string sUrl = string.Format(await CommandUrl(), iCourseId);
            return await Execute(sUrl);
        }
        public async Task<object> GetCourseDiscussionEntries(int iCourseId, int iDiscussionTopicId)
        {
            this.CommandToExecute = CanvasCommand.GET_COURSE_DISCUSSION_ENTRIES;
            string sUrl = string.Format(await CommandUrl(), iCourseId, iDiscussionTopicId);
            return await Execute(sUrl);
        }
        public async Task<object> ReplyToTopicEntry(int iCourseId, int iDiscussionTopicId, int iEntryId, string sReply)
        {
            this.CommandToExecute = CanvasCommand.REPLY_TO_DISCUSSION_ENTRY;
            string sUrl = string.Format(await CommandUrl(), iCourseId, iDiscussionTopicId, iEntryId);
            sUrl = string.Concat(sUrl, "?1=1&message=", HttpUtility.UrlEncode(sReply));
            return await Execute(sUrl);
        }
        public async Task<object> MarkEntryAsRead(int iCourseId, int iDiscussionTopicId, int iEntryId)
        {
            this.CommandToExecute = CanvasCommand.MARK_ENTRY_AS_READ;
            string sUrl = string.Format(await CommandUrl(), iCourseId, iDiscussionTopicId, iEntryId);
            return await Execute(sUrl);
        }

        public async Task<object> CreateCourse(Courses.Add Add)
        {
            if(Add != null)
            { 
                List<KeyValuePair<string, string>> lstCommands = new List<KeyValuePair<string, string>>();
                lstCommands.Add(new KeyValuePair<string, string>("course[name]", Add.CourseName));
                lstCommands.Add(new KeyValuePair<string, string>("course[public_description]", Add.CourseDescription));
                if(Add.StartDate != null)
                {
                    lstCommands.Add(new KeyValuePair<string, string>("course[start_at]", Add.StartDate.Value.ToString("o")));
                }
                if (Add.EndDate != null)
                {
                    lstCommands.Add(new KeyValuePair<string, string>("course[end_at]", Add.EndDate.Value.ToString("o")));
                }
                lstCommands.Add(new KeyValuePair<string, string>("enroll_me", true.ToString().ToLower()));
                lstCommands.Add(new KeyValuePair<string, string>("course[is_public]", true.ToString().ToLower()));
                lstCommands.Add(new KeyValuePair<string, string>("course[self_enrollment]", true.ToString().ToLower()));
                return await Execute(CanvasCommand.CREATE_COURSE, lstCommands);
            }
            
            return null;
        }
        public async Task<object> UpdateCourse(Courses.Add Update, int iCourseId)
        {
            if (Update != null)
            {
                List<KeyValuePair<string, string>> lstCommands = new List<KeyValuePair<string, string>>();
                lstCommands.Add(new KeyValuePair<string, string>("course[name]", Update.CourseName));
                lstCommands.Add(new KeyValuePair<string, string>("course[public_description]", Update.CourseDescription));
                if (Update.StartDate != null)
                {
                    lstCommands.Add(new KeyValuePair<string, string>("course[start_at]", Update.StartDate.Value.ToString("o")));
                }
                if (Update.EndDate != null)
                {
                    lstCommands.Add(new KeyValuePair<string, string>("course[end_at]", Update.EndDate.Value.ToString("o")));
                }
                lstCommands.Add(new KeyValuePair<string, string>("enroll_me", true.ToString().ToLower()));
                lstCommands.Add(new KeyValuePair<string, string>("course[is_public]", true.ToString().ToLower()));
                lstCommands.Add(new KeyValuePair<string, string>("course[self_enrollment]", true.ToString().ToLower()));

                this.CommandToExecute = CanvasCommand.UPDATE_COURSE;
                string sUrl = await CommandUrl();
                sUrl = string.Format(sUrl, iCourseId);
                if (lstCommands != null && lstCommands.Count > 0)
                {
                    if (sUrl.EndsWith("/"))
                    {
                        sUrl = sUrl.Substring(0, sUrl.Length - 1);
                    }
                    sUrl = string.Concat(sUrl, "?1=1");
                    foreach (KeyValuePair<string, string> aCommand in lstCommands)
                    {
                        string sParamName = aCommand.Key;
                        string sParamValue = aCommand.Value;
                        if ((!string.IsNullOrEmpty(sParamName) && (!string.IsNullOrEmpty(sParamValue))))
                        {
                            sUrl = string.Concat(sUrl, "&", sParamName, "=", sParamValue);
                        }
                    }
                }
                return await Execute(sUrl);
            }

            return null;
        }
        public async Task<object> DeleteCourse(int iCourseId)
        {
            List<KeyValuePair<string, string>> lstCommands = new List<KeyValuePair<string, string>>();
            lstCommands.Add(new KeyValuePair<string, string>("event", "delete"));
            this.CommandToExecute = CanvasCommand.DELETE_COURSE;
            string sUrl = await CommandUrl();
            sUrl = string.Format(sUrl, iCourseId);
            if (lstCommands != null && lstCommands.Count > 0)
            {
                if (sUrl.EndsWith("/"))
                {
                    sUrl = sUrl.Substring(0, sUrl.Length - 1);
                }
                sUrl = string.Concat(sUrl, "?1=1");
                foreach (KeyValuePair<string, string> aCommand in lstCommands)
                {
                    string sParamName = aCommand.Key;
                    string sParamValue = aCommand.Value;
                    if ((!string.IsNullOrEmpty(sParamName) && (!string.IsNullOrEmpty(sParamValue))))
                    {
                        sUrl = string.Concat(sUrl, "&", sParamName, "=", sParamValue);
                    }
                }
            }
            return await Execute(sUrl);
        }

        public async Task<object> Execute(CanvasCommand cmd)
        {
            return await Execute(cmd, null);
        }
        public async Task<object> Execute(CanvasCommand cmd, List<KeyValuePair<string, string>> lstCommands)
        {
            this.CommandToExecute = cmd;
            string sUrl = await CommandUrl();
            if(lstCommands != null && lstCommands.Count > 0)
            {
                if (sUrl.EndsWith("/"))
                {
                    sUrl = sUrl.Substring(0, sUrl.Length - 1);
                }
                sUrl = string.Concat(sUrl, "?1=1");
                foreach (KeyValuePair<string, string> aCommand in lstCommands)
                {
                    string sParamName = aCommand.Key;
                    string sParamValue = aCommand.Value;
                    if ((!string.IsNullOrEmpty(sParamName) && (!string.IsNullOrEmpty(sParamValue))))
                    {
                        sUrl = string.Concat(sUrl, "&", sParamName, "=", sParamValue);
                    }
                }
            }
            return await Execute(sUrl);
        }
        private async Task<object> Execute(string CanvasUrl)
        {
            if(!string.IsNullOrEmpty(CanvasUrl))
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(CanvasUrl);
                request.Headers.Add("Authorization", string.Concat("Bearer ", this.CanvasAccessToken));
                request.Method = this.Method;
                string sResponse = string.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    List<HttpStatusCode> lstSuccessStatus = new List<HttpStatusCode>() { HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.NoContent };
                    if (lstSuccessStatus.Contains(response.StatusCode))
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(dataStream))
                            {
                                sResponse = await reader.ReadToEndAsync();
                                return sResponse;
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}