using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Emlak_Yorumlari.Models;
using Emlak_Yorumlari_Entities;
using Emlak_Yorumlari_Entities.Models;
using Emlak_Yorumlari_WebApp.ViewModels;
using Emalk_Yorumlari_Redis;
using System.Linq;

namespace Emlak_Yorumlari_WebApp.Controllers
{
    public class MLopsLoginController : Controller
    {
        private MyContext db = new MyContext();
        // GET: MLopsLogin
        public ActionResult MLopsLogin()
        {
            Session["Admin"] = null;
            return View();
            
        }

        [HttpPost]
        public ActionResult MLopsLogin(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {


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
                return RedirectToAction("MLopsEmbedding", "MLopsEmbedding");
            }

            return View(model);
        }
    }
}