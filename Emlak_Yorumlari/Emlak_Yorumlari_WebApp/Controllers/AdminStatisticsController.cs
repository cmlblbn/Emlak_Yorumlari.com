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
using System.Web.Helpers;


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

        public ActionResult UserChart()
        {
            var dataStringDay = redis.getKey("Daily");
            var dataStringWeek = redis.getKey("Weekly");
            var dataStringMonth = redis.getKey("Monthly");
            var dataStringLegend = redis.getKey("Statistic");
            var dataDay = dataStringDay.Split(' ');
            var dataWeek = dataStringWeek.Split(' ');
            var dataMonth = dataStringMonth.Split(' ');
            var dataLegend = dataStringLegend.Split(' ');

            int[] chartData_yAxis = new int[3];
            chartData_yAxis[0] = int.Parse(dataDay[1]);
            chartData_yAxis[1] = int.Parse(dataWeek[1]);
            chartData_yAxis[2] = int.Parse(dataMonth[1]);
            string[] chartData_xAxis = new string[3];
            int totalData = chartData_yAxis[0] + chartData_yAxis[1] + chartData_yAxis[2];
            float xDataElement = 0;
            for (int i = 0; i < chartData_yAxis.Length; i++)
            {
                xDataElement = (float) chartData_yAxis[i] / totalData;
                xDataElement =  (float)Math.Round(xDataElement * 100f) / 100f;
                xDataElement = xDataElement * 100;
                chartData_xAxis[i] =dataLegend[i] + " %" + xDataElement.ToString();

            }


            Chart chart = new Chart(350, 350,theme:ChartTheme.Yellow);
            chart.AddTitle("Emlak yorumları - Aktif Kullanıcı Grafiği");
            chart.AddSeries(name: "UserActivate", chartType: "Doughnut",
                xValue:chartData_xAxis,
                yValues: chartData_yAxis);
            chart.AddLegend();

            return View(chart);
        }

        public ActionResult PlaceChart()
        {
            var dataStringDay = redis.getKey("Daily");
            var dataStringWeek = redis.getKey("Weekly");
            var dataStringMonth = redis.getKey("Monthly");
            var dataStringLegend = redis.getKey("Statistic");
            var dataDay = dataStringDay.Split(' ');
            var dataWeek = dataStringWeek.Split(' ');
            var dataMonth = dataStringMonth.Split(' ');
            var dataLegend = dataStringLegend.Split(' ');

            int[] chartData_yAxis = new int[3];
            chartData_yAxis[0] = int.Parse(dataDay[2]);
            chartData_yAxis[1] = int.Parse(dataWeek[2]);
            chartData_yAxis[2] = int.Parse(dataMonth[2]);
            string[] chartData_xAxis = new string[3];
            int totalData = chartData_yAxis[0] + chartData_yAxis[1] + chartData_yAxis[2];
            float xDataElement = 0;
            for (int i = 0; i < chartData_yAxis.Length; i++)
            {
                xDataElement = (float)chartData_yAxis[i] / totalData;
                xDataElement = (float)Math.Round(xDataElement * 100f) / 100f;
                xDataElement = xDataElement * 100;
                chartData_xAxis[i] = dataLegend[i] + " %" + xDataElement.ToString();

            }

            Chart chart = new Chart(350, 350, theme: ChartTheme.Yellow);
            chart.AddTitle("Emlak yorumları - Site Grafiği");
            chart.AddSeries(name: "Place", chartType: "Doughnut",
                xValue: chartData_xAxis,
                yValues: chartData_yAxis);
            chart.AddLegend();

            return View(chart);
        }


    }
}