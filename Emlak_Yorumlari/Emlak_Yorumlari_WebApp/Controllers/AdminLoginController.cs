using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Emlak_Yorumlari.Models;
using Emlak_Yorumlari_Entities;
using Emlak_Yorumlari_Entities.Models;
using Emlak_Yorumlari_WebApp.ViewModels;
using Emalk_Yorumlari_Redis;

namespace Emlak_Yorumlari_WebApp.Controllers
{
    public class AdminLoginController : Controller
    {
        private MyContext db = new MyContext();
        private RedisManager redis = new RedisManager();

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
                if (redis.IsSet("Statistic"))
                {
                    //redis.Remove("Statistic");
                    //redis.Remove("Daily");
                    //redis.Remove("Weekly");
                    //redis.Remove("Monthly");
                    //redis.Remove("All");
                    return RedirectToAction("Adminplace", "AdminPlace");

                }
                else
                {
                    string trigger =  "Daily Weekly Monthly All" ;
                    redis.setKey("Statistic",trigger,1440);
                    var param = trigger.Split(' ');
                    setDailyStatisticCache(param[0]);
                    setWeeklyStatisticCache(param[1]);
                    setMonthlyStatisticCache(param[2]);
                    setAllStatisticCache(param[3]);
                    return RedirectToAction("Adminplace", "AdminPlace");
                }

            }

