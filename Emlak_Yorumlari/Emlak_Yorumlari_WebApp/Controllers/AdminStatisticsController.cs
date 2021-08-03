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
        public Dictionary<int, string> referenceValueByStats = new Dictionary<int, string>()
        {
                {0, "male_count"},
                {1, "female_count"},
                {2, "otherSex_count"},
                {3, "primarySchool_count"},
                {4, "middleSchool_count"},
                {5, "highSchool_count"},
                {6, "degree_count"},
                {7, "masterDegree_count"},
                {8, "married_count"},
                {9, "single_count"},
                {10, "divorced_count"},
                {11, "widow_count"},
                {12, "average_rent"},
                {13, "age_lower_18"},
                {14, "age_between_18_34"},
                {15, "age_between_34_55"},
                {16, "age_upper_55"}
        };
        private MyContext db = new MyContext();
        private RedisManager redis = new RedisManager();
        // GET: AdminStatistics

        public static int[] getStatsByPlace(Place place)
        {
            MyContext db_statistic = new MyContext();
            int[] Stats = new int[17];
            var PlaceData = db_statistic.Surveys.Where(x => x.place_id == place.place_id).ToList();
            if(PlaceData == null)
            {
                return Stats;
            }
            int totalRent = 0;
            foreach(var data in PlaceData)
            {
                if(data.question_id == 5)
                {

                    if(data.score == 2)
                    {
                        Stats[0] += 1;
                    }
                    else if(data.score == 3)
                    {
                        Stats[1] += 1;
                    }
                    else if(data.score == 4)
                    {
                        Stats[2] += 1;
                    }
                }
                else if(data.question_id == 9)
                {
                    if(data.score == 6)
                    {
                        Stats[3] += 1; 
                    }
                    else if(data.score == 7)
                    {
                        Stats[4] += 1;
                    }
                    else if(data.score == 8)
                    {
                        Stats[5] += 1;
                    }
                    else if(data.score == 9)
                    {
                        Stats[6] += 1;
                    }
                    else if(data.score == 10)
                    {
                        Stats[7] += 1;
                    }
                }
                else if(data.question_id == 10)
                {
                    if(data.score == 11)
                    {
                        Stats[8] += 1;
                    }
                    else if(data.score == 12)
                    {
                        Stats[9] += 1;
                    }
                    else if(data.score == 13)
                    {
                        Stats[10] += 1;
                    }
                    else if(data.score == 14)
                    {
                        Stats[11] += 1;
                    }
                }
                else if(data.question_id == 11)
                {
                    Stats[12] += (int)data.score;
                    totalRent += 1;
                }
                else if(data.question_id == 6)
                {
                    if(data.score <= 18)
                    {
                        Stats[13] += 1;
                    }
                    else if(data.score > 18 && data.score <= 34)
                    {
                        Stats[14] += 1;
                    }
                    else if(data.score > 34 && data.score <= 55)
                    {
                        Stats[15] += 1;
                    }
                    else if(data.score > 55 && data.score <= 100)
                    {
                        Stats[16] += 1;
                    }
                }
                
            }
            if(totalRent == 0)
            {
                Stats[12] = 0;
            }
            else
            {
                Stats[12] = Stats[12] / totalRent;
                totalRent = 0;
            }

            db_statistic = null;
            return Stats;
        }

        public static void getScheduleStatistic()
        {
            MyContext db_append = new MyContext();
            var places = db_append.Places.ToList();
            foreach(var place in places)
            {
                var Stats = getStatsByPlace(place);
                Place_Statistics statistics = new Place_Statistics() {
                    place = place,
                    place_id = place.place_id,
                    male_count = Stats[0],
                    female_count = Stats[1],
                    otherSex_count = Stats[2],
                    primarySchool_count = Stats[3],
                    middleSchool_count = Stats[4],
                    highSchool_count = Stats[5],
                    degree_count = Stats[6],
                    masterDegree_count = Stats[7],
                    married_count = Stats[8],
                    single_count = Stats[9],
                    divorced_count = Stats[10],
                    widow_count = Stats[11],
                    average_rent = Stats[12],
                    age_lower_18 = Stats[13],
                    age_between_18_34 = Stats[14],
                    age_between_34_55 = Stats[15],
                    age_upper_55 = Stats[16],
                    createdOn = DateTime.Now,
                    IsActive = true
                };

                db_append.Place_Statistics.Add(statistics);
                //db_append.SaveChanges();
            }



        }

        public ActionResult StatisticsPage()
        {
            //getScheduleStatistic();
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