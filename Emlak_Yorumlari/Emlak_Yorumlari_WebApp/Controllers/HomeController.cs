using Emlak_Yorumlari_Entities;
using Emlak_Yorumlari_WebApp.ViewModels;
using System;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Emalk_Yorumlari_Redis;

namespace Emlak_Yorumlari_WebApp.Controllers
{

    public class HomeController : Controller
    {
        RedisManager redis = new RedisManager();
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
                
                MyContext db = new MyContext();
                User kontrolkisi = null;
                kontrolkisi = db.Users.Where(x => x.username == model.username).FirstOrDefault();
                if (kontrolkisi != null) { 
                if (kontrolkisi.IsActive == false)
                {
                    ModelState.AddModelError("", "Hesabınız aktif değil!");
                }
                }

                kontrolkisi = db.Users.Where(x => x.username == model.username).FirstOrDefault();
                if (kontrolkisi == null)
                {
                    ModelState.AddModelError("","Kullanıcı adı yanlış!");
                }

                string hashpsw = Crypto.SHA256(model.password);
                hashpsw = Rotate(hashpsw, 10);

                kontrolkisi = db.Users.Where(x => x.password == hashpsw).FirstOrDefault();

                if (kontrolkisi == null)
                {
                    ModelState.AddModelError("", "Şifre yanlış!");
                }

                //kontrolkisi = db.Users.Where(x => x.IsActive == false).FirstOrDefault();

              

                foreach (var item in ModelState)
                {
                    if (item.Value.Errors.Count > 0)
                    {
                        
                        return View(model);
                    }
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
            bool hata = false;
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

                if (uploadfile != null)
                {

                    if (!uploadfile.FileName.EndsWith(".png")) //| !uploadfile.FileName.EndsWith(".jpg") | !uploadfile.FileName.EndsWith(".jpeg"))
                    {
                        ModelState.AddModelError("", "Lütfen fotoğraf seçin! (.png-.jpg-.jpeg)");
                    }
                }
                
                foreach (var item in ModelState)
                {
                    if (item.Value.Errors.Count > 0)
                    {
                        hata = true;
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
                kisi.IsActive = false;
                kisi.password = hashpsw;
                kisi.email = model.email;
                kisi.createOn = DateTime.Now.Date;
                kisi.profileImage = model.profileImage;
                context.Users.Add(kisi);
                SendActivationEmail(kisi);
                

                int status = context.SaveChanges();

                if (status > 0)
                {
                    ViewBag.status = status;
                    ViewBag.message = "Üyelik başarılı, e-mail adresinize aktivasyon kodu gönderildi. Yönlendiriliyorsunuz....";
                }
                else
                {
                    ViewBag.status = status;
                    ViewBag.message = "Üyelik Başarısız, Tekrar deneyin.";
                }
            }
            
            return View(model);
            
        }

        private void SendActivationEmail(User kontrolkisi)
        {
            WebMail.SmtpServer = "smtp.gmail.com";
            WebMail.SmtpPort = 587;
            WebMail.UserName = "meunotes1@gmail.com";
            WebMail.Password = "meunotes1.Meu";
            WebMail.EnableSsl = true;
            
            string Url = Rotate(Crypto.SHA256(kontrolkisi.username).ToString(), 8);
            redis.setKey(Url, kontrolkisi.username,10);
            try
            {
                WebMail.Send(
                    to: kontrolkisi.email, subject: "Aktivasyon",
                    body: "\nSayın :" + kontrolkisi.username + "<br/> Hesabınızı aktifleştirmek için lütfen bağlantıya tıklayın, "+"<br/>"
                         + string.Format("{0}://{1}/Home/UserActivate/{2}", Request.Url.Scheme, Request.Url.Authority, Url),
                    replyTo: "dont-reply@gmail.com", isBodyHtml: true);
            }
            catch (Exception e)
            {
            }
        }

        public ActionResult UserActivate()
        {
            if (RouteData.Values["id"] != null)
            {
                string query = RouteData.Values["id"].ToString();

                if (redis.IsSet(query) )
                {
                    string kod = redis.getKey(query);
                    MyContext context = new MyContext();
                    User kontrolkisi = context.Users.Where(x => x.username == kod).FirstOrDefault();
                    if (kontrolkisi == null)
                    {
                        ViewBag.status = 1;
                    }
                    kontrolkisi.IsActive = true;
                    context.SaveChanges();
                    return View();
                }
                else
                {
                    return RedirectToAction("UserActivateTimeout");
                }
                
                //ViewBag.Bilgi = "Aktivasyon başarılı.";
            }
            return View();
        }

        [HttpGet]
        public ActionResult UserActivateTimeout()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserActivateTimeout(ForgotMyPassViewModel model)
        {
            if (ModelState.IsValid)
            {
                MyContext db = new MyContext();
                User kisi = null;

                kisi = db.Users.Where(x => x.email == model.email).FirstOrDefault();
                if (kisi != null)
                {
                    SendActivationEmail(kisi);
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError("", "Geçerli bir e posta adresi giriniz.");

                return View(model);
            }

            return View(model);
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