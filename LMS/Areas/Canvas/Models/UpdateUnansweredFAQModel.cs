using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Areas.Canvas.Models
{
    public class UpdateUnansweredFAQModel
    {
        public int unanswered_faq_id { get; set; }
        public string faq_answer { get; set; }
    }
}