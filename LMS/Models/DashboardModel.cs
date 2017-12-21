using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public class DashboardModel
    {
        public List<Cours> Courses = new List<Cours>();
        public List<University> Universities = new List<University>();
    }
}