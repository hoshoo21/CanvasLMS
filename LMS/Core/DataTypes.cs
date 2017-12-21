using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace LMS.Core
{
    public enum AutoResponderRuleTypes
    {
        [Description("Discussion")]
        DISCUSSION = 2,
        [Description("FAQs")]
        FAQs = 3
    }

    public enum AutoResponderConditionType
    {
        [Description("Discussion")]
        UNKNOWN = 0
        ,
        [Description("Contains String")]
        CONTAINS_STRING = 1
        ,
        [Description("Match String")]
        MATCH_STRING = 2
        ,
        [Description("Contains Any Word in String")]
        CONTAINS_ANY_WORD = 3
    }
}