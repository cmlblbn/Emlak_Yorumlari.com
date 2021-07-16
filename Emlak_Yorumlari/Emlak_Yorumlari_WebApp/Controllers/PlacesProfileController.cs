using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using Emalk_Yorumlari_Redis;
using Emlak_Yorumlari_Entities;
using Emlak_Yorumlari_Entities.Models;
using Emlak_Yorumlari_WebApp.ViewModels;

namespace Emlak_Yorumlari_WebApp.Controllers
{
    public class PlacesProfileController : Controller
    {
        // GET: PlacesProfile
        private MyContext db = new MyContext();

        private RedisManager redis = new RedisManager();

        public static float[] PlaceScoresCalculator(Place place)
        {
            float[] scores = new float[3];
            MyContext puanSorgu = new MyContext();
            IQueryable<Survey> q1 = puanSorgu.Surveys.Where(x => x.question_id == 1 && x.place_id == place.place_id);
            IQueryable<Survey> q2 = puanSorgu.Surveys.Where(x => x.question_id == 2 && x.place_id == place.place_id);
            IQueryable<Survey> q3 = puanSorgu.Surveys.Where(x => x.question_id == 3 && x.place_id == place.place_id);

            var sorguList = q1.ToList();




            float q1_mainscore = 0;
            float q2_mainscore = 0;
            float q3_mainscore = 0;


            foreach (var point in q1)
            {
                if (sorguList.Count == 0 )
                {
                    q1_mainscore = 0;
                    break;
                }
                q1_mainscore = q1_mainscore + point.score;
               
            }
            foreach (var point in q2)
            {
                if (sorguList.Count == 0)
                {
                    q2_mainscore = 0;
                    break;
                }
                q2_mainscore = q2_mainscore + point.score;
              
            }
            foreach (var point in q3)
            {

                if (sorguList.Count == 0)
                {
                    q3_mainscore = 0;
                    break;
                }
                q3_mainscore = q3_mainscore + point.score;
                
            }




            q1_mainscore = q1_mainscore / q1.Count();
            q2_mainscore = q2_mainscore / q2.Count();
            q3_mainscore = q3_mainscore / q3.Count();

            if (Single.IsNaN(q1_mainscore))
            {
                q1_mainscore = 0;
                q2_mainscore = 0;
                q3_mainscore = 0;
            }




            scores[0] = q1_mainscore;
            scores[1] = q2_mainscore;
            scores[2] = q3_mainscore;


            return scores;
        }

        public static List<string> GetUserScore(User sorguUser,Place place)
        {
            List<string> scores = new List<string>(4);
            MyContext sonSorgu = new MyContext();
            IQueryable<Survey> userPuan = sonSorgu.Surveys.Where(x => x.place_id == place.place_id && x.user_id == sorguUser.user_id);

            foreach (var puan in userPuan)
            {
                if (puan.question_id == 1)
                {
                    scores.Insert(0,puan.score.ToString());
                }
                else if(puan.question_id == 2)
                {
                    scores.Insert(1,puan.score.ToString());
                }
                else if(puan.question_id == 3)
                {
                    scores.Insert(2,puan.score.ToString());
                }
            }


            return scores;
        }

        public Dictionary<string, List<string>> commentHelper(MyContext db,Place place)
        {

            Dictionary<string, List<string>> commentsAndPoints = new Dictionary<string, List<string>>();
            MyContext yorumSorgu = new MyContext();
            User sorguUser = new User();

            IQueryable<Comment> comments = db.Comments.Where(x => x.place_id == place.place_id);


            string Commentusername = "";
            foreach (var comment in comments)
            {
                sorguUser = yorumSorgu.Users.Where(x => x.user_id == comment.user_id).FirstOrDefault();
                Commentusername = sorguUser.username;
                var userscoreComment = GetUserScore(sorguUser, place);
                userscoreComment.Insert(3,comment.text);
                commentsAndPoints.Add(Commentusername, userscoreComment);
            }

            return commentsAndPoints;
        }

