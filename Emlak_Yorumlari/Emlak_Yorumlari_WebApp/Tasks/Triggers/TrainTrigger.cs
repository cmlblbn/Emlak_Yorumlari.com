using Emlak_Yorumlari_WebApp.Tasks.Jobs;
using Quartz;
using Quartz.Impl;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emlak_Yorumlari_WebApp.Tasks.Triggers
{
    public class TrainTrigger
    {
        public static void Trigger()
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();

            IScheduler scheduler = factory.GetScheduler();
            if (!scheduler.IsStarted)
            {
                scheduler.Start();
            }

            IJobDetail task = JobBuilder.Create<Train>().Build();

            ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                .WithIdentity("Train", null)
                .WithCronSchedule("0 59 23 1 1/1 ? *")
                .Build();

            scheduler.ScheduleJob(task, trigger);
        }
    }
}