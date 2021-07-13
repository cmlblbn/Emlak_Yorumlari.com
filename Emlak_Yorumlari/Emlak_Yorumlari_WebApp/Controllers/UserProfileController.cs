using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Data.Entity.Migrations;
using System.IO;
using System.Web.Helpers;
using System.Web.Mvc;
using Emalk_Yorumlari_Redis;
using Emlak_Yorumlari_Entities;
using Emlak_Yorumlari_Entities.Models;
using Emlak_Yorumlari_WebApp.ViewModels;

namespace Emlak_Yorumlari_WebApp.Controllers
{
    public class UserProfileController : Controller
    {
        private MyContext db = new MyContext();
        // GET: UserProfile
        public ActionResult Userprofile()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            string username = Session["User"].ToString();
            User user = new User();
            user = db.Users.Where(x => x.username == username).FirstOrDefault();
            UserProfileViewModel model = new UserProfileViewModel();

            model.username = user.username;
            model.email = user.email;
            model.profileImage = user.profileImage;

            List<Comment> comments = new List<Comment>();
            List<Place> places = new List<Place>();
            comments = db.Comments.Where(x => x.user_id == user.user_id).ToList();
            places = db.Places.Where(x => x.user_id == user.user_id).ToList();

            model.commentsCount = comments.Count;
            model.placeCount = places.Count;
            model.user_id = user.user_id;

            return View(model);
        }

        [HttpPost]
        public ActionResult Userprofile(UserProfileViewModel model, HttpPostedFileBase uploadfile)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Home");
            }


            string username = Session["User"].ToString();
            User userupdate = new User();
            userupdate = db.Users.Where(x => x.username == username).FirstOrDefault();

            if (userupdate != null)
            {
                var usercontrol = db.Users.Where(x => x.username != userupdate.username && x.username == model.username).FirstOrDefault();
                if (usercontrol != null)
                {
                    ModelState.AddModelError("", "Kullanıcı adı kullanılıyor! Başka bir kullanıcı adı seçiniz");

                }

            }


            if (userupdate != null)
            {
                var usercontrol = db.Users.Where(x => x.email != userupdate.email && x.email == model.email).FirstOrDefault();
                if (usercontrol != null)
                {
                    ModelState.AddModelError("", "E-mail adresi Kullanılıyor, başka bir e-mail seçiniz");

                }
            }

            if (uploadfile != null)
            {

                if (!(uploadfile.FileName.EndsWith(".png") || uploadfile.FileName.EndsWith(".jpg") || uploadfile.FileName.EndsWith(".jpeg"))) 
                {
                    ModelState.AddModelError("", "Lütfen fotoğraf seçin! (.png-.jpg-.jpeg)");
                }
            }
            List<Comment> comments = new List<Comment>();
            List<Place> places = new List<Place>();
            comments = db.Comments.Where(x => x.user_id == userupdate.user_id).ToList();
            places = db.Places.Where(x => x.user_id == userupdate.user_id).ToList();

            model.commentsCount = comments.Count;
            model.placeCount = places.Count;
            model.user_id = userupdate.user_id;
            foreach (var item in ModelState)
            {
                if (item.Value.Errors.Count > 0)
                {
                    return View(model);
                }
            }

            userupdate.username = model.username;
            userupdate.email = model.email;
            if (uploadfile != null)
            {
                HttpPostedFileBase file = Request.Files["uploadfile"];
                model.profileImage = HomeController.ConvertToBytes(file);
            }
            else
            {
                model.profileImage = userupdate.profileImage;
            }
            userupdate.profileImage = model.profileImage;

            if (model.password != null)
            {
                string hashpsw = Crypto.SHA256(model.password);
                hashpsw = HomeController.Rotate(hashpsw, 10);
                userupdate.password = hashpsw;
            }

            model.password = null;
            model.repassword = null;
            db.SaveChanges();
            
            return View(model);
        }




        public ActionResult ReturnImage(int? Id)
        {
            User user = new User();
            user = db.Users.Where(x => x.user_id == Id).FirstOrDefault();
            byte[] cover = user.profileImage;
            if (cover != null)
            {
                return File(cover, "image/jpg");
            }
            else
            {
                return null;
            }
        }
    }
}