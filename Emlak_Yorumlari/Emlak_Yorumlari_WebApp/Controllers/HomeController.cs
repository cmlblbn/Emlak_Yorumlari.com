using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Emlak_Yorumlari_WebApp.ViewModels;
using Emlak_Yorumlari_Entities;

namespace Emlak_Yorumlari_WebApp.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            Session["User"] = null;

            return View();
        }




        public ActionResult Homepage()
        {
            return View();
        }






        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }






        [HttpPost] //post yapıldı veritabanı bağlanmalı
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool hasError = false;
                MyContext db = new MyContext();
                User kontrolkisi = null;


                kontrolkisi = db.Users.Where(x => x.username == model.username).FirstOrDefault();
                if (kontrolkisi == null)
                {
                    hasError = true;
                }

                string hashpsw = Crypto.SHA256(model.password);
                hashpsw = Rotate(hashpsw, 10);

                kontrolkisi = db.Users.Where(x => x.password == hashpsw).FirstOrDefault();

                if (kontrolkisi == null)
                {
                    hasError = true;
                }

                if (hasError)
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı!!!!");
                    return View(model);
                }

                Session["User"] = model.username;
                return RedirectToAction("Homepage");
            }

            return View(model);
        }






        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }



        [HttpPost] // 
        public ActionResult Register(RegisterViewModel model, HttpPostedFileBase uploadfile)
        {
            if (ModelState.IsValid)
            {
                MyContext db = new MyContext();
                User kontrolkisi = null;

                kontrolkisi = db.Users.Where(x => x.username == model.username).FirstOrDefault();
                if (kontrolkisi != null)
                {
                    if (kontrolkisi.username == model.username)
                    {

                        ModelState.AddModelError("", "Kullanıcı adı kullanılıyor! Başka bir kullanıcı adı seçiniz");

                    }
                }

                kontrolkisi = db.Users.Where(x => x.email == model.email).FirstOrDefault();
                if (kontrolkisi != null)
                {
                    if (kontrolkisi.email == model.email)
                    {

                        ModelState.AddModelError("", "E-mail kullanılıyor! Başka bir e-posta seçiniz");
                    }
                }


                foreach (var item in ModelState)
                {
                    if (item.Value.Errors.Count > 0)
                    {
                        return View(model);
                    }
                }


                HttpPostedFileBase file = Request.Files["uploadfile"];
                model.profileImage = ConvertToBytes(file);
                string hashpsw = Crypto.SHA256(model.password);
                hashpsw = Rotate(hashpsw, 10);
                MyContext context = new MyContext();
                User kisi = new User();
                kisi.username = model.username;
                kisi.IsAdmin = false;
                kisi.IsActive = true;
                kisi.password = hashpsw;
                kisi.email = model.email;
                kisi.createOn = DateTime.Now.Date;
                kisi.profileImage = model.profileImage;
                context.Users.Add(kisi);

                int status = context.SaveChanges();

                if (status > 0)
                {
                    ViewBag.status = status;
                    ViewBag.message = "Üyelik başarılı, e-mail adresinize aktivasyon kodu gönderildi. Yönlendiriliyorsunuz.";
                }
                else
                {
                    ViewBag.status = status;
                    ViewBag.message = "Üyelik Başarısız, Tekrar deneyin.";
                }
            }

            

            return RedirectToAction("Login");
        }



        public byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
        }


        public static string Rotate(string s, int numberOfChars)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            string word = "";
            char[] wordArr = s.ToCharArray();

            for (int i = 0; i <= numberOfChars; i++)
            {
                word = word + wordArr[i];
            }
            return word;
        }





        public ActionResult ForgotMyPassword()
        {
            return View();
        }




        [HttpPost] // şifre sıfırlama kısmı
        public ActionResult ForgotMyPassword(ForgotMyPassViewModel model)
        {
            MyContext db = new MyContext();
            User kontrolkisi = null;

            kontrolkisi = db.Users.Where(x => x.email == model.email).FirstOrDefault();
            if (kontrolkisi == null)
            {
                ModelState.AddModelError("", "Sistemde kayıtlı e-mail adresi bulunamadı");
                return View(model);
            }
            else
            {
                // yeni şifremizi hashcode üzerinden alıyoruz, her seferinde random şifre geliyor...
                string newpsw = Crypto.HashPassword(kontrolkisi.password);
                newpsw = Rotate(newpsw, 8);


                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.SmtpPort = 587;
                WebMail.UserName = "meunotes1@gmail.com";
                WebMail.Password = "meunotes1.Meu";
                WebMail.EnableSsl = true;

                try
                {
                    
                    WebMail.Send(
                        to: kontrolkisi.email, subject: "Yeni Şifre",
                        body: "\nSayın :" + kontrolkisi.username + " Şifreniz sıfırlanmıştır! </br>" + kontrolkisi.email +
                              "\n Kayıtlı e-mail adresinin yeni şifresi : <b>" + newpsw + "</b>",
                        replyTo: "dont-reply@gmail.com", isBodyHtml: true);


                }
                catch (Exception e)
                {
                    
                }

                newpsw = Crypto.SHA256(newpsw);
                newpsw = Rotate(newpsw, 10);



                kontrolkisi.password = newpsw;

                db.Users.AddOrUpdate(kontrolkisi);

                db.SaveChanges();

            }

            return RedirectToAction("Login");
        }








        public ActionResult UserActivate()
        {
            //aktivasyon işlemi
            return View();
        }

        public ActionResult Logout()
        {
            if (Session["User"] != null)
            {
                Session["User"] = null;
            }
            return RedirectToAction("Index");
        }
    }
}