using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Configuration;
using LMS.Core;
using System.Threading.Tasks;
using LMS.Areas.Canvas.Models;
using LMS.Models;
using System.Text.RegularExpressions;

namespace LMS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private const int DEFAULT_INTERVAL = 15;
        private const string DEFAULT_INTERVAL_UNIT = "s";

        CanvasAutoRespondLogger _canvasLogger = null;
        CanvasAutoRespondLogger canvasLogger
        {
            get
            {
                if (_canvasLogger == null)
                {
                    _canvasLogger = new CanvasAutoRespondLogger();
                }
                return _canvasLogger;
            }
        }

        private CanvasManager _cm = null;
        private CanvasManager cm
        {
            get
            {
                if (_cm == null)
                {
                    _cm = new CanvasManager();
                }
                return _cm;
            }
        }

        private LMSEntities _db = null;
        protected LMSEntities db
        {
            get
            {
                if (_db == null)
                {
                    _db = new LMSEntities();
                }
                return _db;
            }
        }

        private static double Intervals
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["Interval"]))
                {
                    double iIntervals = 0;
                    if (double.TryParse(ConfigurationManager.AppSettings["Interval"], out iIntervals))
                    {
                        if (iIntervals > 0)
                        {
                            return iIntervals;
                        }
                    }
                }
                return DEFAULT_INTERVAL;
            }
        }
        private static string IntervalUnit
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["IntervalUnit"]))
                {
                    List<string> lstValidUnits = new List<string>() { "ms", "s", "m", "h" };
                    string sIntervalUnit = ConfigurationManager.AppSettings["IntervalUnit"];
                    if (lstValidUnits.Contains(sIntervalUnit.Trim().ToLower()))
                    {
                        return sIntervalUnit.Trim().ToLower();
                    }
                }
                return DEFAULT_INTERVAL_UNIT;
            }
        }
        private static double TimerInterval
        {
            get
            {
                double iTimerInterval = Intervals;
                switch (IntervalUnit.Trim().ToLower())
                {
                    case "ms":
                        break;

                    case "s":
                        iTimerInterval = TimeSpanUtil.ConvertSecondsToMilliseconds(iTimerInterval);
                        break;

                    case "m":
                        iTimerInterval = TimeSpanUtil.ConvertMinutesToMilliseconds(iTimerInterval);
                        break;

                    case "h":
                        iTimerInterval = TimeSpanUtil.ConvertHoursToMilliseconds(iTimerInterval);
                        break;
                }
                return iTimerInterval;
            }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Task.Factory.StartNew(() => {
                ExecuteCanvasAutoResponder();
            });
            Task.Factory.StartNew(() => {
                new FAQEmailScheduler().InitSchedular();
            });

        }

        private async Task<ActionResult> ExecuteCanvasAutoResponder()
        {
            try
            {                
                canvasLogger.CanvasResponderInitiated();
                canvasLogger.Log("Getting Courses...");
                string sCourses = await cm.Execute(CanvasCommand.GET_COURSE_LIST) as string;
                if (sCourses != null)
                {
                    List<Task> lstTasks = new List<Task>();
                    List<CanvasCourseModel> allCourses = sCourses.FromJson<List<CanvasCourseModel>>();
                    if (allCourses != null && allCourses.Count > 0)
                    {
                        canvasLogger.Log(string.Concat(allCourses.Count, " courses found."));
                        foreach (var aCourse in allCourses)
                        {
                            //Task aTask = Task.Factory.StartNew(() => {
                            //    GetCourseDiscussion(aCourse);
                            //});
                            Task aTask = new Task(() => {
                                GetCourseDiscussion(aCourse);
                            });
                            aTask.Start();
                            lstTasks.Add(aTask);
                        }
                    }
                    Task.WaitAll(lstTasks.ToArray());
                    canvasLogger.Log(string.Format("Canvas auto reponsder cycle completed at {0}.", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss")));
                }
                canvasLogger.Log(string.Format("Canvas auto reponder shall be recalled at {0}", DateTime.Now.AddMilliseconds(TimerInterval + 1000)));
                System.Threading.Thread.Sleep(Convert.ToInt32(TimerInterval));
                Task.Factory.StartNew(() => {
                    ExecuteCanvasAutoResponder();
                });

            }
            catch (Exception ex)
            {
                ExecuteCanvasAutoResponder();
            }
            return null;
        }
        private async Task<object> GetCourseDiscussion(CanvasCourseModel course)
        {
            if (course != null)
            {
                canvasLogger.Log(string.Format("Getting discussions for subject {0}.", course.name));
                string sResponse = await this.cm.GetCourseDiscussionsTopics(course.id) as string;
                if (!string.IsNullOrEmpty(sResponse))
                {
                    List<CanvasCourseDiscussionTopicModel> lstDiscussions = sResponse.FromJson<List<CanvasCourseDiscussionTopicModel>>();
                    if (lstDiscussions != null && lstDiscussions.Count > 0)
                    {
                        canvasLogger.Log(string.Format("{0} discussion(s) found against course {1}.", lstDiscussions.Count, course.name));
                        List<Task> lstDiscussionEntries = new List<Task>();
                        foreach (var aDiscussion in lstDiscussions)
                        {
                            Task aTask = new Task(() => {
                                ProcessDiscussionTopicEntries(course, aDiscussion);
                            });
                            lstDiscussionEntries.Add(aTask);
                            aTask.Start();
                        }
                        Task.WaitAll(lstDiscussionEntries.ToArray());
                    }
                    else
                    {
                        canvasLogger.Log(string.Format("No discussion(s) found against course {0}.", course.name));
                    }
                }
            }
            return null;
        }

        private async Task<object> ProcessDiscussionTopicEntries(CanvasCourseModel oCourse, CanvasCourseDiscussionTopicModel oDiscussionTopics)
        {
            if (oCourse != null && oDiscussionTopics != null)
            {
                if (oDiscussionTopics.unread_count > 0)
                {
                    string sTopicsEntries = await cm.GetCourseDiscussionEntries(oCourse.id, oDiscussionTopics.id) as string;
                    if (!string.IsNullOrEmpty(sTopicsEntries))
                    {
                        List<CanvasDiscussionTopicEntriesModel> lstTopicEntries = sTopicsEntries.FromJson<List<CanvasDiscussionTopicEntriesModel>>();
                        if (lstTopicEntries != null && lstTopicEntries.Count > 0)
                        {
                            int iTopicsCount = lstTopicEntries.Count;
                            canvasLogger.Log(string.Format("Discussion topic '{0}' of '{1}' has {2} Entrie(s).", oDiscussionTopics.title, oCourse.name, lstTopicEntries.Count));
                            lstTopicEntries = (from t in lstTopicEntries
                                         where (t.read_state == "unread")
                                         select t)
                                     .ToList();
                            if (lstTopicEntries.Count > 0)
                            {
                                canvasLogger.Log(string.Format("Total unread entrie(s) for discussion topic '{0}' of course {1} : {2}", oDiscussionTopics.title, oCourse.name, lstTopicEntries.Count));
                                List<int> lstDiscussionRules = (from dr in db.CanvasRespondRules
                                                                where (dr.CanvasRuleForId == ((int)AutoResponderRuleTypes.DISCUSSION)
                                                                && (dr.Active != null && dr.Active.Value == true))
                                                                select dr.CanvasRuleId)
                                                                .ToList();
                                if(lstDiscussionRules.Count > 0) /* If any discussion rule found */
                                {
                                    List<CanvasRespondRuleQuestion> lstQuestions = (from q in db.CanvasRespondRuleQuestions
                                                                                    where (q.Active != null && q.Active.Value == true)
                                                                                    && lstDiscussionRules.Contains(q.CanvasRuleId)
                                                                                    select q)
                                                                                    .ToList();
                                    if(lstQuestions != null && lstQuestions.Count > 0) /* If any discussion question found */
                                    {
                                        foreach(var aEntry in lstTopicEntries)
                                        {
                                            if(string.IsNullOrEmpty(aEntry.message))
                                            {
                                                continue;
                                            }
                                            
                                            bool isAnswerFound = false;
                                            foreach (var aQuestion in lstQuestions)
                                            {
                                                CanvasAnsweringRules aAnswer = null;
                                                if (string.IsNullOrEmpty(aQuestion.CanvasQuestion))
                                                {
                                                    continue;
                                                }
                                                if (aQuestion.MatchType != null)
                                                {
                                                    if (Enum.IsDefined(typeof(AutoResponderConditionType), (int)aQuestion.MatchType.Value))
                                                    {
                                                        
                                                        AutoResponderConditionType condition = (AutoResponderConditionType)aQuestion.MatchType.Value;
                                                        string sPlainText = RemoveHtmlTags(aEntry.message);
                                                        if(!string.IsNullOrEmpty(sPlainText))
                                                        {
                                                            switch (condition)
                                                            {
                                                                case AutoResponderConditionType.CONTAINS_ANY_WORD:

                                                                    break;

                                                                case AutoResponderConditionType.CONTAINS_STRING:
                                                                    isAnswerFound = sPlainText.Trim().ToLower().IndexOf(aQuestion.CanvasQuestion.Trim().ToLower()) != -1;
                                                                    break;

                                                                case AutoResponderConditionType.MATCH_STRING:
                                                                    isAnswerFound = sPlainText.Trim().ToLower().Equals(aQuestion.CanvasQuestion.Trim().ToLower(), StringComparison.InvariantCultureIgnoreCase);
                                                                    break;
                                                            }
                                                        }
                                                        if (isAnswerFound)
                                                        {                                                            
                                                            var answerFound = (from a in db.CanvasRespondRules
                                                                               where a.CanvasRuleId == aQuestion.CanvasRuleId
                                                                               select a)
                                                                               .FirstOrDefault();
                                                            if (answerFound != null && (!string.IsNullOrEmpty(answerFound.CanvasAnswer)))
                                                            {
                                                                canvasLogger.Log(string.Format("Marking entry '{0}' as read.", aEntry.message));
                                                                await cm.MarkEntryAsRead(oCourse.id, oDiscussionTopics.id, aEntry.id);
                                                                canvasLogger.Log(string.Format("Entry '{0}' marked as read.", aEntry.message));

                                                                canvasLogger.Log(string.Format("Replying to question '{0}' with answer '{1}'.", aEntry.message, answerFound.CanvasAnswer));
                                                                string sResponse = await cm.ReplyToTopicEntry(oCourse.id, oDiscussionTopics.id, aEntry.id, answerFound.CanvasAnswer) as string;
                                                                canvasLogger.Log(string.Format("Question '{0}' replied with answer '{1}'.", aEntry.message, answerFound.CanvasAnswer));
                                                            }
                                                            break;
                                                        }
                                                    }
                                                        
                                                }
                                            }
                                        }
                                        
                                    }
                                }
                            }
                            else
                            {
                                canvasLogger.Log(string.Format("No unread topic(s) for discussion '{0}' of course {1} was found", oDiscussionTopics.title, oCourse.name));
                            }
                        }
                    }
                }

            }

            return null;
        }

        public string RemoveHtmlTags(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
    }
}
