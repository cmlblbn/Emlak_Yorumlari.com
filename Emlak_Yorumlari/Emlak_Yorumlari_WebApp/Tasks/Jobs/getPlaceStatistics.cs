using Emlak_Yorumlari_WebApp.Controllers;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Emlak_Yorumlari_WebApp.Tasks.Jobs
{
    public class getPlaceStatistics : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                AdminStatisticsController.getScheduleStatistic();
            }
            catch (Exception)
            {
                //Do not anything...
            }
     
        }
    }
}