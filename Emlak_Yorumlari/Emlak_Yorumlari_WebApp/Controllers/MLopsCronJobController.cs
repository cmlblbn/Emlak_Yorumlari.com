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
    public class MLopsCronJobController : Controller
    {

        private MyContext db = new MyContext();
        // GET: MLopsCronJob
        public ActionResult MLopsCronJob()
        {
            if (Session["User"] == null && Session["Admin"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            CronJobViewModel model = new CronJobViewModel();
            Crobjob_Parameter data = null;
            data = db.Cronjob_Parameters.Where(x => x.cronjob_id == 1).FirstOrDefault();

            model.type = data.type;
            model.maxlen = data.maxlen;
            model.batch_size = data.batch_size;
            model.epoch = data.epoch;

            return View(model);
        }

        [HttpPost]
        public ActionResult MLopsCronJob(CronJobViewModel model)
        {
            Crobjob_Parameter data = null;
            data = db.Cronjob_Parameters.Where(x => x.cronjob_id == 1).FirstOrDefault();

            data.type = model.type;
            data.maxlen = model.maxlen;
            data.batch_size = model.batch_size;
            data.epoch = model.epoch;

            db.Entry(data).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("MLopsCronJob", "MLopsCronJob");
        }
    }
}