        public ActionResult PlaceProfile(int? placeId)
        {

            if (Session["User"] == null && Session["Admin"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            
            if (placeId != null)
            {
                placeId = Convert.ToInt32(placeId);
            }

            string username = "";
            if (Session["User"] == null)
            {
                username = Session["Admin"].ToString();
            }
            else
            {
                username = Session["User"].ToString();
            }

            User user = new User();
            user = db.Users.Where(x => x.username == username).FirstOrDefault();
            Comment kisisorgu = new Comment();
            kisisorgu = db.Comments.Where(x => x.user_id == user.user_id && x.place_id == placeId).FirstOrDefault();
            if (kisisorgu != null)
            {
                TempData["post"] = "ok";
            }
            else
            {
                TempData["post"] = "not";
            }

            Question_Definition questionName1 = new Question_Definition();
            questionName1 = db.Question_Definitions.Where(x => x.question_id == 1).FirstOrDefault();
            Question_Definition questionName2 = new Question_Definition();
            questionName2 = db.Question_Definitions.Where(x => x.question_id == 2).FirstOrDefault();
            Question_Definition questionName3 = new Question_Definition();
            questionName3 = db.Question_Definitions.Where(x => x.question_id == 3).FirstOrDefault();

            PlaceWithoutSurveys model = new PlaceWithoutSurveys();
            Place place = new Place();
            place = db.Places.Where(x => x.place_id == placeId).FirstOrDefault();
            
            if (place.IsActive == false && Session["Admin"] == null)
            {
                return RedirectToAction("DeActivatedProfileError");
            }
            //redis.Remove(place.place_id.ToString());
            var scores = PlaceScoresCalculator(place);

            model.guven_puani_mainscore = scores[0];
            model.aktivite_alani_mainscore = scores[1];
            model.yonetim_memnuniyeti_mainscore = scores[2];
            Dictionary<string, List<string>> commentsAndPoints;
            if (redis.IsSet(place.place_id.ToString()))
            {
                var getJson = redis.getKey(place.place_id.ToString());
                var getDictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(getJson);
                commentsAndPoints = getDictionary;
            }
            else
            {
                commentsAndPoints = commentHelper(db, place);
                var getJson = Newtonsoft.Json.JsonConvert.SerializeObject(commentsAndPoints);
                redis.setKey(place.place_id.ToString(), getJson, 3600);
            }

            model.commentsAndPoints = commentsAndPoints;
            model.guven_puani_score = 0;
            model.aktivite_alani_score = 0;
            model.yonetim_memnuniyeti_score = 0;
            model.comment = "";
            model.question1_name = questionName1.question_name;
            model.question2_name = questionName2.question_name;
            model.question3_name = questionName3.question_name;
            model.place = place;
            model.user = user;

            Adress_Description mahalle = new Adress_Description();
            Adress_Description ilce = new Adress_Description();
            Adress_Description sehir = new Adress_Description();
            MyContext sorgu = new MyContext();

            string birlesmisAdres;

            birlesmisAdres = "";
            mahalle = sorgu.Adress_Descriptions.Where(x => x.adress_desc_id == model.place.adress_desc_id).FirstOrDefault();
            ilce = sorgu.Adress_Descriptions.Where(x => x.adress_desc_id == mahalle.parent_id).FirstOrDefault();
            sehir = sorgu.Adress_Descriptions.Where(x => x.adress_desc_id == ilce.parent_id).FirstOrDefault();

            birlesmisAdres = birlesmisAdres + sehir.adress_name + " / ";
            birlesmisAdres = birlesmisAdres + ilce.adress_name + " / ";
            birlesmisAdres = birlesmisAdres + mahalle.adress_name;

            ViewBag.TamAdres = birlesmisAdres;

            return View(model);
        }

        [HttpPost]
        public ActionResult PlaceProfile(PlaceWithoutSurveys model, int? placeId)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            TempData["post"] = "ok";
            Place place = new Place();
            place = db.Places.Where(x => x.place_id == placeId).FirstOrDefault();
            string username = Session["User"].ToString();
            User user = new User();
            user = db.Users.Where(x => x.username == username).FirstOrDefault();

            Question_Definition guven_question = new Question_Definition();
            guven_question = db.Question_Definitions.Where(x => x.question_id == 1).FirstOrDefault();
            Question_Definition aktivite_question = new Question_Definition();
            aktivite_question = db.Question_Definitions.Where(x => x.question_id == 2).FirstOrDefault();
            Question_Definition yonetim_question = new Question_Definition();
            yonetim_question = db.Question_Definitions.Where(x => x.question_id == 3).FirstOrDefault();

            if (model.aktivite_alani_score != 0 && model.yonetim_memnuniyeti_score != 0 && model.yonetim_memnuniyeti_score != 0 && model.comment != null)
            {
                

                Survey quiz1 = new Survey();

                quiz1.user_id = user.user_id;
                quiz1.user = user;
                quiz1.question_id = guven_question.question_id;
                quiz1.question_definitioın = guven_question;
                quiz1.place_id = place.place_id;
                quiz1.place = place;
                quiz1.score = model.guven_puani_score;
                quiz1.createdOn = DateTime.Now;
                quiz1.IsActive = true;


                db.Surveys.Add(quiz1);

               

                Survey quiz2 = new Survey();

                quiz2.user_id = user.user_id;
                quiz2.user = user;
                quiz2.question_id = aktivite_question.question_id;
                quiz2.question_definitioın = aktivite_question;
                quiz2.place_id = place.place_id;
                quiz2.place = place;
                quiz2.score = model.aktivite_alani_score;
                quiz2.createdOn = DateTime.Now;
                quiz2.IsActive = true;

                db.Surveys.Add(quiz2);

                

                Survey quiz3 = new Survey();

                quiz3.user_id = user.user_id;
                quiz3.user = user;
                quiz3.question_id = yonetim_question.question_id;
                quiz3.question_definitioın = yonetim_question;
                quiz3.place_id = place.place_id;
                quiz3.place = place;
                quiz3.score = model.yonetim_memnuniyeti_score;
                quiz3.createdOn = DateTime.Now;
                quiz3.IsActive = true;

                db.Surveys.Add(quiz3);



                Comment comments = new Comment();
                comments.user_id = user.user_id;
                comments.user = user;
                comments.place_id = place.place_id;
                comments.place = place;
                comments.text = model.comment;
                comments.createdOn = DateTime.Now;
                comments.IsActive = true;

                db.Comments.Add(comments);

                place.surveys.Add(quiz1);
                place.surveys.Add(quiz2);
                place.surveys.Add(quiz3);
                place.comments.Add(comments);

                user.surveys.Add(quiz1);
                user.surveys.Add(quiz2);
                user.surveys.Add(quiz3);
                user.comments.Add(comments);


                
                ViewBag.status = db.SaveChanges();

                if (redis.IsSet(place.place_id.ToString()))
                {
                    redis.Remove(place.place_id.ToString());
                }


            }


            
            return RedirectToAction("PlaceProfile",new{placeId = place.place_id});


        }

        [HttpGet]
        public ActionResult EditComment()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return PartialView("~/Views/Shared/_PartialComment.cshtml");
        }

