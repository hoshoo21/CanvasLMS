using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Models;
using LMSCrawler;
namespace LMS.Controllers
{
    public class HomeController : Controller
    {
        private LMSEntities db = new LMSEntities();

        public ActionResult Index()
        {
            DashboardModel viewModel = new DashboardModel();
            List<University> universities = db.Universities.ToList();
            List<Cours> courses = db.Courses.ToList();

            viewModel.Courses = courses;
            viewModel.Universities = universities;

            

            return View(viewModel);
        }
        [HttpGet]
        public JsonResult GetStudentByCourse(string CourseID) 
        {
            List<ChartModel> lst = new List<ChartModel>();
            Guid id = Guid.Parse(CourseID);
            List<GetStudentUniversity_Result> DBlst = db.GetStudentUniversity(id).ToList();
            if (DBlst != null && DBlst.Count > 0)
            {
                foreach (var item in DBlst)
                {
                    lst.Add(new ChartModel { NoOfStudent = item.NoOfStudent+ 1000 , Category = item.UniversityName });
                }
            }

            //lst.Add(new ChartModel { NoOfStudent = 100, Category = "Baker University" });
            //lst.Add(new ChartModel { NoOfStudent = 589, Category = "Barstow College" });
            //lst.Add(new ChartModel { NoOfStudent = 258, Category = "Benedictine University" });
            //lst.Add(new ChartModel { NoOfStudent = 987, Category = "Northeastern University " });
            //lst.Add(new ChartModel { NoOfStudent = 547, Category = "University of Virginia" });
            //lst.Add(new ChartModel { NoOfStudent = 25, Category = "Upper Iowa University " });

            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}