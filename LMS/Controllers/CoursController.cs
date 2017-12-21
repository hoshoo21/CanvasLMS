using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS.Models;
using LMSCrawler;

namespace LMS.Controllers
{
    public class CoursController : Controller
    {
        private LMSEntities db = new LMSEntities();
        MoodleCrawler baseCrawler = new MoodleCrawler();

        public void getCourse() {
            baseCrawler.GotoMainPage();

            baseCrawler.LoginPage();


            Guid univid = Guid.Parse("dbac5470-14db-4492-80b0-d8e8d9d85c3e");

            foreach (string key in baseCrawler.CourseList.Keys)
            {
                string coursename = baseCrawler.CourseList[key];
                var course = (from crs in db.Courses where crs.CourseName == coursename select crs).FirstOrDefault();

                if (course == null)
                {


                    LMS.Models.Cours cose = new Cours();
                    cose.CourseId = Guid.NewGuid();
                    cose.CourseName = baseCrawler.CourseList[key];
                    cose.CourseDescription = baseCrawler.CourseList[key];
                    cose.CourseCreateDate = DateTime.Now;

                    db.Courses.Add(cose);

                    LMS.Models.CourseUniversity courseUnv = new CourseUniversity();
                    courseUnv.UniversityId = univid;
                    courseUnv.CourseId = cose.CourseId;
                    courseUnv.Id = Guid.NewGuid();

                    db.CourseUniversities.Add(courseUnv);

                    db.SaveChanges();
                }
            }
        }

        // GET: Cours
        public ActionResult Index()


        {

            
          

            return View(db.GetCourse().ToList());
        }

        // GET: Cours/Create


        public PartialViewResult CourseMapping(Guid? Id)
        {
            var CourseLMSUniveristies =  (from CU in db.CourseUniversities
                                           join Unv in db.Universities on CU.UniversityId equals Unv.UniversityId
                                           join CP in db.CourseProviders on CU.CourseId equals CP.CourseId
                                           join LP in db.LMSProviders on CP.ProviderId equals LP.ProviderId
                                          where CU.CourseId == Id
                                         select new Courses.CourseDetail {
                                             LMSProvider = LP.ProviderName,
                                             UniversityName = Unv.UniversityName,
                                             CourseId = CU.CourseId
                                         }).ToList();
            return PartialView(CourseLMSUniveristies);
        }


        public ActionResult CourseDicussion(Guid? Id)
        {


            //getCourse();
            var DiscussionList = (from kd in db.KnownDiscussions where kd.CourseId == Id select kd).ToList();
           // Console.WriteLine(baseCrawler.Discussions);
            return Json(DiscussionList, JsonRequestBehavior.AllowGet);
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
                UpdateViewBagProvder(Add.ProviderId,Add.CourseId);
                UpdateViewBagUniversity(Add.UniversityId, Add.CourseId);
            }
            ViewBags();
            return View(Add);
        }


        public void ViewBags()
        {
            ViewBag.University = db.Universities.ToList();
            ViewBag.Provider = db.LMSProviders.ToList();
        }

        public void UpdateViewBag(Guid? id)
        {
           // ViewBag.SelectUniversityId = db.CourseUniversities.Where(ee => ee.CourseId == id).ToList();
            ViewBag.CourseSchedule = db.CourseSchedules.FirstOrDefault(ee=>ee.CourseId == id);
        }

