using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public class AutoResponderModel
    {
        public int id { get; set; }
        public int? TextType { get; set; }
        public int? ConditionType { get; set; }

        public string MatchingText { get; set; }
             

        public Guid? QuestionId { get; set; } 
    }

    public class ResponseEmailRulesModel {

        public int EmailId { get; set; }
        public int RuleId { get; set; }
    }

    public class AutoResponderEmailModel {
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public  Boolean IsDraft { get; set; }
        public List<int> RulesId { get; set; }
        public  int EmailId { get; set; }

    }

    public class ForumObject
    {
        public string ForumTopic { get; set; }

        public string Forumreplies { get; set; }

        public string ForumLastPost { get; set; }

        public string Originator { get; set; }

        public int? TotalNummberofReplies { get; set; }

        public string LastRepliedBy { get; set; }

        public Boolean isPostReplies { get; set; }

        public string MainPostContent { get; set; }

        public string URL { get; set; }

        public List<SubForumReplies> forumReplies = new List<SubForumReplies>();



    }

    public class SubForumReplies
    {
        public string ForumPost { get; set; }

        public Guid? SubForumId { get; set; }


    }

}

