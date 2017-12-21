using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using LMS.Areas.Canvas.Models;
using System.Web.Script.Serialization;
using LMS.Core;
using Newtonsoft.Json;
using LMS.Areas.Canvas.CanvasAPI;
using Newtonsoft.Json.Linq;

namespace LMS.Areas.Canvas.Controllers
{
    public class CanvasController :  BaseController
    {
        // GET: Canvas
        public async Task<ActionResult> Index1()
        {
            CanvasManager cm = new CanvasManager();
            await cm.GetCourseDetail(100);

            await cm.Execute(CanvasCommand.GET_COURSE_LIST);

            CanvasDashboardModel viewModel = new CanvasDashboardModel();

            ActionResult courses = await GetAllCourses();            
            if(courses != null)
            {
                System.Web.Mvc.JsonResult result = courses as JsonResult;
                if(result != null)
                {
                    string json = base.serializer.Serialize(result.Data);
                    if(!string.IsNullOrEmpty(json))
                    {
                        var allCourses = result.Data.FromJson<List<CanvasCourseModel>>();
                        if(allCourses != null)
                        {
                            allCourses = (from c in allCourses
                                          orderby c.name
                                          select c)
                                          .ToList();
                            viewModel.Courses = allCourses;
                        }
                    }
                }
                
            }
            return View("~/Areas/Canvas/Views/Canvas/CanvasDashBoard.cshtml", viewModel);
        }

        public async Task<ActionResult> Index()
        {
            CanvasManager cm = new CanvasManager();

            CanvasDashboardModel viewModel = new CanvasDashboardModel();

            string courses = await cm.Execute(CanvasCommand.GET_COURSE_LIST) as string;
            if (courses != null)
            {
                List<CanvasCourseModel> allCourses = courses.FromJson<List<CanvasCourseModel>>();
                viewModel.Courses = allCourses;
            }
            return View("~/Areas/Canvas/Views/Canvas/CanvasDashBoard.cshtml", viewModel);
        }

    }
}