        public void UpdateViewBagProvder(Guid?[] ProviderIds, Guid? CourseId) {
            List<Guid> LMSProvs = new List<Guid>();
            //for (int i = 0; i < ProviderIds.Length; i++) {
            //    var Providers = (from prov in db.CourseUniversityLMS where prov.ProviderId == ProviderIds[i] select prov.ProviderId).FirstOrDefault();
            //    LMSProvs.Add(Providers);
            //    var courseproviders = db.CourseProviders.Where(ee => ee.ProviderId == Providers && ee.CourseId == CourseId).ToList();
            //    foreach (CourseProvider item in courseproviders) {
            //        LMSProvs.Add(item.ProviderId??Guid.Empty);
            //    }
            //}

            ViewBag.SelectProvideId = LMSProvs;

        }
        public void UpdateViewBagUniversity(Guid?[] UniversityIds, Guid? CourseId)
        {
            List<Guid> LMSUnv = new List<Guid>();
            //for (int i = 0; i < UniversityIds.Length; i++)
            //{
            //    var Providers = (from prov in db.CourseUniversityLMS where prov.UniversityId == UniversityIds[i] select prov.ProviderId).FirstOrDefault();
            //    LMSUnv.Add(Providers);
            //    var courseproviders = db.CourseProviders.Where(ee => ee.ProviderId == Providers && ee.CourseId == CourseId).ToList();
            //    foreach (CourseProvider item in courseproviders)
            //    {
            //        LMSUnv.Add(item.ProviderId ?? Guid.Empty);
            //    }
            //}

            ViewBag.ViewBag.SelectUniversityId = LMSUnv;

        }

        // POST: Cours/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Courses.Add Add)
        {
            if (ModelState.IsValid)
            {
                var courceID = Guid.NewGuid();
                Cours c = new Cours();
                if(Add.CourseId != new Guid() && Add.CourseId != null)
                {
                    c = db.Courses.FirstOrDefault(e => e.CourseId == Add.CourseId);
                }
                if (c == null || c.CourseId == new Guid() )
                {
                    c = new Cours();
                    c.CourseId = courceID;
                    c.CourseCreateDate = DateTime.Now;
                    c.CourseDescription = Add.CourseDescription;
                    c.CourseName = Add.CourseName;
                    db.Courses.Add(c);
                }
                else
                {
                    courceID = c.CourseId;
                    c.CourseDescription = Add.CourseDescription;
                    c.CourseName = Add.CourseName;
                }

                var schedule = db.CourseSchedules.FirstOrDefault(ee => ee.CourseId == Add.CourseId);
                if(schedule != null)
                {
                    db.CourseSchedules.Remove(schedule);
                }

                schedule = new CourseSchedule();
                schedule.CourseId = courceID;
                schedule.EndDate = Add.EndDate;
                schedule.ScheduleId = Guid.NewGuid();
                schedule.StartDate = Add.StartDate;
                db.CourseSchedules.Add(schedule);

                var courceProvider = db.CourseProviders.Where(ee=>ee.CourseId == courceID).ToList();
                if(courceProvider != null && courceProvider.Any())
                {
                    db.CourseProviders.RemoveRange(courceProvider);
                }

                if(Add.ProviderId != null && Add.ProviderId.Any())
                {
                    foreach (var item in Add.ProviderId)
                    {
                        db.CourseProviders.Add(new CourseProvider() {
                             CourseId = courceID,
                              Id = Guid.NewGuid(),
                              ProviderId = item
                        });
                    }
                }

                var CourceUniversity = db.CourseUniversities.Where(ee => ee.CourseId == courceID).ToList();
                if(CourceUniversity != null && CourceUniversity.Any())
                {
                    db.CourseUniversities.RemoveRange(CourceUniversity);
                }

                if(Add.UniversityId != null && Add.UniversityId.Any())
                {
                    foreach (var item in Add.UniversityId)
                    {
                        db.CourseUniversities.Add(new CourseUniversity()
                        {
                            CourseId = courceID,
                            Id = Guid.NewGuid(),
                            UniversityId= item
                        });
                    }
                }


                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBags();
            return View(Add);
        }

        // GET: Cours/Delete/5
        public ActionResult Delete(Guid? id)
        {
            var delete = db.Courses.FirstOrDefault(e=>e.CourseId == id);
            if(delete !=null)
            {
                db.Courses.Remove(delete);
                db.SaveChanges();
            }
            return RedirectToAction("index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
