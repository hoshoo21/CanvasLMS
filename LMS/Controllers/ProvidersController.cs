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
    public class ProvidersController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: Providers
        public ActionResult Index()
        {
            return View(db.LMSProviders.ToList());
        }

        // GET: Providers/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LMSProvider lMSProvider = db.LMSProviders.Find(id);
            if (lMSProvider == null)
            {
                return HttpNotFound();
            }
            return View(lMSProvider);
        }

        // GET: Providers/Create
        public ActionResult Create(Guid? id)
        {
            LMSProvider lMSProvider = db.LMSProviders.Find(id);
            if (lMSProvider == null)
            {
                lMSProvider = new LMSProvider();
                lMSProvider.ProviderId = new Guid();
            }
            return View(lMSProvider);
        }

        // GET: Providers/Edit
        public ActionResult Edit(Guid? id)
        {
            var viewModel = new ProviderUniversitiesModel();

            LMSProvider lMSProvider = db.LMSProviders.Find(id);
            if (lMSProvider == null)
            {
                return HttpNotFound();

            }
            else
            {
                viewModel.Provider = lMSProvider;
                List<CourseUniversityLM> lst = db.CourseUniversityLMS.Where(a => a.ProviderId == lMSProvider.ProviderId).ToList();
                if (lst.Count > 0)
                {
                    foreach (var item in lst)
                    {
                        University university = db.Universities.Find(item.UniversityId);
                        viewModel.Universities.Add(university);
                    }
                }
            }
            return View(viewModel);
        }
        // POST: Providers/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProviderUniversitiesModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.Provider.ProviderId != new Guid())
                {
                    db.Entry(viewModel.Provider).State = EntityState.Modified;
                }
                //else
                //{
                //    viewModel.Provider.ProviderCreateDate = DateTime.Now;
                //    viewModel.Provider.ProviderId = Guid.NewGuid();
                //    db.LMSProviders.Add(viewModel.Provider);
                //}
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(viewModel);
        }

        // POST: Providers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProviderId,ProviderName,ProviderDescription,ProviderCreateDate,CreatedBy,Updatedby")] LMSProvider lMSProvider)
        {
            if (ModelState.IsValid)
            {
                //if (lMSProvider.ProviderId != new Guid())
                //{
                //    db.Entry(lMSProvider).State = EntityState.Modified;
                //}
                //else
                //{
                    lMSProvider.ProviderCreateDate = DateTime.Now;
                    lMSProvider.ProviderId = Guid.NewGuid();
                    db.LMSProviders.Add(lMSProvider);
                //}
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(lMSProvider);
        }

        // POST: Providers/Delete/5
        [HttpGet, ActionName("Delete")]
        public ActionResult Delet(Guid? id)
        {
            LMSProvider lMSProvider = db.LMSProviders.Find(id);
            if (lMSProvider != null)
            {
                db.LMSProviders.Remove(lMSProvider);
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
