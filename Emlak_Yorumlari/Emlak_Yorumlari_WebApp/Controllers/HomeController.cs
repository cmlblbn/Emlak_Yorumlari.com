using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Emlak_Yorumlari_WebApp.ViewModels;

namespace Emlak_Yorumlari_WebApp.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
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







                if (hasError == true)
                {
                    return View(model);
                }
                Session["login"] = model.username;
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost] // post yapıldı veritabanı bağlanmalı
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool hasError = false;







                if (hasError == true)
                {
                    return View(model);
                }
            }

            

            return View(model);
        }

        public ActionResult ForgotMyPassword()
        {
            return View();
        }

        [HttpPost] // şifre sıfırlama kısmı
        public ActionResult ForgotMyPassword(ForgotMyPassViewModel model)
        {
            return View();
        }

        public ActionResult UserActivate()
        {
            //aktivasyon işlemi
            return View();
        }

        public ActionResult Logout()
        {
            //çıkış yapma
            return View();
        }
    }
}