using Emlak_Yorumlari.Models;
using Emlak_Yorumlari_Entities;
using Emlak_Yorumlari_WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Emlak_Yorumlari_WebApp.Controllers
{
    public class MLopsLabelingController : Controller
    {
        private MyContext db = new MyContext();
        // GET: MLopsLabeling
        public ActionResult MLopsLabeling()
        {

            if (Session["User"] == null && Session["Admin"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            MLopsLabelViewModel model = new MLopsLabelViewModel();
            model.data = db.Comment_Logs.Where(x => x.isLabeled == false).ToList();
            return View(model);
        }


        [HttpPost]
        public ActionResult MLopsLabeling(MLopsLabelViewModel model, int? user_id,int? place_id)
        {
            Dictionary<int, string> classes = new Dictionary<int, string>();
            
            if(user_id != null && place_id != null)
            {
                Comment_Log loggedCommend = db.Comment_Logs.Where(x => x.user_id == user_id && x.place_id == place_id).FirstOrDefault();
                Embedding embedding = new Embedding();
                embedding.text = loggedCommend.text;
                embedding.prediction_sentiment = loggedCommend.toxic_type;
                if(model.Label != null)
                {
                    embedding.actual_sentiment = Convert.ToInt32(model.Label);
                }
                else
                {
                    embedding.actual_sentiment = loggedCommend.toxic_type;
                }
                embedding.isTrained = false;
                embedding.createdOn = DateTime.Now.Date;
                embedding.isActive = true;
                db.Embeddings.Add(embedding);

                
                loggedCommend.isLabeled = true;
                db.Entry(loggedCommend).State = EntityState.Modified;

                db.SaveChanges();
            }

            return RedirectToAction("MLopsLabeling", "MLopsLabeling");
        }
    }
}