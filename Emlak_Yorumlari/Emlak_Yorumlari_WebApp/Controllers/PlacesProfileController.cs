using System;
using System.Collections.Generic;
using System.Linq;
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

        public ActionResult PlaceProfile(int? placeId)
        {
            string username = Session["User"].ToString();
            User user = new User();
            user = db.Users.Where(x => x.username == username).FirstOrDefault();
            Comment kisisorgu = new Comment();
            kisisorgu = db.Comments.Where(x => x.user_id == user.user_id).FirstOrDefault();
            if (kisisorgu != null)
            {
                TempData["post"] = "ok";
            }
            else
            {
                TempData["post"] = "not";
            }
            

            PlaceWithoutSurveys model = new PlaceWithoutSurveys();
            Place place = new Place();
            place = db.Places.Where(x => x.place_id == placeId).FirstOrDefault();

            model.aktivite_alani_mainscore = 5.0f;
            model.guven_puani_mainscore = 4.2f;
            model.yonetim_memnuniyeti_mainscore = 3.5f;

            model.guven_puani_score = 0;
            model.aktivite_alani_score = 0;
            model.yonetim_memnuniyeti_score = 0;
            model.comment = "";
            model.place = place;


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
            TempData["post"] = "ok";
            Place place = new Place();
            place = db.Places.Where(x => x.place_id == placeId).FirstOrDefault();
            string username = Session["User"].ToString();
            User user = new User();
            user = db.Users.Where(x => x.username == username).FirstOrDefault();

            if (ModelState.IsValid)
            {
                

                
                Question_Definition guven_question = new Question_Definition();
                guven_question = db.Question_Definitions.Where(x => x.question_id == 1).FirstOrDefault();
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

                Question_Definition aktivite_question = new Question_Definition();
                aktivite_question = db.Question_Definitions.Where(x => x.question_id == 2).FirstOrDefault();
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

                Question_Definition yonetim_question = new Question_Definition();
                yonetim_question = db.Question_Definitions.Where(x => x.question_id == 3).FirstOrDefault();
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

                //ViewBag.status = db.SaveChanges();

                
            }



            model.place = place;
            return View(model);


        }
    }
}