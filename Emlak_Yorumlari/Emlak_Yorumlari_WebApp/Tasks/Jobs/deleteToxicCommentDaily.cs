using Emlak_Yorumlari_WebApp.Controllers;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emlak_Yorumlari_WebApp.Tasks.Jobs
{
    public class deleteToxicCommentDaily : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                AdminCommentController.deleteToxicCommentDaily();
            }
            catch (Exception)
            {
                //Do not anything...
            }
        }
    }
}