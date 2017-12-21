using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public class TopicParticipants
    {
        public Guid CourseId { get; set; }

        public Guid DiscussionId { get; set; }

        public string CourseName { get; set; }

        public string Discussion { get; set; }

        public string SubTopic { get; set; }

        public string ParticipantName { get; set; }


    }
}