using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Areas.Canvas.Models
{
    public class CanvasAnsweringRules
    {        
        public byte MatchType { get; set; }
        public string Question { get; set; }
        public bool Active { get; set; }
    }
    public class CanvasResponderSaveModel
    {
        public int CanvasRuleId { get; set; }
        public byte RuleForId { get; set; }
        public string CanvasRuleTitle { get; set; }
        public string Answer { get; set; }
        public bool Active { get; set; }
        public List<CanvasAnsweringRules> AnsweringRules { get; set; }
    }
}