            return View(model);
        }

        public void setDailyStatisticCache(string cacheKey)
        {
            string cacheData = "";
            string totalUserDaily = db.Users.Where(x => x.createOn.Day == DateTime.Now.Day - 1).Count().ToString();
            string totalActiveUserDaily = db.Users.Where(x => x.createOn.Day == DateTime.Now.Day - 1 && x.IsActive).Count().ToString();
            string totalPlaceDaily = db.Users.Where(x => x.createOn.Day == DateTime.Now.Day - 1).Count().ToString();
            string totalCommentDaily = db.Comments.Where(x => x.createdOn.Day == DateTime.Now.Day - 1).Count().ToString();

            string activeUserPlaceRatiostr = "";
            float activeUserPlaceRatio = (float)(db.Places.Where(x => x.IsActive && x.createdOn.Day == DateTime.Now.Day - 1).Count()) / (db.Users.Where(x => x.IsActive && x.createOn.Day == DateTime.Now.Day - 1).Count());
            activeUserPlaceRatio = (float)Math.Round(activeUserPlaceRatio * 100f) / 100f;
            activeUserPlaceRatiostr = activeUserPlaceRatio.ToString();
            if(activeUserPlaceRatiostr == "∞" || activeUserPlaceRatiostr == "NaN")
            {
                activeUserPlaceRatiostr = "0";
            }

            string activeUserCommentRatiostr = "";
            float activeUserCommentRatio = (float)(db.Comments.Where(x => x.IsActive && x.createdOn.Day == DateTime.Now.Day - 1).Count()) / (db.Users.Where(x => x.IsActive && x.createOn.Day == DateTime.Now.Day - 1).Count());
            activeUserCommentRatio = (float)Math.Round(activeUserCommentRatio * 100f) / 100f;
            activeUserCommentRatiostr = activeUserCommentRatio.ToString();
            if(activeUserPlaceRatiostr == "∞" || activeUserCommentRatiostr == "NaN")
            {
                activeUserCommentRatiostr = "0";
            }

            cacheData = totalUserDaily + " " + totalActiveUserDaily + " " + totalPlaceDaily + " " + totalCommentDaily + " " + activeUserPlaceRatiostr + " " + activeUserCommentRatiostr;

            redis.setKey(cacheKey, cacheData, 1440);
        }
        public void setWeeklyStatisticCache(string cacheKey)
        {
            string cacheData = "";
            string totalUserDaily = db.Users.Where(x => x.createOn.Day >= (DateTime.Now.Day - 7)).Count().ToString();
            string totalActiveUserDaily = db.Users.Where(x => x.createOn.Day >= (DateTime.Now.Day - 7) && x.IsActive).Count().ToString();
            string totalPlaceDaily = db.Users.Where(x => x.createOn.Day >= (DateTime.Now.Day - 7)).Count().ToString();
            string totalCommentDaily = db.Comments.Where(x => x.createdOn.Day >= (DateTime.Now.Day - 7)).Count().ToString();

            string activeUserPlaceRatiostr = "";
            float activeUserPlaceRatio = (float)(db.Places.Where(x => x.IsActive && x.createdOn.Day >=(DateTime.Now.Day - 7)).Count()) / (db.Users.Where(x => x.IsActive && x.createOn.Day >= (DateTime.Now.Day - 7)).Count());
            activeUserPlaceRatio = (float)Math.Round(activeUserPlaceRatio * 100f) / 100f;
            activeUserPlaceRatiostr = activeUserPlaceRatio.ToString();
            if (activeUserPlaceRatiostr == "∞" || activeUserPlaceRatiostr == "NaN")
            {
                activeUserPlaceRatiostr = "0";
            }

            string activeUserCommentRatiostr = "";
            float activeUserCommentRatio = (float)(db.Comments.Where(x => x.IsActive && x.createdOn.Day >= (DateTime.Now.Day - 7)).Count()) / (db.Users.Where(x => x.IsActive && x.createOn.Day >= (DateTime.Now.Day - 7)).Count());
            activeUserCommentRatio = (float)Math.Round(activeUserCommentRatio * 100f) / 100f;
            activeUserCommentRatiostr = activeUserCommentRatio.ToString();
            if (activeUserPlaceRatiostr == "∞" || activeUserCommentRatiostr == "NaN")
            {
                activeUserCommentRatiostr = "0";
            }

            cacheData = totalUserDaily + " " + totalActiveUserDaily + " " + totalPlaceDaily + " " + totalCommentDaily + " " + activeUserPlaceRatiostr + " " + activeUserCommentRatiostr;

            redis.setKey(cacheKey, cacheData, 1440);
        }
        public void setMonthlyStatisticCache(string cacheKey)
        {
            string cacheData = "";
            string totalUserDaily = db.Users.Where(x => x.createOn.Month >= (DateTime.Now.Month - 1)).Count().ToString();
            string totalActiveUserDaily = db.Users.Where(x => x.createOn.Month >= (DateTime.Now.Month - 1) && x.IsActive).Count().ToString();
            string totalPlaceDaily = db.Users.Where(x => x.createOn.Month >= (DateTime.Now.Month - 1)).Count().ToString();
            string totalCommentDaily = db.Comments.Where(x => x.createdOn.Month >= (DateTime.Now.Month - 1)).Count().ToString();

            string activeUserPlaceRatiostr = "";
            float activeUserPlaceRatio = (float)(db.Places.Where(x => x.IsActive && x.createdOn.Month >= (DateTime.Now.Month - 1)).Count()) / (db.Users.Where(x => x.IsActive && x.createOn.Month >= (DateTime.Now.Month - 1)).Count());
            activeUserPlaceRatio = (float)Math.Round(activeUserPlaceRatio * 100f) / 100f;
            activeUserPlaceRatiostr = activeUserPlaceRatio.ToString();
            if (activeUserPlaceRatiostr == "∞" || activeUserPlaceRatiostr == "NaN")
            {
                activeUserPlaceRatiostr = "0";
            }

            string activeUserCommentRatiostr = "";
            float activeUserCommentRatio = (float)(db.Comments.Where(x => x.IsActive && x.createdOn.Month >= (DateTime.Now.Month - 1)).Count()) / (db.Users.Where(x => x.IsActive && x.createOn.Month >= (DateTime.Now.Month - 1)).Count());
            activeUserCommentRatio = (float)Math.Round(activeUserCommentRatio * 100f) / 100f;
            activeUserCommentRatiostr = activeUserCommentRatio.ToString();
            if (activeUserPlaceRatiostr == "∞" || activeUserCommentRatiostr == "NaN")
            {
                activeUserCommentRatiostr = "0";
            }

            cacheData = totalUserDaily + " " + totalActiveUserDaily + " " + totalPlaceDaily + " " + totalCommentDaily + " " + activeUserPlaceRatiostr + " " + activeUserCommentRatiostr;

            redis.setKey(cacheKey, cacheData, 1440);

        }
        public void setAllStatisticCache(string cacheKey)
        {
            string cacheData = "";
            string totalUserDaily = db.Users.Count().ToString();
            string totalActiveUserDaily = db.Users.Where(x => x.IsActive).Count().ToString();
            string totalPlaceDaily = db.Users.Count().ToString();
            string totalCommentDaily = db.Comments.Count().ToString();

            string activeUserPlaceRatiostr = "";
            float activeUserPlaceRatio = (float)(db.Places.Where(x => x.IsActive).Count()) / (db.Users.Where(x => x.IsActive).Count());
            activeUserPlaceRatio = (float)Math.Round(activeUserPlaceRatio * 100f) / 100f;
            activeUserPlaceRatiostr = activeUserPlaceRatio.ToString();
            if (activeUserPlaceRatiostr == "∞" || activeUserPlaceRatiostr == "NaN")
            {
                activeUserPlaceRatiostr = "0";
            }

            string activeUserCommentRatiostr = "";
            float activeUserCommentRatio = (float)(db.Comments.Where(x => x.IsActive).Count()) / (db.Users.Where(x => x.IsActive).Count());
            activeUserCommentRatio = (float)Math.Round(activeUserCommentRatio * 100f) / 100f;
            activeUserCommentRatiostr = activeUserCommentRatio.ToString();
            if (activeUserPlaceRatiostr == "∞" || activeUserCommentRatiostr == "NaN")
            {
                activeUserCommentRatiostr = "0";
            }

            cacheData = totalUserDaily + " " + totalActiveUserDaily + " " + totalPlaceDaily + " " + totalCommentDaily + " " + activeUserPlaceRatiostr + " " + activeUserCommentRatiostr;

            redis.setKey(cacheKey, cacheData, 1440);

        }
    }
    
}