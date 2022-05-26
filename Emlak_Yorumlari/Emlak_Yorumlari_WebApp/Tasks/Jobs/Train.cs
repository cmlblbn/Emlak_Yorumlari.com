using Emlak_Yorumlari_WebApp.Controllers;
using Emlak_Yorumlari.Models;
using Emlak_Yorumlari_Entities;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;

namespace Emlak_Yorumlari_WebApp.Tasks.Jobs
{
    public class Train : IJob
    {
        private MyContext db = new MyContext();

        private static async Task<bool> SendRequestTrain(Uri u, HttpContent c)
        {

            using (var client = new HttpClient())
            {

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = u,
                    Content = c
                };

                HttpResponseMessage result = await client.SendAsync(request);
                if (result.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            return false;
        }

        private static void trainAsync(Crobjob_Parameter jobValue)
        {
            Uri u = new Uri("http://localhost:4444/train/");

            var willTrain = new Dictionary<string, string>
                    {
                      {"type",jobValue.type},
                      {"maxlen",jobValue.maxlen.ToString()},
                      {"batch_size",jobValue.batch_size.ToString()},
                      {"epoch",jobValue.epoch.ToString()},
                    };

            string strWillTrain = Newtonsoft.Json.JsonConvert.SerializeObject(willTrain);

            HttpContent c = new StringContent(strWillTrain, Encoding.UTF8, "application/json");
            var t = Task.Run(() => SendRequestTrain(u, c));
            t.Wait();
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Crobjob_Parameter jobValue = db.Cronjob_Parameters.Where(x => x.cronjob_id == 1).FirstOrDefault();
                trainAsync(jobValue);
            }
            catch (Exception)
            {
                //Do not anything...
            }
        }

    }
}