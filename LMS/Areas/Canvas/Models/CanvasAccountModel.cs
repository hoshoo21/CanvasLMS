using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Areas.Canvas.Models
{
    public class CanvasAccountModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string workflow_state { get; set; }
        public object parent_account_id { get; set; }
        public object root_account_id { get; set; }
        public string uuid { get; set; }
        public int default_storage_quota_mb { get; set; }
        public int default_user_storage_quota_mb { get; set; }
        public int default_group_storage_quota_mb { get; set; }
        public string default_time_zone { get; set; }
    }
}