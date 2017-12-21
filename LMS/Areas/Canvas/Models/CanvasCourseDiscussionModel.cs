using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Areas.Canvas.Models
{
    public class Permissions
    {
        public bool? attach { get; set; }
        public bool? update { get; set; }
        public bool? reply { get; set; }
        public bool? delete { get; set; }
    }

    public class Author
    {
        public int id { get; set; }
        public string display_name { get; set; }
        public string avatar_image_url { get; set; }
        public string html_url { get; set; }
    }

    public class CanvasCourseDiscussionTopicModel
    {
        public int id { get; set; }
        public string title { get; set; }
        public DateTime? last_reply_at { get; set; }
        public DateTime? delayed_post_at { get; set; }
        public DateTime? posted_at { get; set; }
        public object assignment_id { get; set; }
        public object root_topic_id { get; set; }
        public object position { get; set; }
        public bool? podcast_has_student_posts { get; set; }
        public string discussion_type { get; set; }
        public DateTime? lock_at { get; set; }
        public bool? allow_rating { get; set; }
        public bool? only_graders_can_rate { get; set; }
        public bool? sort_by_rating { get; set; }
        public string user_name { get; set; }
        public int? discussion_subentry_count { get; set; }
        public Permissions permissions { get; set; }
        public object require_initial_post { get; set; }
        public bool? user_can_see_posts { get; set; }
        public object podcast_url { get; set; }
        public string read_state { get; set; }
        public int? unread_count { get; set; }
        public bool? subscribed { get; set; }
        public List<object> topic_children { get; set; }
        public List<object> attachments { get; set; }
        public bool? published { get; set; }
        public bool? can_unpublish { get; set; }
        public bool? locked { get; set; }
        public bool? can_lock { get; set; }
        public bool? comments_disabled { get; set; }
        public Author author { get; set; }
        public string html_url { get; set; }
        public string url { get; set; }
        public bool? pinned { get; set; }
        public object group_category_id { get; set; }
        public bool? can_group { get; set; }
        public bool? locked_for_user { get; set; }
        public string message { get; set; }
    }
}