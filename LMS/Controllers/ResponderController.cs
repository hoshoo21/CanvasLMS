using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Models;
using Newtonsoft.Json;

namespace LMS.Controllers
{
    public class ResponderController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: Responder
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SaveRules(AutoResponderModel objRules) {

            var autoResponsder = new AutoRespondingRule();
            autoResponsder.ConditionType = objRules.ConditionType;
            autoResponsder.MatchingText = objRules.MatchingText;
            autoResponsder.TextType = objRules.TextType;

            db.AutoRespondingRules.Add(autoResponsder);
            db.SaveChanges();

           return Json( autoResponsder.id,JsonRequestBehavior.AllowGet);
            
                 

        }
        public ActionResult GetRules()
        {
            var EnginerRules = (from rules in db.AutoRespondingRules select rules).ToList();
            return Json(EnginerRules, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult FaqAnswers(List<AutoResponderModel> objRules) {

            
            List<AutoResponderEmailModel> possibleAnswers = new List<AutoResponderEmailModel>();
            foreach (AutoResponderModel objrule in objRules) {
                if (objrule.ConditionType == 1 || objrule.ConditionType==3) {
                    var foundaswer = (from kf in db.KnowFaqs where kf.Question.Contains(objrule.MatchingText) select kf).ToList();
                    if (foundaswer != null && foundaswer.Count>0) {
                        foreach (KnowFaq knownaswer in foundaswer) {
                            possibleAnswers.Add(new AutoResponderEmailModel() { EmailSubject = "Re:" + knownaswer.Question, EmailBody = knownaswer.Answer });

                        }
                        
                    }


                }
                if (objrule.ConditionType == 2)
                {
                    var foundaswer = (from kf in db.KnowFaqs where kf.Question == objrule.MatchingText select kf).ToList();
                    if (foundaswer != null && foundaswer.Count > 0)
                    {
                        foreach (KnowFaq knownaswer in foundaswer)
                        {
                            possibleAnswers.Add(new AutoResponderEmailModel() { EmailSubject = "Re:" + knownaswer.Question, EmailBody = knownaswer.Answer });

                        }

                    }


                }



            }
            return Json(possibleAnswers, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DiscussionAnswers(List<AutoResponderModel> objRules)
        {


            List<AutoResponderEmailModel> possibleAnswers = new List<AutoResponderEmailModel>();
            foreach (AutoResponderModel objrule in objRules)
            {
                if (objrule.ConditionType == 1 || objrule.ConditionType == 3)
                {
                    var foundaswer = (from kf in db.KnownDiscussions where kf.Question.Contains(objrule.MatchingText) select kf).ToList();
                    if (foundaswer != null && foundaswer.Count > 0)
                    {
                        foreach (KnownDiscussion knownaswer in foundaswer)
                        {
                            possibleAnswers.Add(new AutoResponderEmailModel() { EmailSubject = "Re:" + knownaswer.Question, EmailBody = knownaswer.Answer });

                        }

                    }


                }
                if (objrule.ConditionType == 2)
                {
                    var foundaswer = (from kf in db.KnownDiscussions where kf.Question == objrule.MatchingText select kf).ToList();
                    if (foundaswer != null && foundaswer.Count > 0)
                    {
                        foreach (KnownDiscussion knownaswer in foundaswer)
                        {
                            possibleAnswers.Add(new AutoResponderEmailModel() { EmailSubject = "Re:" + knownaswer.Question, EmailBody = knownaswer.Answer });

                        }

                    }


                }



            }
            return Json(possibleAnswers, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveUnasweredFaq(string Question) {
            var unansfaq = (from ques in db.FAQs where ques.Question == Question select ques).FirstOrDefault();

            if (unansfaq == null) {
                unansfaq = new FAQ();
                unansfaq.FAQId = Guid.NewGuid();
                unansfaq.Question = Question;
                db.FAQs.Add(unansfaq);
                db.SaveChanges();

            }
            return Json("Question Moved", JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveUnasweredDiscussions(string Question)
        {
            var unansfaq = (from ques in db.DisucssionMasters where ques.DiscussionSubject == Question select ques).FirstOrDefault();

            if (unansfaq == null)
            {
                unansfaq = new DisucssionMaster();
                unansfaq.DiscussionId = Guid.NewGuid();
                unansfaq.DiscussionSubject = Question;
                db.DisucssionMasters.Add(unansfaq);
                db.SaveChanges();

            }
            return Json("Question Moved", JsonRequestBehavior.AllowGet);
        }

    }
}