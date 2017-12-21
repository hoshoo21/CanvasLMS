using LMS.Models;
using OpenPop.Mime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LMS.Core
{
    public class FAQEmailScheduler
    {
        private const int DEFAULT_INTERVAL = 15;
        private const string DEFAULT_INTERVAL_UNIT = "s";

        private LMSEntities _db = null;
        private LMSEntities db
        {
            get
            {
                if(_db == null)
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

        private List<FAQEmail> _EmailsAccounts = null;
        private List<FAQEmail> EmailAccounts
        {
            get
            {
                if(_EmailsAccounts == null)
                {
                    _EmailsAccounts = (from fe in db.FAQEmails
                                       where ((fe.active == null) || (fe.active == true))
                                       select fe)
                                       .ToList();
                }
                return _EmailsAccounts;
            }
        }

        private FAQRespondLogger _FaqLogger = null;
        public FAQRespondLogger FaqLogger
        {
            get
            {
                if (_FaqLogger == null)
                {
                    _FaqLogger = new FAQRespondLogger();
                }
                return _FaqLogger;
            }
            set
            {
                _FaqLogger = value;
            }
        }

        public void InitSchedular()
        {
            if(EmailAccounts != null)
            {
                List<Task> lstTasks = new List<Task>();
                foreach(FAQEmail aEmail in EmailAccounts)
                {
                    Task aTask = new Task(() =>
                    {
                        SendAutoReply(aEmail);
                    });
                    aTask.Start();
                    lstTasks.Add(aTask);
                }
                Task.WaitAll(lstTasks.ToArray());
                Log(string.Format("FAQ auto reponder shall be recalled at {0}", DateTime.Now.AddMilliseconds(TimerInterval + 1000)));
                System.Threading.Thread.Sleep(Convert.ToInt32(TimerInterval));
                Task.Factory.StartNew(() => {
                    lstTasks = null;
                    InitSchedular();
                });
            }
        }

        private void Log(string sMessage)
        {
            FaqLogger.Log(sMessage);
        }

        private async Task<object> SendAutoReply(FAQEmail aEmail)
        {
            if(aEmail != null)
            {
                EmailDownloader aDownloader = new EmailDownloader(aEmail.faq_email_id);
                aDownloader.FaqLogger = this.FaqLogger;
                List<Message> UnreadEmails = await aDownloader.UnreadEmails();
                if(UnreadEmails != null && UnreadEmails.Count > 0)
                {
                    StringBuilder builder = null;
                    foreach (Message aUnreadEmail in UnreadEmails)
                    {
                        FAQEmailsDownload newEmail = new FAQEmailsDownload() {
                            download_date = DateTime.Now
                            , email_uid = aUnreadEmail.Headers.MessageId
                            , faq_email_id = aEmail.faq_email_id
                        };
                        this.db.FAQEmailsDownloads.Add(newEmail);
                        this.db.SaveChanges();

                        builder = new StringBuilder();
                        OpenPop.Mime.MessagePart plainText = aUnreadEmail.FindFirstPlainTextVersion();
                        if (plainText != null)
                        {
                            // We found some plaintext!
                            builder.Append(plainText.GetBodyAsText());
                        }
                        else
                        {
                            // Might include a part holding html instead
                            OpenPop.Mime.MessagePart html = aUnreadEmail.FindFirstHtmlVersion();
                            if (html != null)
                            {
                                // We found some html!
                                builder.Append(html.GetBodyAsText());
                            }
                        }
                        if(builder != null && !string.IsNullOrEmpty(builder.ToString()))
                        {
                            CanvasRespondRule answerFound = AnswerFinder.FindAnswer(AutoResponderRuleTypes.FAQs, builder.ToString());
                            if (answerFound != null)
                            {
                                SmtpPopSetting settings = (from s in db.SmtpPopSettings
                                                           where (s.smtp_pop_id == aEmail.smtp_pop_id)
                                                           select s
                                                           ).FirstOrDefault();
                                if (settings != null)
                                {
                                    string sEmailReceivedFrom = aUnreadEmail.Headers.From.MailAddress.Address;
                                    using (MailMessage mail = new MailMessage(aEmail.user_name, sEmailReceivedFrom))
                                    {
                                        using (SmtpClient client = new SmtpClient(settings.smtp_server, settings.smpt_port))
                                        {
                                            client.EnableSsl = settings.smtp_use_ssl;
                                            client.DeliveryMethod = SmtpDeliveryMethod.Network;
                                            client.UseDefaultCredentials = false;
                                            client.Credentials = new NetworkCredential(aEmail.user_name, aEmail.use_password);
                                            if (!builder.ToString().Trim().ToLower().StartsWith("re:"))
                                            {
                                                mail.Subject = string.Concat("RE: ", aUnreadEmail.Headers.Subject);
                                            }
                                            mail.Body = answerFound.CanvasAnswer;
                                            try
                                            {
                                                //client.SendCompleted += (s, e) =>
                                                //{

                                                //};
                                                //client.SendAsync(mail, null);
                                                client.Send(mail);
                                                Log(string.Format("Question '{0}' replied with answer '{1}' to email account '{2}'"
                                                    , builder.ToString(), answerFound.CanvasAnswer, sEmailReceivedFrom));
                                                AnsweredFAQ aAnswer = new AnsweredFAQ()
                                                {
                                                    answer_date_time = DateTime.Now
                                                    ,
                                                    answer_replied_with = answerFound.CanvasAnswer
                                                    ,
                                                    faq = builder.ToString()
                                                    ,
                                                    canvas_rule_id = answerFound.CanvasRuleId
                                                    ,
                                                    faq_email_id = aEmail.faq_email_id
                                                };
                                                this.db.AnsweredFAQs.Add(aAnswer);
                                                this.db.SaveChanges();
                                            }
                                            catch (Exception ex)
                                            {
                                                Log(string.Concat("EXCEPTION : ", ex.Message));
                                            }

                                        }
                                    }
                                }
                            }
                            else
                            {
                                Log(string.Format("No answer was found against FAQ '{0}'", builder.ToString()));
                                UnAnsweredFAQ unAnswered = new UnAnsweredFAQ() {
                                    faq = builder.ToString()
                                    , faq_email_id = aEmail.faq_email_id
                                    , to_email_address = aUnreadEmail.Headers.From.MailAddress.Address
                                    , faq_email_subject = aUnreadEmail.Headers.Subject
                                };
                                db.UnAnsweredFAQs.Add(unAnswered);
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}