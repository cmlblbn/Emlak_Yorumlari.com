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
    public class MLopsModelController : Controller
    {
        private MyContext db = new MyContext();

        static async Task<bool> SendRequestTrain(Uri u, HttpContent c)
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

        static async Task<string> SendRequestPredict(Uri u, HttpContent c)
        {
            string text = string.Empty;
            using (var client = new HttpClient())
            {
                Task<string> response;
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = u,
                    Content = c
                };

                HttpResponseMessage result = await client.SendAsync(request);
                if (result.IsSuccessStatusCode)
                {
                    response = result.Content.ReadAsStringAsync();
                    text = response.Result;
                    return text;
                }
            }
            return text;
        }

        // GET: MLopsModel
        public ActionResult MLopsModel()
        {
            if (Session["User"] == null && Session["Admin"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            MLopsModelViewModel data = new MLopsModelViewModel();
            data.models = db.Models.ToList();
            data.models = data.models.OrderBy(x => x.model_id).ToList();
            data.activeModel_id = data.models.Where(x => x.isActive).FirstOrDefault().model_id;
            
            return View(data);
        }

        [HttpPost]
        public ActionResult MLopsModel(MLopsModelViewModel data)
        {
            if(data != null)
            {
                if(data.maxlen != 0 && data.batch_size != 0 && data.epoch != 0)
                {
                    Uri u = new Uri("http://localhost:4444/train/");

                    var willTrain = new Dictionary<string, string>
                    {
                      {"type",data.model_type},
                      {"maxlen",data.maxlen.ToString()},
                      {"batch_size",data.batch_size.ToString()},
                      {"epoch",data.epoch.ToString()},
                    };

                    string strWillTrain = Newtonsoft.Json.JsonConvert.SerializeObject(willTrain);

                    HttpContent c = new StringContent(strWillTrain, Encoding.UTF8, "application/json");
                    var t = Task.Run(() => SendRequestTrain(u, c));
                    t.Wait();
                    bool status = t.Result;
                    data.models = db.Models.ToList();
                    data.models = data.models.OrderBy(x => x.model_id).ToList();
                    data.activeModel_id = data.models.Where(x => x.isActive).FirstOrDefault().model_id;
                    data.currTrainStatus = status;

                    return View(data);
                }
                else
                {
                    ModelState.AddModelError("", "Doğru parametre girişi yapınız.");

                    data.models = db.Models.ToList();
                    data.models = data.models.OrderBy(x => x.model_id).ToList();
                    data.activeModel_id = data.models.Where(x => x.isActive).FirstOrDefault().model_id;
                    

                    return View(data);
                }
                //Call async function to make a training. make api and call it here.


            }
            return RedirectToAction("MLopsModel", "MLopsModel");
        }

        public ActionResult ModelActivate(int? model_id)
        {
            if (Session["User"] == null && Session["Admin"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (model_id != null)
            {
                Model model = db.Models.Where(x => x.isActive).FirstOrDefault();
                model.isActive = false;
                db.Entry(model).State = EntityState.Modified;

                model = db.Models.Where(x => x.model_id == model_id).FirstOrDefault();
                model.isActive = true;
                db.Entry(model).State = EntityState.Modified;

                db.SaveChanges();
            }
            return RedirectToAction("ModelDetail", "MLopsModel", new { model_id });
        }

        public ActionResult ModelDetail(int? model_id)
        {
            if(model_id != null)
            {
                ModelDetailViewModel data = new ModelDetailViewModel();
                Model model = db.Models.Where(x => x.model_id == model_id).FirstOrDefault();
                data.model = model;
                data.predict = "";
                return View(data);
            }
            return View();

        }




        [HttpPost]
        public ActionResult ModelDetail(ModelDetailViewModel data, int? model_id)
        {
            if(model_id != null)
            {
                var modelId = Convert.ToInt32(model_id);
                string text = data.predictText;
                //Call async function to make a prediction.

               
                Model model = db.Models.Where(x => x.model_id == model_id).FirstOrDefault();
                data.model = model;
                data.predictText = text;
                if(data.predictText != null)
                {
                    Uri u = new Uri("http://localhost:4444/specPredict/");

                    var willPredict = new Dictionary<string, string>
                    {
                      {"model_id",model_id.ToString()},
                      {"text",data.predictText}
                    };

                    string strWillPredict = Newtonsoft.Json.JsonConvert.SerializeObject(willPredict);

                    HttpContent c = new StringContent(strWillPredict, Encoding.UTF8, "application/json");
                    var t = Task.Run(() => SendRequestPredict(u, c));
                    t.Wait();
                    var predictionobj = Newtonsoft.Json.JsonConvert.DeserializeObject<prediction>(t.Result);
                    data.predict = predictionobj.response;
                }

                return View(data);
            }

            return RedirectToAction("ModelDetail", "MLopsModel",new { model_id });
        }
    }
}