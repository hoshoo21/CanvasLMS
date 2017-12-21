using LMS.Areas.Canvas.CanvasAPI;
using LMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace LMS.Core
{
    public class BaseController : Controller
    {
        private JavaScriptSerializer _serializer = null;
        protected JavaScriptSerializer serializer
        {
            get
            {
                if(_serializer == null)
                {
                    _serializer = new JavaScriptSerializer();
                }
                return _serializer;
            }
        }

        public async Task<ActionResult> GetAllCourses()
        {
            clsApiTest CanvasAPI = new clsApiTest();
            CanvasAPI.createSampleConfigFile();
            dynamic taskallcourses = await CanvasAPI.execute();

            return Json(CanvasAPI.ReturnedJSON, JsonRequestBehavior.AllowGet);
        }

        private CanvasManager _canvasManager = null;
        protected CanvasManager canvasManager
        {
            get
            {
                if(_canvasManager == null)
                {
                    _canvasManager = new CanvasManager();
                }
                return _canvasManager;
            }
        }


        private int? _CanvasAccountId = null;
        protected int CanvasAccountId
        {
            get
            {
                if(_CanvasAccountId == null)
                {
                    _CanvasAccountId = GetSettings<int>("CanvasAccountId");
                }
                return _CanvasAccountId.Value;
            }
        }

        protected T GetSettings<T>(string sKey)
        {
            return (T)Convert.ChangeType(System.Configuration.ConfigurationManager.AppSettings[sKey], typeof(T));
        }

        private LMSEntities _db = null;
        protected LMSEntities db
        {
            get
            {
                if(_db == null)
                {
                    _db = new LMSEntities();
                }
                return _db;
            }
        }

        public ActionResult SuccessMessage(string sMessage)
        {
            return GetJsonMessage(sMessage, true);
        }
        public ActionResult NoSuccessMessage(string sMessage)
        {
            return GetJsonMessage(sMessage, false);
        }
        private ActionResult GetJsonMessage(string sMessage, bool isSuccess)
        {
            return Json(new { Success = isSuccess, Message = sMessage });
        }
    }
}