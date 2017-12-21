using LMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LMS.Areas.Canvas.Models
{
    public class CanvasCourseDetailModel : BaseModel
    {
        private Guid? _CourseId = null;
        public Guid? CourseId
        {
            get
            {
                return _CourseId;
            }
            set
            {
                _CourseId = value;
                InitCourseDetail();
            }
        }
        [Required(ErrorMessage = "Course name is required")]
        public string CourseName { get; set; }
        public string CourseDescription { get; set; }

        private List<LMSProvider> _Providers = null;
        public List<LMSProvider> Providers
        {
            get
            {
                if(_Providers == null)
                {
                    _Providers = (from p in base.db.LMSProviders
                                  orderby p.ProviderName
                                  select p
                                  ).ToList();
                }
                return _Providers;
            }
        }

        private List<Guid> _SelectedProviders = null;
        //public List<Guid> SelectedProviders
        //{
        //    get
        //    {
        //        if(_SelectedProviders == null)
        //        {
        //            _SelectedProviders = new List<Guid>();
        //        }
        //        return _SelectedProviders;
        //    }
        //    set
        //    {
        //        _SelectedProviders = value;
        //    }
        //}
        public List<Guid> SelectedProviders { get; set; }

        private List<University> _Universities = null;
        public List<University> Universities
        {
            get
            {
                if(_Universities == null)
                {
                    _Universities = (from u in base.db.Universities
                                     orderby u.UniversityName
                                     select u)
                                     .ToList();
                }
                return _Universities;
            }
        }

        private List<Guid> _SelectedUniversities = null;
        //public List<Guid> SelectedUniversities
        //{
        //    get
        //    {
        //        if(_SelectedUniversities == null)
        //        {
        //            _SelectedUniversities = new List<Guid>();
        //        }
        //        return _SelectedUniversities;
        //    }
        //    set
        //    {
        //        _SelectedUniversities = value;
        //    }
        //}
        public List<Guid> SelectedUniversities { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        private void InitCourseDetail()
        {
            CourseName = null;
            CourseDescription = null;
            _SelectedProviders = null;
            _SelectedUniversities = null;
            StartDate = null;
            EndDate = null;

            if(CourseId != null) 
            {
                Cours aCourse = (from c in base.db.Courses
                           where (c.CourseId == this.CourseId)
                           select c).FirstOrDefault();
                if(aCourse != null)
                {
                    this.CourseName = aCourse.CourseName;
                    this.CourseDescription = aCourse.CourseDescription;

                    
                    this._SelectedProviders = (from cp in base.db.CourseProviders
                                              where (cp.CourseId == this.CourseId)
                                              select cp.ProviderId.Value)
                                              .ToList();


                    this._SelectedUniversities = (from cu in base.db.CourseUniversities
                                                  where (cu.CourseId == this.CourseId)
                                                  select cu.UniversityId.Value
                                                  ).ToList();

                    this.StartDate = (from cs in base.db.CourseSchedules
                                      where cs.CourseId == this.CourseId
                                      select cs.StartDate
                                      ).FirstOrDefault();
                    this.EndDate = (from cs in base.db.CourseSchedules
                                      where cs.CourseId == this.CourseId
                                      select cs.EndDate
                                      ).FirstOrDefault();
                }
            }
        }
    }
}