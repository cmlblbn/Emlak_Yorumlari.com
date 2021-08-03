using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Emlak_Yorumlari_WebApp.Tasks.Jobs;
using Quartz;
using Quartz.Impl;
namespace Emlak_Yorumlari_WebApp.Tasks.Triggers
{
    public class getPlaceStatisticsTrigger
    {
        public static void Trigger()
        {

            StdSchedulerFactory factory = new StdSchedulerFactory();

            IScheduler scheduler = factory.GetScheduler();
            if (!scheduler.IsStarted)
            {
                scheduler.Start();
            }

            IJobDetail task = JobBuilder.Create<getPlaceStatistics>().Build();

            ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                .WithIdentity("getPlaceStatistics",null)
                .WithCronSchedule("0 30 23 * * ? *")
                .Build();

            scheduler.ScheduleJob(task, trigger);
        }
    }
}