        [HttpPost]
        public ActionResult EditComment(PlaceWithoutSurveys model, int? placeId)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            TempData["post"] = "ok";
            Place place = new Place();
            place = db.Places.Where(x => x.place_id == placeId).FirstOrDefault();
            string username = Session["User"].ToString();
            User user = new User();
            user = db.Users.Where(x => x.username == username).FirstOrDefault();

            Question_Definition guven_question = new Question_Definition();
            guven_question = db.Question_Definitions.Where(x => x.question_id == 1).FirstOrDefault();
            Question_Definition aktivite_question = new Question_Definition();
            aktivite_question = db.Question_Definitions.Where(x => x.question_id == 2).FirstOrDefault();
            Question_Definition yonetim_question = new Question_Definition();
            yonetim_question = db.Question_Definitions.Where(x => x.question_id == 3).FirstOrDefault();

            if (model.aktivite_alani_score != 0 && model.yonetim_memnuniyeti_score != 0 && model.yonetim_memnuniyeti_score != 0 && model.comment != null)
            {


                Survey quiz1 = db.Surveys.Where(x => x.user_id == user.user_id && x.place_id == placeId && x.question_id == 1).FirstOrDefault();

                quiz1.user_id = user.user_id;
                quiz1.user = user;
                quiz1.question_id = guven_question.question_id;
                quiz1.question_definitioın = guven_question;
                quiz1.place_id = place.place_id;
                quiz1.place = place;
                quiz1.score = model.guven_puani_score;
                quiz1.createdOn = DateTime.Now;
                quiz1.IsActive = true;


                db.Surveys.AddOrUpdate(quiz1);



                Survey quiz2 = db.Surveys.Where(x => x.user_id == user.user_id && x.place_id == placeId && x.question_id == 2).FirstOrDefault(); ;

                quiz2.user_id = user.user_id;
                quiz2.user = user;
                quiz2.question_id = aktivite_question.question_id;
                quiz2.question_definitioın = aktivite_question;
                quiz2.place_id = place.place_id;
                quiz2.place = place;
                quiz2.score = model.aktivite_alani_score;
                quiz2.createdOn = DateTime.Now;
                quiz2.IsActive = true;

                db.Surveys.AddOrUpdate(quiz2);



                Survey quiz3 = db.Surveys.Where(x => x.user_id == user.user_id && x.place_id == placeId && x.question_id == 3).FirstOrDefault(); ;

                quiz3.user_id = user.user_id;
                quiz3.user = user;
                quiz3.question_id = yonetim_question.question_id;
                quiz3.question_definitioın = yonetim_question;
                quiz3.place_id = place.place_id;
                quiz3.place = place;
                quiz3.score = model.yonetim_memnuniyeti_score;
                quiz3.createdOn = DateTime.Now;
                quiz3.IsActive = true;

                db.Surveys.AddOrUpdate(quiz3);



                Comment comments = db.Comments.Where(x => x.user_id == user.user_id && x.place_id == placeId).FirstOrDefault(); ;

                comments.user_id = user.user_id;
                comments.user = user;
                comments.place_id = place.place_id;
                comments.place = place;
                comments.text = model.comment;
                comments.createdOn = DateTime.Now;
                comments.IsActive = true;

                db.Comments.AddOrUpdate(comments);

               



                ViewBag.status = db.SaveChanges();




            }

