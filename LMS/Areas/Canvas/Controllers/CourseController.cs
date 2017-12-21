using LMS.Areas.Canvas.Models;
using LMS.Core;
using LMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LMS.Areas.Canvas.Controllers
{
    public class CourseController : BaseController
    {
        // GET: Canvas/Course
        private LMSEntities db = new LMSEntities();
        public async Task<ActionResult> Index1()
        {
            List<CanvasCourseModel> viewModel = new List<CanvasCourseModel>();

            ActionResult courses = await GetAllCourses();
            if (courses != null)
            {
                System.Web.Mvc.JsonResult result = courses as JsonResult;
                if (result != null)
                {
                    string json = base.serializer.Serialize(result.Data);
                    if (!string.IsNullOrEmpty(json))
                    {
                        var allCourses = result.Data.FromJson<List<CanvasCourseModel>>();
                        if (allCourses != null)
                        {
                            allCourses = (from c in allCourses
                                          orderby c.name
                                          select c)
                                          .ToList();
                            viewModel = allCourses;
                        }
                    }
                }

            }
            return View(viewModel);
        }

        public async Task<ActionResult> Index()
        {
            List<CanvasCourseModel> viewModel = new List<CanvasCourseModel>();

            string courses = await base.canvasManager.Execute(CanvasCommand.GET_COURSE_LIST) as string;
            if (courses != null)
            {
                List<CanvasCourseModel> allCourses = courses.FromJson<List<CanvasCourseModel>>();
                if(allCourses != null)
                {
                    allCourses.ForEach(c => {
                        c.course_id = (from cr in db.Courses
                                            where cr.CanvasId == c.id
                                            select cr.CourseId)
                                       .FirstOrDefault();
                    });
                }
                viewModel = allCourses;
            }
            return View(viewModel);
        }


        public ActionResult Create(Guid? Id)
        {
            Cours cours = db.Courses.Find(Id);
            var Add = new Courses.Add();
            if (cours != null)
            {
                Add.CourseDescription = cours.CourseDescription;
                Add.CourseId = cours.CourseId;
                Add.CourseName = cours.CourseName;
                UpdateViewBag(Add.CourseId);
                UpdateViewBagProvder(Add.ProviderId, Add.CourseId);
                UpdateViewBagUniversity(Add.UniversityId, Add.CourseId);
            }
            ViewBags();
            return View(Add);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Courses.Add Add)
        {
            if (ModelState.IsValid)
            {
                string oResponse = await base.canvasManager.CreateCourse(Add) as string;
                int iCanvasCourseId = 0;
                if(!string.IsNullOrEmpty(oResponse))
                {
                    CanvasCourseModel newCourse = oResponse.FromJson<CanvasCourseModel>();
                    iCanvasCourseId = newCourse.id;
                }

                var courceID = Guid.NewGuid();
                Cours c = new Cours();
                if (Add.CourseId != new Guid() && Add.CourseId != null)
                {
                    c = db.Courses.FirstOrDefault(e => e.CourseId == Add.CourseId);
                }
                if (c == null || c.CourseId == new Guid())
                {
                    c = new Cours();
                    c.CourseId = courceID;
                    c.CourseCreateDate = DateTime.Now;
                    c.CourseDescription = Add.CourseDescription;
                    c.CourseName = Add.CourseName;
                    c.CanvasId = iCanvasCourseId;
                    db.Courses.Add(c);
                }
                else
                {
                    courceID = c.CourseId;
                    c.CourseDescription = Add.CourseDescription;
                    c.CourseName = Add.CourseName;
                }

                var schedule = db.CourseSchedules.FirstOrDefault(ee => ee.CourseId == Add.CourseId);
                if (schedule != null)
                {
                    db.CourseSchedules.Remove(schedule);
                }

                schedule = new CourseSchedule();
                schedule.CourseId = courceID;
                schedule.EndDate = Add.EndDate;
                schedule.ScheduleId = Guid.NewGuid();
                schedule.StartDate = Add.StartDate;
                db.CourseSchedules.Add(schedule);

                var courceProvider = db.CourseProviders.Where(ee => ee.CourseId == courceID).ToList();
                if (courceProvider != null && courceProvider.Any())
                {
                    db.CourseProviders.RemoveRange(courceProvider);
                }

                if (Add.ProviderId != null && Add.ProviderId.Any())
                {
                    foreach (var item in Add.ProviderId)
                    {
                        db.CourseProviders.Add(new CourseProvider()
                        {
                            CourseId = courceID,
                            Id = Guid.NewGuid(),
                            ProviderId = item
                        });
                    }
                }

                var CourceUniversity = db.CourseUniversities.Where(ee => ee.CourseId == courceID).ToList();
                if (CourceUniversity != null && CourceUniversity.Any())
                {
                    db.CourseUniversities.RemoveRange(CourceUniversity);
                }

                if (Add.UniversityId != null && Add.UniversityId.Any())
                {
                    foreach (var item in Add.UniversityId)
                    {
                        db.CourseUniversities.Add(new CourseUniversity()
                        {
                            CourseId = courceID,
                            Id = Guid.NewGuid(),
                            UniversityId = item
                        });
                    }
                }


                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBags();
            return View(Add);
        }

        public ActionResult CourseDetail(int iCourseId)
        {
            return View();
        }

        public PartialViewResult CourseMapping(Guid? Id)
        {
            var CourseLMSUniveristies = (from CU in db.CourseUniversities
                                         join Unv in db.Universities on CU.UniversityId equals Unv.UniversityId
                                         join CP in db.CourseProviders on CU.CourseId equals CP.CourseId
                                         join LP in db.LMSProviders on CP.ProviderId equals LP.ProviderId
                                         where CU.CourseId == Id
                                         select new Courses.CourseDetail
                                         {
                                             LMSProvider = LP.ProviderName,
                                             UniversityName = Unv.UniversityName,
                                             CourseId = CU.CourseId
                                         }).ToList();
            return PartialView(CourseLMSUniveristies);
        }

        public ActionResult Detail(Guid? id)
        {
            CanvasCourseDetailModel courseDetail = new CanvasCourseDetailModel();
            if (id != null)
            {
                var obj = (from c in base.db.Courses
                           where c.CourseId == id
                           select c
                           ).FirstOrDefault();
                if (obj != null)
                {
                    courseDetail.CourseId = obj.CourseId;
                    courseDetail.CourseDescription = obj.CourseDescription;
                    courseDetail.CourseName = obj.CourseName;
                    courseDetail.EndDate = (from cs in base.db.CourseSchedules
                                            where cs.CourseId == obj.CourseId
                                            select cs.EndDate)
                                            .FirstOrDefault();
                    courseDetail.StartDate = (from cs in base.db.CourseSchedules
                                            where cs.CourseId == obj.CourseId
                                            select cs.StartDate)
                                            .FirstOrDefault();

                    var lstProviders = (from p in base.db.CourseProviders
                                        where p.CourseId == obj.CourseId
                                        select p.ProviderId)
                                        .ToList();
                    if(lstProviders != null)
                    {
                        courseDetail.SelectedProviders = new List<Guid>();
                        lstProviders.ForEach(p => {
                            courseDetail.SelectedProviders.Add(p.Value);
                        });
                    }

                    var lstUniversities = (from p in base.db.CourseUniversities
                                        where p.CourseId == obj.CourseId
                                        select p.UniversityId)
                                        .ToList();
                    if (lstUniversities != null)
                    {
                        courseDetail.SelectedUniversities = new List<Guid>();
                        lstUniversities.ForEach(u => {
                            courseDetail.SelectedUniversities.Add(u.Value);
                        });
                    }
                }
            }
            return View(courseDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> SaveCourseDetail(CanvasCourseDetailModel courseDetail)
        {
            if(ModelState.IsValid)
            {
                if(courseDetail.CourseId == null || courseDetail.CourseId == new Guid())
                {
                    return await CreateNewCourse(courseDetail);
                }
                else
                {
                    return await UpdateCourse(courseDetail);
                }
            }
            
            return base.serializer.Serialize(new { Success = false, Message = "Error" });
        }

        public async Task<ActionResult> DeleteCourse(Guid id)
        {
            if(id != null && id != new Guid())
            {
                Cours course = (from c in base.db.Courses
                                where c.CourseId == id
                                select c)
                                  .FirstOrDefault();
                if(course != null && course.CanvasId != null && course.CanvasId.Value > 0)
                {
                    string sResponse = await base.canvasManager.DeleteCourse(course.CanvasId.Value) as string;
                    if(sResponse != null)
                    {
                        base.db.Courses.Remove(course);
                        base.db.SaveChanges();
                    }
                }
            }
            return RedirectToAction("Index");
        }

        private async Task<string> CreateNewCourse(CanvasCourseDetailModel courseDetail)
        {
            if(courseDetail != null)
            {
                Courses.Add Add = new Courses.Add()
                {
                    CourseDescription = courseDetail.CourseDescription
                    ,
                    CourseName = courseDetail.CourseName
                    ,
                    EndDate = courseDetail.EndDate
                    ,
                    StartDate = courseDetail.StartDate
                };
                string oResponse = await base.canvasManager.CreateCourse(Add) as string;
                if(!string.IsNullOrEmpty(oResponse))
                {
                    CanvasCourseModel newCourse = oResponse.FromJson<CanvasCourseModel>();
                    if (newCourse != null)
                    {
                        Guid courseId = Guid.NewGuid();
                        Cours c = new Cours()
                        {
                            CanvasId = newCourse.id
                            , CourseCreateDate = DateTime.Now
                            , CourseDescription = courseDetail.CourseDescription
                            , CourseName = courseDetail.CourseName
                            , CourseId = courseId
                        };
                        
                        
                        try
                        {
                            base.db.Courses.Add(c);
                            db.SaveChanges();
                            SaveCourseSchedule(courseDetail);
                            SaveCourseProviders(courseDetail);
                            SaveCourseUniversities(courseDetail);
                        }
                        catch (Exception ex)
                        {
                            return base.serializer.Serialize(new { Success = false, Message = ex.Message });
                        }
                        
                        return base.serializer.Serialize(new { Success = true, Message = string.Empty });
                    }                    
                }
            }
            return base.serializer.Serialize(new { Success = false, Message = "Error" });
        }

        private async Task<string> UpdateCourse(CanvasCourseDetailModel courseDetail)
        {
            if(courseDetail != null)
            {
                int? iCanvasId = (from c in base.db.Courses
                                where c.CourseId == courseDetail.CourseId
                                select c.CanvasId
                                ).FirstOrDefault();
                if (iCanvasId != null)
                {
                    Courses.Add Add = new Courses.Add()
                    {
                        CourseDescription = courseDetail.CourseDescription
                    ,
                        CourseName = courseDetail.CourseName
                    ,
                        EndDate = courseDetail.EndDate
                    ,
                        StartDate = courseDetail.StartDate
                    };
                    string oResponse = await base.canvasManager.UpdateCourse(Add, iCanvasId.Value) as string;
                    if (!string.IsNullOrEmpty(oResponse))
                    {
                        
                        try
                        {
                            CanvasCourseModel updateCourse = oResponse.FromJson<CanvasCourseModel>();
                            Cours c = (from co in base.db.Courses
                                       where (co.CourseId == courseDetail.CourseId)
                                       select co)
                                       .FirstOrDefault();
                            if(c != null)
                            {
                                c.CourseName = courseDetail.CourseName;
                                c.CourseDescription = courseDetail.CourseDescription;
                                base.db.Entry(c).State = System.Data.Entity.EntityState.Modified;
                                base.db.SaveChanges();
                                SaveCourseSchedule(courseDetail);
                                SaveCourseProviders(courseDetail);
                                SaveCourseUniversities(courseDetail);
                            }
                            //Cours c = new Cours()
                            //{
                            //    CanvasId = updateCourse.id
                            //    ,
                            //    CourseCreateDate = DateTime.Now
                            //    ,
                            //    CourseDescription = courseDetail.CourseDescription
                            //    ,
                            //    CourseName = courseDetail.CourseName
                            //    ,
                            //    CourseId = Add.CourseId
                            //};
                            
                        }
                        catch(Exception ex)
                        {
                            return base.serializer.Serialize(new { Success = false, Message = ex.Message });
                        }
                        return base.serializer.Serialize(new { Success = true, Message = string.Empty });


                    }
                }
            }
            return base.serializer.Serialize(new { Success = false, Message = "Error" });
        }
        private void SaveCourseSchedule(CanvasCourseDetailModel courseDetail)
        {
            if(courseDetail != null)
            {
                var courseSchedule = (from cs in base.db.CourseSchedules
                                      where (cs.CourseId == courseDetail.CourseId)
                                      select cs)
                                      .FirstOrDefault();
                if(courseSchedule != null && (courseDetail.StartDate != null || courseDetail.EndDate != null))
                {
                    courseSchedule.StartDate = courseDetail.StartDate;
                    courseSchedule.EndDate = courseDetail.EndDate;
                    base.db.Entry(courseSchedule).State = System.Data.Entity.EntityState.Modified;
                    
                }
                else
                {
                    db.CourseSchedules.Add(new CourseSchedule()
                    {
                        ScheduleId = Guid.NewGuid()
                                ,
                        CourseId = courseDetail.CourseId

                                ,
                        EndDate = courseDetail.EndDate
                                ,
                        StartDate = courseDetail.StartDate
                    });
                }
                base.db.SaveChanges();
            }
        }
        private void SaveCourseProviders(CanvasCourseDetailModel courseDetail)
        {
            if(courseDetail != null)
            {
                var lstCourseProviders = (from cp in base.db.CourseProviders
                                          where cp.CourseId == courseDetail.CourseId
                                          select cp)
                                          .ToList();
                if(lstCourseProviders != null && lstCourseProviders.Count > 0)
                {
                    base.db.CourseProviders.RemoveRange(lstCourseProviders);
                }
                if (courseDetail.SelectedProviders != null && courseDetail.SelectedProviders.Count > 0)
                {
                    foreach (Guid aProvider in courseDetail.SelectedProviders)
                    {
                        base.db.CourseProviders.Add(new CourseProvider()
                        {
                            CourseId = courseDetail.CourseId
                            ,
                            ProviderId = aProvider
                            ,
                            Id = Guid.NewGuid()
                        });
                    }
                }
                base.db.SaveChanges();
            }
        }
        private void SaveCourseUniversities(CanvasCourseDetailModel courseDetail)
        {
            if(courseDetail != null)
            {
                var lstCourseUniversities = (from cu in base.db.CourseUniversities
                                             where (cu.CourseId == courseDetail.CourseId)
                                             select cu).ToList();
                if(lstCourseUniversities != null && lstCourseUniversities.Count > 0)
                {
                    base.db.CourseUniversities.RemoveRange(lstCourseUniversities);
                }
                if (courseDetail.SelectedUniversities != null)
                {
                    foreach (Guid aUni in courseDetail.SelectedUniversities)
                    {
                        base.db.CourseUniversities.Add(new CourseUniversity()
                        {
                            CourseId = courseDetail.CourseId
                            ,
                            Id = Guid.NewGuid()
                            ,
                            UniversityId = aUni
                        });
                    }
                }
                base.db.SaveChanges();
            }
        }

        #region Helping Methods
        public void UpdateViewBag(Guid? id)
        {
            // ViewBag.SelectUniversityId = db.CourseUniversities.Where(ee => ee.CourseId == id).ToList();
            ViewBag.CourseSchedule = db.CourseSchedules.FirstOrDefault(ee => ee.CourseId == id);
        }
        public void UpdateViewBagProvder(Guid?[] ProviderIds, Guid? CourseId)
        {
            List<Guid> LMSProvs = new List<Guid>();
            ViewBag.SelectProvideId = LMSProvs;

        }
        public void UpdateViewBagUniversity(Guid?[] UniversityIds, Guid? CourseId)
        {
            List<Guid> LMSUnv = new List<Guid>();
            ViewBag.SelectUniversityId = LMSUnv;
        }
        public void ViewBags()
        {
            ViewBag.University = db.Universities.ToList();
            ViewBag.Provider = db.LMSProviders.ToList();
        }
        #endregion
    }
}