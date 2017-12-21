using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public class ProviderUniversitiesModel
    {
        public LMSProvider Provider { get; set; }
        public List<University> Universities = new List<University>();
    }
}