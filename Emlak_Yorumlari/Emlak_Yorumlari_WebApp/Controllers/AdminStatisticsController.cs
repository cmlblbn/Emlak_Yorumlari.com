using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Emalk_Yorumlari_Redis;
using Emlak_Yorumlari_Entities;
using Emlak_Yorumlari_Entities.Models;
using Emlak_Yorumlari_WebApp.ViewModels;
using Emlak_Yorumlari.Models;
using System.Data.Entity.Migrations;

namespace Emlak_Yorumlari_WebApp.Controllers
{
    public class AdminStatisticsController : Controller
    {
        private MyContext db = new MyContext();
        private RedisManager redis = new RedisManager();
        // GET: AdminStatistics
        public ActionResult StatisticsPage()
        {

            AdminStatisticViewModel model = new AdminStatisticViewModel();
            if (redis.IsSet("Statistic"))
            {
                string dataString = redis.getKey("All");
                string[] data = dataString.Split(' ');
                model.totalUser = int.Parse(data[0]);
                model.totalActiveUser = int.Parse(data[1]);
                model.totalPlace = int.Parse(data[2]);
                model.totalComment = int.Parse(data[3]);
                model.activeUserPlaceRatio = float.Parse(data[4]);
                model.activeUserCommentRatio = float.Parse(data[5]);
                return View(model);
            }
            else
            {
                model.totalUser = db.Users.Count();
                model.totalActiveUser = db.Users.Where(x => x.IsActive).Count();
                model.totalPlace = db.Places.Count();
                model.totalComment = db.Comments.Count();
                int deneme1 = 14;
                int deneme2 = 4;
                float sonuc = (float)deneme2 / deneme1;
                model.activeUserPlaceRatio = (float)(db.Places.Where(x => x.IsActive).Count()) / (db.Users.Where(x => x.IsActive).Count());
                model.activeUserPlaceRatio = (float)Math.Round(model.activeUserPlaceRatio * 100f) / 100f;
                model.activeUserCommentRatio = (float)(db.Comments.Where(x => x.IsActive).Count()) / (db.Users.Where(x => x.IsActive).Count());
                model.activeUserCommentRatio = (float)Math.Round(model.activeUserCommentRatio * 100f) / 100f;
                return View(model);
            }

        }

        public ActionResult Daily()
        {
            if (redis.IsSet("Statistic"))
            {
                AdminStatisticViewModel model = new AdminStatisticViewModel();
                string dataString = redis.getKey("Daily");
                string[] data = dataString.Split(' ');

                model.totalUser = int.Parse(data[0]);
                model.totalActiveUser = int.Parse(data[1]);
                model.totalPlace = int.Parse(data[2]);
                model.totalComment = int.Parse(data[3]);
                model.activeUserPlaceRatio = float.Parse(data[4]);
                model.activeUserCommentRatio = float.Parse(data[5]);
                return View(model);
            }
            else
            {
                return RedirectToAction("StatisticsPage");
            }
            
        }

        public ActionResult Weekly()
        {
            if (redis.IsSet("Statistic"))
            {
                AdminStatisticViewModel model = new AdminStatisticViewModel();
                string dataString = redis.getKey("Weekly");
                string[] data = dataString.Split(' ');

                model.totalUser = int.Parse(data[0]);
                model.totalActiveUser = int.Parse(data[1]);
                model.totalPlace = int.Parse(data[2]);
                model.totalComment = int.Parse(data[3]);
                model.activeUserPlaceRatio = float.Parse(data[4]);
                model.activeUserCommentRatio = float.Parse(data[5]);
                return View(model);
            }
            else
            {
                return RedirectToAction("StatisticsPage");
            }

        }

        public ActionResult Monthly()
        {
            if (redis.IsSet("Statistic"))
            {
                AdminStatisticViewModel model = new AdminStatisticViewModel();
                string dataString = redis.getKey("Monthly");
                string[] data = dataString.Split(' ');

                model.totalUser = int.Parse(data[0]);
                model.totalActiveUser = int.Parse(data[1]);
                model.totalPlace = int.Parse(data[2]);
                model.totalComment = int.Parse(data[3]);
                model.activeUserPlaceRatio = float.Parse(data[4]);
                model.activeUserCommentRatio = float.Parse(data[5]);
                return View(model);
            }
            else
            {
                return RedirectToAction("StatisticsPage");
            }

        }

        public ActionResult All()
        {
            return RedirectToAction("StatisticsPage");
        }
            
        
    }
}