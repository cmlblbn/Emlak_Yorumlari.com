using Emlak_Yorumlari.Models;
using Emlak_Yorumlari_Entities;
using Emlak_Yorumlari_WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Emlak_Yorumlari_WebApp.Controllers
{
    public class MLopsEmbeddingController : Controller
    {
        private MyContext db = new MyContext();
        // GET: MLopsEmbedding
        public ActionResult MLopsEmbedding()
        {
            if (Session["User"] == null && Session["Admin"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            MLopsEmbeddingViewModel model = new MLopsEmbeddingViewModel();
            model.data = db.Embedding_Analyses.Where(x => x.isActive).OrderByDescending(x => x.lastAnalyseDate).Take(7).ToList();
            model.data.Reverse();

            model.lastAnalyseDate = model.data.LastOrDefault().lastAnalyseDate;
            model.trained_path = model.data.LastOrDefault().trained_path;
            model.nontrained_path = model.data.LastOrDefault().nontrained_path;
            return View(model);
        }

        static async Task<bool> SendURI(Uri u)
        {
            
            using (var client = new HttpClient())
            {
                
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = u
                };

                HttpResponseMessage result = await client.SendAsync(request);
                if (result.IsSuccessStatusCode)
                {

                    return true;
                }
            }
            return false;
        }

        public ActionResult GetEmbedding()
        {
            //Communicate with the fastApi to get a predict from AI

            Uri u = new Uri("http://localhost:4444/drawembedding/");

            var t = Task.Run(() => SendURI(u));
            t.Wait();
            bool result = t.Result;    
            
            
            //make async task here to draw trained and nontrained embedding graphs. Other hand calculate the ml_divergence 
            //report database to this section in async task
            //this function only call async task

            return RedirectToAction("MLopsEmbedding", "MLopsEmbedding");
        }
    }
}