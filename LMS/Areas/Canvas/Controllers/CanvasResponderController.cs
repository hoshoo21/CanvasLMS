using LMS.Areas.Canvas.Models;
using LMS.Core;
using LMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMS.Areas.Canvas.Controllers
{
    public class CanvasResponderController : BaseController
    {
        // GET: Canvas/CanvasResponder
        public ActionResult Index()
        {
            return View(new CanvasAutoResponderModel());
        }

        public ActionResult AutoRespondRules()
        {
            List<CanvasRespondRule> rules = null;
            rules = (from r in base.db.CanvasRespondRules
                     orderby r.CanvasRuleTitle
                     where (r.Active == null || r.Active == true)
                     select r)
                     .ToList();
            return View(rules);
        }

        public ActionResult RuleDetails(int? id)
        {
            CanvasAutoResponderModel model = new CanvasAutoResponderModel();
            if(id != null && id > 0)
            {
                CanvasRespondRule aRule = (from r in base.db.CanvasRespondRules
                                           where (r.CanvasRuleId == id)
                                           select r)
                                           .FirstOrDefault();
                if(aRule != null)
                {
                    model.CanvasRuleId = aRule.CanvasRuleId;
                    model.CanvasRuleFor = aRule.CanvasRuleForId == null ? ((byte)0) : aRule.CanvasRuleForId.Value;
                    model.CanvasRuleTitle = aRule.CanvasRuleTitle;
                    model.Answer = aRule.CanvasAnswer;
                    model.Active = aRule.Active == null ? true : aRule.Active.Value;
                    model.lstQuestions = (from q in base.db.CanvasRespondRuleQuestions
                                                                    orderby q.CanvasQuestion
                                                                    where (q.CanvasRuleId == aRule.CanvasRuleId)
                                                                    && (q.Active == null || q.Active == true)
                                                                    select q)
                                                                    .ToList();
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRule(int ruleId)
        {
            if(ruleId > 0)
            {
                CanvasRespondRule aRule = (from r in base.db.CanvasRespondRules
                                              where (r.CanvasRuleId == ruleId)
                                              select r)
                                              .FirstOrDefault();
                if(aRule != null)
                {
                    aRule.Active = false;
                    db.Entry(aRule).State = EntityState.Modified;
                    db.SaveChanges();
                    return base.SuccessMessage(string.Empty);
                }
                return base.NoSuccessMessage(Constants.ERROR_IN_EXECUTION);
            }
            return Json(new { Success = false, Message = Constants.ERROR_IN_EXECUTION }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteQuestion(int questionId)
        {
            if(questionId > 0)
            {
                var aQuestion = (from q in base.db.CanvasRespondRuleQuestions
                                 where (q.CanvasQuestionId == questionId)
                                 select q)
                                 .FirstOrDefault();
                if(aQuestion != null)
                {
                    aQuestion.Active = false;
                    base.db.Entry(aQuestion).State = EntityState.Modified;
                    base.db.SaveChanges();
                    return base.SuccessMessage(string.Empty);
                }
            }
            return base.NoSuccessMessage(Constants.ERROR_IN_EXECUTION);
        }
        public ActionResult RuleDetails1(int? id)
        {
            CanvasResponderSaveModel model = new CanvasResponderSaveModel();
            model.AnsweringRules = new List<CanvasAnsweringRules>();
            if (id != null && id > 0)
            {
                CanvasRespondRule aRule = (from r in base.db.CanvasRespondRules
                                           where (r.CanvasRuleId == id)
                                           select r)
                                           .FirstOrDefault();
                if (aRule != null)
                {
                    List<CanvasRespondRuleQuestion> lstQuestions = (from q in base.db.CanvasRespondRuleQuestions
                                                                    orderby q.CanvasQuestion
                                                                    where (q.Active == null || q.Active == true)
                                                                    select q)
                                                                    .ToList();
                    model.Active = Convert.ToBoolean(aRule.Active);
                    model.Answer = aRule.CanvasAnswer;
                    model.RuleForId = aRule.CanvasRuleForId == null ? ((byte)0) : aRule.CanvasRuleForId.Value;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCanvasRules(CanvasResponderSaveModel model)
        {
            if (model != null)
            {
                if (string.IsNullOrEmpty(model.CanvasRuleTitle))
                {
                    return Json(new { Success = false, Message = "Title is required." }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(model.Answer))
                {
                    return Json(new { Success = false, Message = "Answer is required." }, JsonRequestBehavior.AllowGet);
                }
                if(model.CanvasRuleId <= 0 && (model.AnsweringRules == null || model.AnsweringRules.Count <= 0))
                {
                    return Json(new { Success = false, Message = "Atleast 1 rule is required to save." }, JsonRequestBehavior.AllowGet);
                }

                if (model.CanvasRuleId <= 0)
                {                    
                    return AddNewRule(model);
                }
                else
                {
                    return UpdateRule(model);
                }
            }
            return Json(new { Success = false, Message = Constants.ERROR_IN_EXECUTION }, JsonRequestBehavior.AllowGet);
        }

        #region Helping Methods
        private ActionResult AddNewRule(CanvasResponderSaveModel model)
        {
            if(model != null)
            {
                CanvasRespondRule aAnswer = new CanvasRespondRule()
                {
                    CanvasAnswer = model.Answer
                    ,
                    CreatedOn = DateTime.Now
                    ,
                    CanvasRuleForId = model.RuleForId
                    ,
                    Active = true
                    ,
                    CanvasRuleId = model.CanvasRuleId
                    ,
                    CanvasRuleTitle = model.CanvasRuleTitle
                };
                base.db.CanvasRespondRules.Add(aAnswer);
                try
                {
                    base.db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
                
                if (aAnswer.CanvasRuleId > 0)
                {
                    if (model.AnsweringRules != null && model.AnsweringRules.Count > 0)
                    {
                        foreach (var aQuestion in model.AnsweringRules)
                        {
                            CanvasRespondRuleQuestion aRule = new CanvasRespondRuleQuestion()
                            {
                                Active = true
                                ,
                                CanvasQuestion = aQuestion.Question
                                ,
                                CanvasRuleId = aAnswer.CanvasRuleId
                                ,
                                CreatedOn = DateTime.Now
                                ,
                                MatchType = aQuestion.MatchType
                            };
                            base.db.CanvasRespondRuleQuestions.Add(aRule);
                        }
                        base.db.SaveChanges();
                        return Json(new { Success = true, Message = string.Empty }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new { Success = false, Message = Constants.ERROR_IN_EXECUTION }, JsonRequestBehavior.AllowGet);
        }
        private ActionResult UpdateRule(CanvasResponderSaveModel model)
        {
            if(model != null)
            {
                CanvasRespondRule aRule = (from r in base.db.CanvasRespondRules
                                           where (r.CanvasRuleId == model.CanvasRuleId)
                                           select r)
                                           .FirstOrDefault();
                if(aRule != null)
                {
                    //aRule.Active = model.Active;
                    aRule.CanvasAnswer = model.Answer;
                    aRule.CanvasRuleForId = model.RuleForId;
                    aRule.CanvasRuleTitle = aRule.CanvasRuleTitle;
                    aRule.UpdatedOn = DateTime.Now;

                    db.Entry(aRule).State = System.Data.Entity.EntityState.Modified;
                    int iRowCount = db.SaveChanges();
                    if(iRowCount > 0)
                    {
                        if (model.AnsweringRules != null && model.AnsweringRules.Count > 0)
                        {
                            foreach (var aQuestion in model.AnsweringRules)
                            {
                                CanvasRespondRuleQuestion newQuestion = new CanvasRespondRuleQuestion()
                                {
                                    Active = true
                                    ,
                                    CanvasQuestion = aQuestion.Question
                                    ,
                                    CanvasRuleId = aRule.CanvasRuleId
                                    ,
                                    CreatedOn = DateTime.Now
                                    ,
                                    MatchType = aQuestion.MatchType
                                };
                                base.db.CanvasRespondRuleQuestions.Add(newQuestion);
                            }
                            base.db.SaveChanges();
                            
                        }
                        return Json(new { Success = true, Message = string.Empty }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new { Success = false, Message = Constants.ERROR_IN_EXECUTION }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}