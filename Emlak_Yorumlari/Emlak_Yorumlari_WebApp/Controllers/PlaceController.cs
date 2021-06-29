﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Emalk_Yorumlari_Redis;
using Emlak_Yorumlari_WebApp.ViewModels;
using Emlak_Yorumlari_Entities;
using Emlak_Yorumlari_Entities.Models;

namespace Emlak_Yorumlari_WebApp.Controllers
{
    public class PlaceController : Controller
    {
        // GET: Place
        MyContext db = new MyContext();
        private RedisManager redis = new RedisManager();
        [HttpGet]
        public ActionResult AddPlace()
        {
            if (Session["User"] == null)
            {
                RedirectToAction("Index", "Home");
            }

            
            List<Adress_Description> sehirler = new List<Adress_Description>();
            
            foreach (var adress in db.Adress_Descriptions.Where(x => x.parent_id == 0))
            {
                sehirler.Add(adress);
            }



            Adress_Description ilcesecim = new Adress_Description();
            ilcesecim.adress_desc_id = 1;
            ilcesecim.adress_name = " ";

            Adress_Description mahallesecim = new Adress_Description();
            mahallesecim.adress_desc_id = 2;
            mahallesecim.adress_name =  " ";

            List<Adress_Description> ilceSec = new List<Adress_Description>();
            ilceSec.Add(ilcesecim);
            List<Adress_Description> mahalleSec = new List<Adress_Description>();
            mahalleSec.Add(mahallesecim);

            AddPlaceViewModel model = new AddPlaceViewModel()
            {
                city_ddl   = "dropdownSehir",
                district_ddl = "dropdownIlce",
                quarter_ddl = "dropdownMahalle",
                CityData = new SelectList(sehirler,"adress_desc_id","adress_name"),
                DistrictData = new SelectList(ilceSec, "adress_desc_id", "adress_name"),
                QuarterData = new SelectList(mahalleSec, "adress_desc_id", "adress_name"),
                selectedCityId = -1,
                SelectedDistrictId = -1,
                SelectedQuarterId = -1,
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult AddPlace(AddPlaceViewModel model, HttpPostedFileBase uploadfile)
        {
            if (ModelState.IsValid)
            {
                if (uploadfile != null)
                {

                    if (!uploadfile.FileName.EndsWith(".png")) //| !uploadfile.FileName.EndsWith(".jpg") | !uploadfile.FileName.EndsWith(".jpeg"))
                    {
                        ModelState.AddModelError("", "Lütfen fotoğraf seçin! (.png-.jpg-.jpeg)");
                    }
                }

                if (Int32.Parse(model.quarter_ddl) == 0)
                {
                    ModelState.AddModelError("", "Doğru bir adres seçimi yapınız");
                }
                foreach (var item in ModelState)
                {
                    if (item.Value.Errors.Count > 0)
                    {
                       
                        return View(model);
                    }
                }

                User kisi = new User();
                string sessionname = Session["User"].ToString();
                kisi = db.Users.Where(x => x.username == sessionname).FirstOrDefault();
                Adress_Description adres = new Adress_Description();
                int model_adres_id = Int32.Parse(model.quarter_ddl);
                adres = db.Adress_Descriptions.Where(x => x.adress_desc_id == model_adres_id).FirstOrDefault();

                Place place = new Place();
                place.adress_desc_id = model_adres_id;
                place.adress_description = adres;
                
                place.user_id = kisi.user_id;
                place.user = kisi;

                place.placeName = model.placename;
                place.IsActive = true;
                place.createdOn = DateTime.Now;

                HttpPostedFileBase file = Request.Files["uploadfile"];
                model.placeImage = HomeController.ConvertToBytes(file);
                place.placeImage = model.placeImage;

                db.Places.Add(place);
                int status = db.SaveChanges();
                ViewBag.status = -1;
                if (status > 0)
                {
                    ViewBag.status = status;
                    ViewBag.message = "Site Ekleme Başarılı, Anasayfaya Yönlendiriliyorsunuz...";
                }
                else
                {
                    ViewBag.status = status;
                    ViewBag.message = "Site Ekleme Başarısız, Tekrar deneyin.";
                }

            }

            List<Adress_Description> sehirler = new List<Adress_Description>();

            foreach (var adress in db.Adress_Descriptions.Where(x => x.parent_id == 0))
            {
                sehirler.Add(adress);
            }

            Adress_Description ilcesecim = new Adress_Description();
            ilcesecim.adress_desc_id = 1;
            ilcesecim.adress_name = " ";

            Adress_Description mahallesecim = new Adress_Description();
            mahallesecim.adress_desc_id = 2;
            mahallesecim.adress_name = " ";

            List<Adress_Description> ilceSec = new List<Adress_Description>();
            ilceSec.Add(ilcesecim);
            List<Adress_Description> mahalleSec = new List<Adress_Description>();
            mahalleSec.Add(mahallesecim);

            model.city_ddl = "dropdownSehir";
            model.district_ddl = "dropdownIlce";
            model.quarter_ddl = "dropdownMahalle";
            model.CityData = new SelectList(sehirler, "adress_desc_id", "adress_name");
            model.DistrictData = new SelectList(ilceSec, "adress_desc_id", "adress_name");
            model.QuarterData = new SelectList(mahalleSec, "adress_desc_id", "adress_name");
            model.selectedCityId = -1;
            model.SelectedDistrictId = -1;
            model.SelectedQuarterId = -1;
            

            return View(model);
        }




        public JsonResult SetDropResult(int p)
        {
            string redisKey = p.ToString();
            if (redis.IsSet(redisKey))
            {
                string redisResult = redis.getKey(redisKey);
                return Json(redisResult, JsonRequestBehavior.AllowGet);
            }


            object ilceler = (from x in db.Adress_Descriptions
                where x.parent_id == (p)
                select new
                {
                    Text = x.adress_name,
                    Value = x.adress_desc_id.ToString()
                }).ToArray();

            var result = Newtonsoft.Json.JsonConvert.SerializeObject(ilceler);
            redis.setKey(redisKey, result, 10);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
    }
}