            if (redis.IsSet(place.place_id.ToString()))
            {
                redis.Remove(place.place_id.ToString());
                var commentsAndPoints = commentHelper(db, place);
                var getJson = Newtonsoft.Json.JsonConvert.SerializeObject(commentsAndPoints);
                redis.setKey(place.place_id.ToString(), getJson, 3600);

            }
            
            return RedirectToAction("PlaceProfile",new{placeId = place.place_id});
        }


        public ActionResult DeleteComment(PlaceWithoutSurveys model, int? placeId)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            TempData["post"] = "not";
            Place place = new Place();
            place = db.Places.Where(x => x.place_id == placeId).FirstOrDefault();
            string username = Session["User"].ToString();
            User user = new User();
            user = db.Users.Where(x => x.username == username).FirstOrDefault();

            Survey quiz1 = db.Surveys.Where(x => x.user_id == user.user_id && x.place_id == placeId && x.question_id == 1).FirstOrDefault();
            Survey quiz2 = db.Surveys.Where(x => x.user_id == user.user_id && x.place_id == placeId && x.question_id == 2).FirstOrDefault();
            Survey quiz3 = db.Surveys.Where(x => x.user_id == user.user_id && x.place_id == placeId && x.question_id == 3).FirstOrDefault();
            Comment comment = db.Comments.Where(x => x.user_id == user.user_id && x.place_id == placeId ).FirstOrDefault(); ;
            if (quiz1 != null && quiz2 != null && quiz3 != null && comment  != null)
            {
                db.Surveys.Remove(quiz1);
                db.Surveys.Remove(quiz2);
                db.Surveys.Remove(quiz3);
                db.Comments.Remove(comment);
            }

            db.SaveChanges();
            if (redis.IsSet(place.place_id.ToString()))
            {
                redis.Remove(place.place_id.ToString());
            }


            return RedirectToAction("PlaceProfile",new{placeId = place.place_id});
        }

        public ActionResult DeActivatedProfileError()
        {
            return View();
        }

    }
}