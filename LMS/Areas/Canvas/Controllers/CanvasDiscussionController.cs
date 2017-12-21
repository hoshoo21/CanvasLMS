using LMS.Areas.Canvas.Models;
using LMS.Core;
using LMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;

namespace LMS.Areas.Canvas.Controllers
{
    public class CanvasDiscussionController : BaseController
    {
        // GET: Canvas/Discussion
        public ActionResult Index()
        {
            CanvasUnansweredModel model = new CanvasUnansweredModel();
            model.UnAnsweredFAQs = (from faq in base.db.UnAnsweredFAQs
                                    where (faq.faq_answer == null)
                                    select faq)
                     .ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateUnansweredFAQ(UpdateUnansweredFAQModel model)
        {
            if (model != null)
            {
                UnAnsweredFAQ faq = (from f in base.db.UnAnsweredFAQs
                                     where (f.unanswered_faq_id == model.unanswered_faq_id)
                                     select f)
                                     .FirstOrDefault();
                if(faq != null)
                {
                    faq.faq_answer = model.faq_answer;
                    base.db.Entry(faq).State = System.Data.Entity.EntityState.Modified;
                    base.db.SaveChanges();

                    Task.Factory.StartNew(() =>
                    {
                        SendFAQReply(faq.faq, model.faq_answer, faq.faq_email_id, faq.to_email_address, faq.faq_email_subject);
                    });

                    return base.SuccessMessage(string.Empty);
                }
            }
            return base.NoSuccessMessage(Constants.ERROR_IN_EXECUTION);
        }

        #region Helper Methods
        private void SendFAQReply(string sFAQ, string sFaqAnswer, int? iFaqEmailId, string sToEmailAddress, string sSubject)
        {
            if((!string.IsNullOrEmpty(sFaqAnswer)) && (iFaqEmailId != null) && (!string.IsNullOrEmpty(sToEmailAddress)))
            {
                sSubject = Regex.Replace(sSubject, @"\t|\n|\r", ""); //sSubject.Replace(@"\r\n", string.Empty);
                var faqEmail = (from fe in base.db.FAQEmails
                                where (fe.faq_email_id == iFaqEmailId.Value)
                                select fe)
                                .FirstOrDefault();
                if(faqEmail != null)
                {
                    var settings = (from s in base.db.SmtpPopSettings
                                    where (s.smtp_pop_id == faqEmail.smtp_pop_id)
                                    select s)
                                    .FirstOrDefault();
                    if(settings != null)
                    {
                        using (MailMessage mail = new MailMessage(faqEmail.user_name, sToEmailAddress))
                        {
                            using (SmtpClient client = new SmtpClient(settings.smtp_server, settings.smpt_port))
                            {
                                client.EnableSsl = settings.smtp_use_ssl;
                                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                                client.UseDefaultCredentials = false;
                                client.Credentials = new NetworkCredential(faqEmail.user_name, faqEmail.use_password);
                                if (!sSubject.ToString().Trim().ToLower().StartsWith("re:"))
                                {
                                    mail.Subject = string.Concat("RE: ", sSubject);
                                }
                                mail.Body = sFaqAnswer;
                                try
                                {
                                    client.Send(mail);
                                    AnsweredFAQ aAnswer = new AnsweredFAQ()
                                    {
                                        answer_date_time = DateTime.Now
                                        ,
                                        answer_replied_with = sFaqAnswer
                                        ,
                                        faq = sFAQ
                                        ,
                                        canvas_rule_id = null
                                        ,
                                        faq_email_id = faqEmail.faq_email_id
                                    };
                                    this.db.AnsweredFAQs.Add(aAnswer);
                                    this.db.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    //Log(string.Concat("EXCEPTION : ", ex.Message));
                                }

                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}