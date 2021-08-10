using Emlak_Yorumlari_Entities;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Emalk_Yorumlari_Redis;
using Emlak_Yorumlari.Models;
using Emlak_Yorumlari_Entities.Models;
using Emlak_Yorumlari_WebApp.ViewModels;

namespace Emlak_Yorumlari_WebApp.Controllers
{
    public class AdminCommentController : Controller
    {
        private MyContext db = new MyContext();

        public static void deleteToxicCommentDaily()
        {
            MyContext db_delete = new MyContext();
            var toxicComment = db_delete.Comment_Logs.Where(x => x.toxic_type == 1 && x.createdOn.Day <= (DateTime.Now.Day - 1)).ToList();
            var toxicScore = db_delete.Survey_Logs.Where(x => x.toxic_type == 1 && x.createdOn.Day <= (DateTime.Now.Day - 1)).ToList();

            foreach(var comment in toxicComment)
            {
                db_delete.Comment_Logs.Remove(comment);
                db_delete.SaveChanges();
            }
            foreach(var score in toxicScore)
            {
                db_delete.Survey_Logs.Remove(score);
                db_delete.SaveChanges();
            }
        }

        // GET: AdminComment
        public ActionResult ShowComment()
        {
            var dataList = db.Comment_Logs.ToList();
            AdminCommentViewModel model = new AdminCommentViewModel();
            model.ClassList = new List<AdminCommentViewModel>();
            foreach(var data in dataList)
            {
                var user = db.Users.Where(x => x.user_id == data.user_id).FirstOrDefault();
                var place = db.Places.Where(x => x.place_id == data.place_id).FirstOrDefault();
                AdminCommentViewModel elementModel = new AdminCommentViewModel();
                elementModel.user = user;
                elementModel.place = place;
                elementModel.toxicComment = data;
                model.ClassList.Add(elementModel);                
            }


            return View(model);
        }

        public ActionResult ShowBannedComment()
        {
            var dataList = db.Comment_Logs.ToList();
            AdminCommentViewModel model = new AdminCommentViewModel();
            model.ClassList = new List<AdminCommentViewModel>();
            foreach (var data in dataList)
            {
                var user = db.Users.Where(x => x.user_id == data.user_id).FirstOrDefault();
                var place = db.Places.Where(x => x.place_id == data.place_id).FirstOrDefault();
                AdminCommentViewModel elementModel = new AdminCommentViewModel();
                elementModel.user = user;
                elementModel.place = place;
                elementModel.toxicComment = data;
                model.ClassList.Add(elementModel);
            }


            return View(model);
        }

        public ActionResult AddComment(int? user_id, int? place_id)
        {
            if(user_id != null && place_id != null)
            {
                
                var logComment = db.Comment_Logs.Where(x => x.user_id == user_id && x.place_id == place_id).FirstOrDefault();
                var logSurvey = db.Survey_Logs.Where(x => x.user_id == user_id && x.place_id == place_id).ToList();
                foreach(var data in logSurvey)
                {

                    Survey addQuiz = new Survey();
                    addQuiz.user = data.user;
                    addQuiz.user_id = data.user.user_id;
                    addQuiz.place = data.place;
                    addQuiz.place_id = data.place.place_id;
                    addQuiz.question_definitioın = data.question_definitioın;
                    addQuiz.question_id = data.question_definitioın.question_id;
                    addQuiz.score = data.score;
                    addQuiz.createdOn = data.createdOn;
                    addQuiz.IsActive = true;

                    db.Surveys.Add(addQuiz);
                    data.place.surveys.Add(addQuiz);
                    data.user.surveys.Add(addQuiz);
                    db.SaveChanges();
                    

                }
                Comment addComment = new Comment();
                addComment.user = logComment.user;
                addComment.user_id = logComment.user_id;
                addComment.place = logComment.place;
                addComment.place_id = logComment.place_id;
                addComment.text = logComment.text;
                addComment.createdOn = logComment.createdOn;
                addComment.IsActive = true;

                db.Comments.Add(addComment);
                logComment.user.comments.Add(addComment);
                logComment.place.comments.Add(addComment);
                db.SaveChanges();

                db.Comment_Logs.Remove(logComment);
                var logSurveyDelete = db.Survey_Logs.Where(x => x.user_id == user_id && x.place_id == place_id).ToList();
                foreach (var data in logSurveyDelete)
                {
                    db.Survey_Logs.Remove(data);
                    db.SaveChanges();
                }

            }
            return RedirectToAction("ShowComment");
        }

        public ActionResult DeleteComment(int? user_id, int? place_id)
        {
            if(user_id != null && place_id != null)
            {
                var logComment = db.Comment_Logs.Where(x => x.user_id == user_id && x.place_id == place_id).FirstOrDefault();
                db.Comment_Logs.Remove(logComment);
                var logSurvey = db.Survey_Logs.Where(x => x.user_id == user_id && x.place_id == place_id).ToList();
                foreach(var data in logSurvey)
                {
                    db.Survey_Logs.Remove(data);
                    db.SaveChanges();
                }

            }
            return RedirectToAction("ShowComment");
        }


    }
}