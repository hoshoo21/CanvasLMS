using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using canvasApiTest;
namespace LMS.Controllers
{
    public class CanvasController : Controller
    {
        // GET: Canvas
        public ActionResult Index()
        {
            return View("CanvasDashBoard");
        }

        public async Task<ActionResult> GetAllCourses() {
            clsApiTest CanvasAPI = new clsApiTest();
            CanvasAPI.createSampleConfigFile();
            dynamic taskallcourses= await CanvasAPI.execute();
            
            return Json(CanvasAPI.ReturnedJSON, JsonRequestBehavior.AllowGet);
        }

    }
}