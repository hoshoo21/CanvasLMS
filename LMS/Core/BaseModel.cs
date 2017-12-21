using LMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Core
{
    public class BaseModel
    {
        private LMSEntities _db = null;
        protected LMSEntities db
        {
            get
            {
                if(_db == null)
                {
                    _db = new LMSEntities();
                }
                return _db;
            }
        }
    }
}