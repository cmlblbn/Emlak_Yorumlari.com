using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        /*[HttpPost] //post işlemi yapılmadı
        public ActionResult Login()
        {
            return View();
        }*/

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        /*[HttpPost] // post işlemi yapılmadı
        public ActionResult Register()
        {
            return View();
        }*/
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