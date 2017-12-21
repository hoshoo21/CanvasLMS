using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Areas.Canvas.Models
{
    public class CanvasDashboardModel
    {
        private List<CanvasCourseModel> _Courses = null;
        public List<CanvasCourseModel> Courses
        {
            get
            {
                if(_Courses == null)
                {
                    _Courses = new List<CanvasCourseModel>();
                }
                return _Courses;
            }
            set
            {
                _Courses = value;
            }
        }
    }
}