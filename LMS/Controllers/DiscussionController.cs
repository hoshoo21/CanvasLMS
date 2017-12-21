using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LMS.Models;
using Newtonsoft.Json;

namespace LMS.Controllers
{
    public class DiscussionController : Controller
    {
        private LMSEntities db = new LMSEntities();
        // GET: Discussion
        public ActionResult Index()
        {
            ViewBag.Faq = db.GetFaq(true);
            ViewBag.Discussion = db.GetDiscussion(false);
            return View();
        }


        public JsonResult UpdateFaq(Guid FaqId, string FaqAnswer)
        {
            var faq = (from fq in db.FAQs where fq.FAQId == FaqId select fq).FirstOrDefault();
            if (faq != null)
            {
                faq.Answer = FaqAnswer;
            }
            db.SaveChanges();

            string serializedstring = JsonConvert.SerializeObject("Question Updated Successfully");


            return Json(serializedstring);
        }

        public ActionResult GetFaqResponse(Guid FaqId)
        {
            var faq = (from fq in db.FAQs where fq.FAQId == FaqId select fq).FirstOrDefault();

            return Json(JsonConvert.SerializeObject(faq));


        }



        public ActionResult GetParticipants()
        {

            return View("Participants");

        }
        public ActionResult GetParticipantsContents()
        {

            List<TopicParticipants> participcants = new List<TopicParticipants>();

            participcants = (from sbd in db.SubKnowDiscussions
                             join crs in db.Courses on sbd.CourseId equals crs.CourseId
                             join subd in db.SubDiscussionDetails on sbd.Id equals subd.SubDiscussionId
                             join kd in db.KnownDiscussions on sbd.DiscussionId equals kd.id
                             where sbd.IsNewDiscussion == true || sbd.IsNewDiscussion == null
                             select new TopicParticipants
                             {
                                 DiscussionId = subd.id,
                                 ParticipantName = sbd.Originator,
                                 CourseId = crs.CourseId,
                                 CourseName = crs.CourseName,
                                 SubTopic = subd.SubDiscussionSubject,
                                 Discussion = kd.Question

                             }

                             ).ToList();


            return Json(participcants, JsonRequestBehavior.AllowGet);


        }

        public ActionResult GetParticipantsContentsbyCourse(string CourseId)
        {

            Guid Id = Guid.Parse(CourseId);

            List<TopicParticipants> participcants = new List<TopicParticipants>();

            participcants = (from sbd in db.SubKnowDiscussions
                             join crs in db.Courses on sbd.CourseId equals crs.CourseId
                             join subd in db.SubDiscussionDetails on sbd.Id equals subd.SubDiscussionId
                             join kd in db.KnownDiscussions on sbd.DiscussionId equals kd.id
                             where sbd.CourseId == Id && (sbd.IsNewDiscussion == true || sbd.IsNewDiscussion == null)
                             select new TopicParticipants
                             {
                                 DiscussionId = subd.id,
                                 ParticipantName = sbd.Originator,
                                 CourseId = crs.CourseId,
                                 CourseName = crs.CourseName,
                                 SubTopic = subd.SubDiscussionSubject,
                                 Discussion = kd.Question

                             }

                             ).ToList();


            return Json(participcants, JsonRequestBehavior.AllowGet);


        }

        public ActionResult GetParticipantsContentsbyName(string name)
        {

            

            List<TopicParticipants> participcants = new List<TopicParticipants>();

            participcants = (from sbd in db.SubKnowDiscussions
                             join crs in db.Courses on sbd.CourseId equals crs.CourseId
                             join subd in db.SubDiscussionDetails on sbd.Id equals subd.SubDiscussionId
                             join kd in db.KnownDiscussions on sbd.DiscussionId equals kd.id
                             where sbd.Originator==name && (sbd.IsNewDiscussion ==true || sbd.IsNewDiscussion==null )
                             select new TopicParticipants
                             {
                                 DiscussionId = subd.id,
                                 ParticipantName = sbd.Originator,
                                 CourseId = crs.CourseId,
                                 CourseName = crs.CourseName,
                                 SubTopic = subd.SubDiscussionSubject,
                                 Discussion = kd.Question

                             }

                             ).ToList();


            return Json(participcants, JsonRequestBehavior.AllowGet);


        }

        public ActionResult GetDiscussionDetail(string SubDisccussId)
        {



            List<TopicParticipants> participcants = new List<TopicParticipants>();
            Guid id = Guid.Parse(SubDisccussId);
            participcants = (from sbd in db.SubDiscussionDetails

                             where sbd.id ==id
                             select new TopicParticipants
                             {
                                 Discussion = sbd.SubDiscussionDetail1,
                                 DiscussionId = sbd.SubDiscussionId?? Guid.Empty

                             }

                             ).ToList();


            return Json(participcants, JsonRequestBehavior.AllowGet);


        }
        public ActionResult PostResponseCotents(string SubDisccussId, string ResponseContents)
        {


            PendingReply reply = new PendingReply();

            reply.Id = Guid.NewGuid();
            reply.ResponseContent = ResponseContents;
            reply.DiscussionId = Guid.Parse(SubDisccussId);


            db.PendingReplies.Add(reply);
            db.SaveChanges();


            return Json("Response Successfully posted to moodle, will be posted on mooble in next 30 to 60 secods", JsonRequestBehavior.AllowGet);


        }


        public ActionResult GetDiscussion(string CourseId) {

            Guid crsid = Guid.Empty;
            if (!String.IsNullOrEmpty(CourseId)) {
                crsid = Guid.Parse(CourseId);
            }

            List<KnownDiscussion> discussions = new List<KnownDiscussion>() ;
            if (crsid == Guid.Empty)
            {
                discussions = (from kd in db.KnownDiscussions where kd.CourseId != null  select kd).ToList();
            }
            else
            {
                discussions = (from kd in db.KnownDiscussions where kd.CourseId== crsid select kd).ToList();
            }

            return Json(discussions, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetCourses() {
            var courses = (from crs in db.Courses select crs).ToList();

            return Json(courses, JsonRequestBehavior.AllowGet);
            

        }

        public ActionResult GetDiscussionContents(int DiscussionId, string CourseId) {
            Guid crsId = Guid.Parse(CourseId);

            List<SubKnowDiscussion> discussion = (from skd in db.SubKnowDiscussions
                                                  where skd.CourseId == crsId && skd.DiscussionId ==DiscussionId
                                                  select skd).ToList();


            return Json(discussion, JsonRequestBehavior.AllowGet);


        }


        public ActionResult ViewTopic(int DiscussionId, string SubknownDisucssionId)
        {
            Guid crsId = Guid.Parse(SubknownDisucssionId);

            List<SubDiscussionForumReply> discussion = new List<SubDiscussionForumReply>();

            var main = (from skd in db.SubKnowDiscussions
                        join sdr in db.SubDiscussionForumReplies on skd.Id equals sdr.SubKnownDiscussionId
                        where skd.Id == crsId && sdr.IsmainPost == true
                        select sdr
                         ).FirstOrDefault();

            if (main != null) {
                discussion.Add(main);
                var replies = (from skd in db.SubKnowDiscussions
                        join sdr in db.SubDiscussionForumReplies on skd.Id equals sdr.SubKnownDiscussionId
                        where skd.Id == crsId && sdr.IsmainPost == false
                               select sdr
                         ).ToList();

                foreach (SubDiscussionForumReply item in replies) {
                    discussion.Add(item);
                }

            }

            return Json(discussion, JsonRequestBehavior.AllowGet);


        }

    }
}