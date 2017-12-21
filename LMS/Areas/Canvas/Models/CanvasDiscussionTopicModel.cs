using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Areas.Canvas.Models
{   
    public class User
    {
        public int id { get; set; }
        public string display_name { get; set; }
        public string avatar_image_url { get; set; }
        public string html_url { get; set; }
    }

    public class CanvasDiscussionTopicEntriesModel
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public object parent_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public object rating_count { get; set; }
        public object rating_sum { get; set; }
        public string user_name { get; set; }
        public string message { get; set; }
        public User user { get; set; }
        public string read_state { get; set; }
        public bool forced_read_state { get; set; }
    }
}