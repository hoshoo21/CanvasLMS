using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public class Courses
    {
        public class Add
        {
            public System.Guid CourseId { get; set; }
            public string CourseName { get; set; }
            public string CourseDescription { get; set; }
            public Guid?[] UniversityId { get; set; }
            public Guid?[] ProviderId { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }


        public class CourseDetail {
            public string UniversityName { get; set; }
            public string LMSProvider { get; set; }

            public Guid? CourseId { get; set; }

        }
    }
}