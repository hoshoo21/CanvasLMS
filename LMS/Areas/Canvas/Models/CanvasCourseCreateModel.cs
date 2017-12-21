using LMS.Core;
using LMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS.Areas.Canvas.Models
{
    public class CanvasUniversityModel : University
    {
        public bool IsSelected { get; set; }
    }

    public class CanvasProviderModel : LMSProvider
    {
        public bool IsSelected { get; set; }
    }

    public class CanvasCourseCreateModel : BaseModel
    {
        public int course_id { get; set; }
        public string course_name { get; set; }
        public string course_code { get; set; }
        public DateTime? start_at { get; set; }
        public DateTime? end_at { get; set; }
        public int account_id { get; set; }

        private List<CanvasUniversityModel> _Universities = null;
        public List<CanvasUniversityModel> Universities
        {
            get
            {
                if(_Universities == null)
                {
                    _Universities = (from u in db.Universities
                                     select new CanvasUniversityModel() {
                                         IsSelected = false
                                         , UniversityDescription = u.UniversityDescription
                                         , UniversityId = u.UniversityId
                                         , UniversityName = u.UniversityName
                                     })
                                     .ToList();
                }
                return _Universities;
            }
        }

        private List<CanvasProviderModel> _Providers = null;
        public List<CanvasProviderModel> Providers
        {
            get
            {
                if(_Providers == null)
                {
                    _Providers = (from p in db.LMSProviders
                                  select new CanvasProviderModel() {
                                      CreatedBy = p.CreatedBy
                                      , IsSelected = false
                                      , ProviderCreateDate = p.ProviderCreateDate
                                      , ProviderDescription = p.ProviderDescription
                                      , ProviderId = p.ProviderId
                                      , ProviderName = p.ProviderName
                                      , Updatedby = p.Updatedby
                                  })
                                  .ToList();
                }
                return _Providers;
            }
        }

        private List<Guid> _SelectedProviders = null;
        public List<Guid> SelectedProviders
        {
            get
            {
                if(_SelectedProviders == null)
                {
                    _SelectedProviders = new List<Guid>();
                }
                return _SelectedProviders;
            }
            set
            {
                _SelectedProviders = value;
            }
        }

        private List<Guid> _SelectedUniversities = null;
        public List<Guid> SelectedUniversities
        {
            get
            {
                if(_SelectedUniversities == null)
                {
                    _SelectedUniversities = new List<Guid>();
                }
                return _SelectedUniversities;
            }
        }
    }
}