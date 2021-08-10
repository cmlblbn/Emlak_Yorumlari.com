using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Emlak_Yorumlari_WebApp.Tasks.Jobs;
using Quartz;
using Quartz.Impl;

namespace Emlak_Yorumlari_WebApp.Tasks.Triggers
{
    public class deleteToxicCommentDailyTrigger
    {
        public static void Trigger()
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();

            IScheduler scheduler = factory.GetScheduler();
            if (!scheduler.IsStarted)
            {
                scheduler.Start();
            }

            IJobDetail task = JobBuilder.Create<deleteToxicCommentDaily>().Build();

            ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                .WithIdentity("deleteToxicCommentDaily", null)
                .WithCronSchedule("0 59 23 * * ? *")
                .Build();

            scheduler.ScheduleJob(task, trigger);
        }
    }
}