using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LMS.Models;

namespace LMS.Controllers
{
    public class UniversitiesController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: Universities
        public ActionResult Index()
        {
            return View(db.Universities.ToList());
        }

        public void ViewBags()
        {
            ViewBag.Provider = db.LMSProviders.ToList();
        }

        public void UpdateViewBag(Guid? id)
        {
            ViewBag.SelectProviders = db.CourseUniversityLMS.Where(ee => ee.UniversityId == id).ToList(); ;
        }

        // GET: Universities/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            University university = db.Universities.Find(id);
            if (university == null)
            {
                return HttpNotFound();
            }
            return View(university);
        }

        // GET: Universities/Create
        public ActionResult Create(Guid? Id)
        {
            ProdiverUnviersityModel ProviderListObject = new ProdiverUnviersityModel();
            
            University university = db.Universities.Find(Id);
            if (university == null)
            {
                university = new University();
                university.UniversityId = new Guid();

                ProviderListObject.UniversityId = university.UniversityId;
               
              
            }
            else {
                var providers = (from lP in db.LMSProviders
                                 join cus in db.CourseUniversityLMS on lP.ProviderId equals cus.ProviderId
                                 where cus.UniversityId == university.UniversityId
                                 select lP);

              
                ProviderListObject.UniversityDescription = university.UniversityDescription;
                ProviderListObject.UniversityId = university.UniversityId;
                ProviderListObject.UniversityName = university.UniversityName;
                UpdateViewBag(university.UniversityId);       
               
            }
            
            ViewBags();
            
            return View(ProviderListObject);
        }

        // POST: Universities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProdiverUnviersityModel universityviewmodel)
        {
            University university = new University();
            if (ModelState.IsValid)
            {
                
                university = (from unv in
                                  db.Universities
                              where unv.UniversityId == universityviewmodel.UniversityId
                              select unv).FirstOrDefault();
                if (universityviewmodel.UniversityId != new Guid())
                {
                    university.UniversityName = universityviewmodel.UniversityName;
                    universityviewmodel.UniversityDescription = universityviewmodel.UniversityDescription;
                    db.Entry(university).State = EntityState.Modified;
                }
                else
                {

                    university = new University();
                    university.UniversityId = Guid.NewGuid();
                    university.UniversityName = universityviewmodel.UniversityName;
                    university.UniversityDescription = universityviewmodel.UniversityDescription;
                    db.Universities.Add(university);
                }
                
                var CUS = db.CourseUniversityLMS.Where(ee => ee.UniversityId == university.UniversityId).ToList();
                if (CUS != null && CUS.Any())
                {
                    db.CourseUniversityLMS.RemoveRange(CUS);
                }

                if (university.UniversityId != null && universityviewmodel.ProviderId.Any())
                {
                    foreach (var item in universityviewmodel.ProviderId)
                    {
                       
                        db.CourseUniversityLMS.Add(new CourseUniversityLM ()
                        {
                            ProviderId = item ?? Guid.Empty,
                            UniversityId = university.UniversityId
                        });
                    }
                }

                UpdateViewBag(university.UniversityId); 
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(universityviewmodel);
        }


        [HttpGet, ActionName("Delete")]
        // GET: Universities/Delete/5
        public ActionResult Delete(Guid? id)
        {
            University university = db.Universities.Find(id);
            if(university != null)
            {
                db.Universities.Remove(university);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
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
