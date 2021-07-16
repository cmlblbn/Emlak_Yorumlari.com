using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Emlak_Yorumlari_Entities;
using Emlak_Yorumlari_WebApp.ViewModels;

namespace Emlak_Yorumlari_WebApp.Controllers
{
    public class AdminLoginController : Controller
    {
        // GET: AdminLogin
        [HttpGet]
        public ActionResult AdminLogin()
        {
            Session["Admin"] = null;
            return View();
        }

        [HttpPost]
        public ActionResult AdminLogin(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                MyContext db = new MyContext();
                User kontrolkisi = null;
                kontrolkisi = db.Users.Where(x => x.username == model.username).FirstOrDefault();
                if (kontrolkisi != null)
                {
                    if (kontrolkisi.IsActive == false)
                    {
                        ModelState.AddModelError("", "Hesabınız aktif değil!");
                    }
                }
                string hashpsw = Crypto.SHA256(model.password);
                hashpsw = HomeController.Rotate(hashpsw, 10);
                kontrolkisi = db.Users.Where(x => x.username == model.username && x.password == hashpsw).FirstOrDefault();
                if (kontrolkisi == null)
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya Şifre Yanlış");
                }
                else
                {
                    if (kontrolkisi.IsAdmin == false)
                    {
                        ModelState.AddModelError("", "Yetkili Kullanıcı Değilsiniz!");
                    }
                }
                
                foreach (var item in ModelState)
                {
                    if (item.Value.Errors.Count > 0)
                    {

                        return View(model);
                    }
                }

                Session["Admin"] = model.username;
                return RedirectToAction("Adminplace","AdminPlace");
            }

            return View(model);
        }
    }
    
}