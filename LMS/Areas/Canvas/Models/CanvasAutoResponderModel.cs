using LMS.Core;
using LMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMS.Areas.Canvas.Models
{
    
    public class CanvasAutoResponderModel
    {
        public int CanvasRuleId { get; set; }

        public int CanvasRuleFor { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(1000, ErrorMessage = "Name cannot be longer than 1000 characters.")]
        public string CanvasRuleTitle { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select rule type.")]
        [Display(Name = "Rule For")]
        public string RuleFor { get; set; }

        [Display(Name = "Matching Condition")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select matching condition.")]
        public string MatchType { get; set; }

        [Display(Name = "Question")]
        [Required(ErrorMessage = "Question is required.")]
        [StringLength(1000, ErrorMessage = "Name cannot be longer than 1000 characters.")]
        public string Question { get; set; }

        [Display(Name = "Answer")]
        [Required(ErrorMessage = "Answer is required.")]
        public string Answer { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

        public List<CanvasRespondRuleQuestion> lstQuestions { get; set; }

        private List<SelectListItem> _RuleTypes = null;
        public List<SelectListItem> RuleTypes
        {
            get
            {
                if(_RuleTypes == null)
                {
                    _RuleTypes = new List<SelectListItem>() {
                        new SelectListItem{ Text="Discussion", Value = "2" }
                        , new SelectListItem{ Text="FAQs", Value = "3" }
                    };
                    _RuleTypes = (from rt in _RuleTypes
                                  orderby rt.Text
                                  select rt)
                                  .ToList();
                    _RuleTypes.Insert(0, new SelectListItem() { Text = "Select Rule", Value = "0" });

                    SelectListItem aItem = (from rt in _RuleTypes
                                            where (rt.Value == this.CanvasRuleFor.ToString())
                                            select rt)
                                            .FirstOrDefault();
                    if(aItem != null)
                    {
                        aItem.Selected = true;
                    }               
                }
                return _RuleTypes;
            }
        }

        private List<SelectListItem> _ConditionTypes = null;
        public List<SelectListItem> ConditionTypes
        {
            get
            {
                if(_ConditionTypes == null)
                {
                    _ConditionTypes = new List<SelectListItem>() {
                        new SelectListItem{ Text="Contains String", Value = "1" }
                        , new SelectListItem{ Text="Match String", Value = "2" }
                        , new SelectListItem{ Text="Contains Any Word in String", Value = "3" }
                    };
                    _ConditionTypes = (from ct in _ConditionTypes
                                       orderby ct.Text
                                       select ct)
                                       .ToList();
                    _ConditionTypes.Insert(0, new SelectListItem() { Text = "Select Condition", Value = "0" });
                }
                return _ConditionTypes;
            }
        }
    }
}