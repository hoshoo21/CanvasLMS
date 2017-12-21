using LMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace LMS.Core
{
    public static class AnswerFinder
    {
        public static string RemoveHtmlTags(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
        public static CanvasRespondRule FindAnswer(AutoResponderRuleTypes answerType, string sQuestion)
        {
            CanvasRespondRule answerFound = null;
            if(!string.IsNullOrEmpty(sQuestion))
            {
                using (LMSEntities db = new LMSEntities())
                {
                    List<int> lstDiscussionRules = (from dr in db.CanvasRespondRules
                                                    where (dr.CanvasRuleForId == ((int)answerType)
                                                    && (dr.Active != null && dr.Active.Value == true))
                                                    select dr.CanvasRuleId)
                                                                    .ToList();
                    if(lstDiscussionRules != null && lstDiscussionRules.Count > 0)
                    {
                        List<CanvasRespondRuleQuestion> lstQuestions = (from q in db.CanvasRespondRuleQuestions
                                                                        where (q.Active != null && q.Active.Value == true)
                                                                        && lstDiscussionRules.Contains(q.CanvasRuleId)
                                                                        select q)
                                                                                    .ToList();
                        if (lstQuestions != null && lstQuestions.Count > 0)
                        {
                            bool isAnswerFound = false;
                            foreach (var aQuestion in lstQuestions)
                            {
                                if (string.IsNullOrEmpty(aQuestion.CanvasQuestion))
                                {
                                    continue;
                                }
                                if (aQuestion.MatchType != null)
                                {
                                    if (Enum.IsDefined(typeof(AutoResponderConditionType), (int)aQuestion.MatchType.Value))
                                    {
                                        AutoResponderConditionType condition = (AutoResponderConditionType)aQuestion.MatchType.Value;
                                        string sPlainText = RemoveHtmlTags(sQuestion);
                                        if (!string.IsNullOrEmpty(sPlainText))
                                        {
                                            switch (condition)
                                            {
                                                case AutoResponderConditionType.CONTAINS_ANY_WORD:

                                                    break;

                                                case AutoResponderConditionType.CONTAINS_STRING:
                                                    isAnswerFound = sPlainText.Trim().ToLower().IndexOf(aQuestion.CanvasQuestion.Trim().ToLower()) != -1;
                                                    break;

                                                case AutoResponderConditionType.MATCH_STRING:
                                                    isAnswerFound = sPlainText.Trim().ToLower().Equals(aQuestion.CanvasQuestion.Trim().ToLower());
                                                    break;
                                            }
                                            if (isAnswerFound)
                                            {
                                                answerFound = (from a in db.CanvasRespondRules
                                                                   where a.CanvasRuleId == aQuestion.CanvasRuleId
                                                                   select a)
                                                                   .FirstOrDefault();
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }                    
            }
            return answerFound;
        }
    }
}