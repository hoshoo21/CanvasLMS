
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace LMS.Models
{

using System;
    using System.Collections.Generic;
    
public partial class AnsweredFAQ
{

    public int faq_answered_id { get; set; }

    public Nullable<int> faq_email_id { get; set; }

    public string faq { get; set; }

    public Nullable<System.DateTime> answer_date_time { get; set; }

    public string answer_replied_with { get; set; }

    public Nullable<int> canvas_rule_id { get; set; }

}

}
