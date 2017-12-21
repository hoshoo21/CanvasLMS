using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public class ProdiverUnviersityModel
    {
        public Guid UniversityId { get; set; }
       

        public string UniversityName { get; set; }
        public Guid?[] ProviderId { get; set; }

        public string UniversityDescription { get; set; }
 
    }
}