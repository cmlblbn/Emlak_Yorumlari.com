using Emlak_Yorumlari_WebApp.Tasks.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Emlak_Yorumlari_WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //Cron Jobs Schedule
            getPlaceStatisticsTrigger.Trigger();
            TrainTrigger.Trigger();
        }
    }
}
