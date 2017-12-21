using System.Web.Mvc;

namespace LMS.Areas.Canvas
{
    public class CanvasAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Canvas";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //context.MapRoute(
            //    "Canvas_default",
            //    "Canvas/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);
            context.MapRoute(
                "Canvas_default",
                "Canvas/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
                //new { controller = "Branch|AdminHome|User" },
                , new { controller = "Canvas|Course|CanvasResponder|CanvasDiscussion" }
                , new[] { "LMS.Areas.Canvas.Controllers" }
            );
        }
    }
}