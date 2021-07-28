using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Emlak_Yorumlari_WebApp.Controllers
{
    public class AdminStatisticsController : Controller
    {
        // GET: AdminStatistics
        public ActionResult StatisticsPage()
        {
            return View();
        }
    }
}