using Emalk_Yorumlari_Redis;
using Emlak_Yorumlari_Entities;
using Emlak_Yorumlari_Entities.Models;
using Emlak_Yorumlari_WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Emlak_Yorumlari_WebApp.Controllers
{
    public class PlaceController : Controller
    {
        // GET: Place
        private MyContext db = new MyContext();

        private RedisManager redis = new RedisManager();

        [HttpGet]
        public ActionResult AddPlace()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Home");
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

            AddPlaceViewModel model = new AddPlaceViewModel()
            {
                city_ddl = "dropdownSehir",
                district_ddl = "dropdownIlce",
                quarter_ddl = "dropdownMahalle",
                CityData = new SelectList(sehirler, "adress_desc_id", "adress_name"),
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
                kisi.places.Add(place);
                adres.places.Add(place);
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
            if (p == 1)
            {
                redisKey = "adana";
            }
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


        public ActionResult ShowPlace()
        {
            User kontrolkisi = new User();
            string sessionname = Session["User"].ToString();
            kontrolkisi = db.Users.Where(x => x.username == sessionname).FirstOrDefault();

            var places = db.Places.Where(p => p.user_id == kontrolkisi.user_id);

            Dictionary<int, string> tamAdres = new Dictionary<int, string>();

            Adress_Description mahalle = new Adress_Description();
            Adress_Description ilce = new Adress_Description();
            Adress_Description sehir = new Adress_Description();
            MyContext sorgu = new MyContext();
            string birlesmisAdres;
            foreach (var placeIter in places)
            {
                birlesmisAdres = "";
                mahalle = sorgu.Adress_Descriptions.Where(x => x.adress_desc_id == placeIter.adress_desc_id).FirstOrDefault();
                ilce = sorgu.Adress_Descriptions.Where(x => x.adress_desc_id == mahalle.parent_id).FirstOrDefault();
                sehir = sorgu.Adress_Descriptions.Where(x => x.adress_desc_id == ilce.parent_id).FirstOrDefault();

                birlesmisAdres = birlesmisAdres + sehir.adress_name + " - ";
                birlesmisAdres = birlesmisAdres + ilce.adress_name + " - ";
                birlesmisAdres = birlesmisAdres + mahalle.adress_name;
                tamAdres.Add(placeIter.adress_desc_id,birlesmisAdres);
                

            }

            ViewBag.TamAdres = tamAdres;



            // ViewBag.adres=db.Places.Where(p=>p.)
            // var places = db.Places.Include(p => p.adress_description).Include(p => p.user);
            return View(places.ToList());
        }

        

        public ActionResult ReturnImage(int? Id)
        {
            Place place = new Place();
            place = db.Places.Where(x => x.place_id == Id).FirstOrDefault();
            byte[] cover = place.placeImage; 
            if (cover != null)
            {
                return File(cover, "image/jpg");
            }
            else
            {
                return null;
            }
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Place place = db.Places.Find(id);
            if (place == null)
            {
                return HttpNotFound();
            }
            db.Places.Remove(place);
            db.SaveChanges();
            return RedirectToAction("ShowPlace");
        }
        // GET: Place/Edit/5
        public ActionResult Edit(int?id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Place place = db.Places.Find(id);
            if (place == null)
            {
                return HttpNotFound();
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

            AddPlaceViewModel model = new AddPlaceViewModel()
            {
                city_ddl = "dropdownSehir",
                district_ddl = "dropdownIlce",
                quarter_ddl = "dropdownMahalle",
                CityData = new SelectList(sehirler, "adress_desc_id", "adress_name"),
                DistrictData = new SelectList(ilceSec, "adress_desc_id", "adress_name"),
                QuarterData = new SelectList(mahalleSec, "adress_desc_id", "adress_name"),
                selectedCityId = -1,
                SelectedDistrictId = -1,
                SelectedQuarterId = -1,
            };


            ViewBag.city = model.CityData;
            ViewBag.district = model.DistrictData;
            ViewBag.quarter = model.QuarterData;


            return View(place);
        }

        [HttpPost]
        public ActionResult Edit( Place place, HttpPostedFileBase uploadfile)
        {
            var rate = Request["quarter_ddl"].ToString();
            //place.user = db.Users.Where(x => x.user_id == place.user_id).FirstOrDefault();
            //place.adress_desc_id = db.Adress_Descriptions.Where();
            place.adress_desc_id = Convert.ToInt32(rate);
            if (ModelState.IsValid)
            {
                if (place.adress_desc_id == 0)
                {
                    ModelState.AddModelError("", "Lütfen adresi seçin!");
                }
                if (uploadfile != null)
                {

                    if (!uploadfile.FileName.EndsWith(".png")) //| !uploadfile.FileName.EndsWith(".jpg") | !uploadfile.FileName.EndsWith(".jpeg"))
                    {
                        ModelState.AddModelError("", "Lütfen fotoğraf seçin! (.png-.jpg-.jpeg)");
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

                AddPlaceViewModel model = new AddPlaceViewModel()
                {
                    city_ddl = "dropdownSehir",
                    district_ddl = "dropdownIlce",
                    quarter_ddl = "dropdownMahalle",
                    CityData = new SelectList(sehirler, "adress_desc_id", "adress_name"),
                    DistrictData = new SelectList(ilceSec, "adress_desc_id", "adress_name"),
                    QuarterData = new SelectList(mahalleSec, "adress_desc_id", "adress_name"),
                    selectedCityId = -1,
                    SelectedDistrictId = -1,
                    SelectedQuarterId = -1,
                };


                ViewBag.city = model.CityData;
                ViewBag.district = model.DistrictData;
                ViewBag.quarter = model.QuarterData;

                foreach (var item in ModelState)
                {
                    if (item.Value.Errors.Count > 0)
                    {


                        return View(place);
                    }
                }


                HttpPostedFileBase file = Request.Files["uploadfile"];
                place.placeImage = HomeController.ConvertToBytes(file);
                db.Entry(place).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ShowPlace");
            }
            //ViewBag.adress_desc_id = new SelectList(db.Adress_Descriptions, "adress_desc_id", "adress_name", place.adress_desc_id);
            //ViewBag.user_id = new SelectList(db.Users, "user_id", "username", place.user_id);
            return View(place);
        }

        //[HttpPost]
        //public ActionResult Edit(AddPlaceViewModel model, HttpPostedFileBase uploadfile)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        if (uploadfile != null)
        //        {
        //            if (!uploadfile.FileName.EndsWith(".png")) //| !uploadfile.FileName.EndsWith(".jpg") | !uploadfile.FileName.EndsWith(".jpeg"))
        //            {
        //                ModelState.AddModelError("", "Lütfen fotoğraf seçin! (.png-.jpg-.jpeg)");
        //            }
        //        }

        //        if (Int32.Parse(model.quarter_ddl) == 0)
        //        {
        //            ModelState.AddModelError("", "Doğru bir adres seçimi yapınız");
        //        }
        //        foreach (var item in ModelState)
        //        {
        //            if (item.Value.Errors.Count > 0)
        //            {
        //                return View(model);
        //            }
        //        }

        //        User kisi = new User();
        //        string sessionname = Session["User"].ToString();
        //        kisi = db.Users.Where(x => x.username == sessionname).FirstOrDefault();
        //        Adress_Description adres = new Adress_Description();
        //        int model_adres_id = Int32.Parse(model.quarter_ddl);
        //        adres = db.Adress_Descriptions.Where(x => x.adress_desc_id == model_adres_id).FirstOrDefault();

        //        Place place = new Place();
        //        place.adress_desc_id = model_adres_id;
        //        place.adress_description = adres;

        //        place.user_id = kisi.user_id;
        //        place.user = kisi;

        //        place.placeName = model.placename;
        //        place.IsActive = true;
        //        place.createdOn = DateTime.Now;

        //        HttpPostedFileBase file = Request.Files["uploadfile"];
        //        model.placeImage = HomeController.ConvertToBytes(file);
        //        place.placeImage = model.placeImage;

        //        db.Places.Add(place);
        //        int status = db.SaveChanges();
        //        ViewBag.status = -1;
        //        if (status > 0)
        //        {
        //            ViewBag.status = status;
        //            ViewBag.message = "Site Ekleme Başarılı, Anasayfaya Yönlendiriliyorsunuz...";
        //        }
        //        else
        //        {
        //            ViewBag.status = status;
        //            ViewBag.message = "Site Ekleme Başarısız, Tekrar deneyin.";
        //        }
        //    }

        //    List<Adress_Description> sehirler = new List<Adress_Description>();

        //    foreach (var adress in db.Adress_Descriptions.Where(x => x.parent_id == 0))
        //    {
        //        sehirler.Add(adress);
        //    }

        //    Adress_Description ilcesecim = new Adress_Description();
        //    ilcesecim.adress_desc_id = 1;
        //    ilcesecim.adress_name = " ";

        //    Adress_Description mahallesecim = new Adress_Description();
        //    mahallesecim.adress_desc_id = 2;
        //    mahallesecim.adress_name = " ";

        //    List<Adress_Description> ilceSec = new List<Adress_Description>();
        //    ilceSec.Add(ilcesecim);
        //    List<Adress_Description> mahalleSec = new List<Adress_Description>();
        //    mahalleSec.Add(mahallesecim);

        //    model.city_ddl = "dropdownSehir";
        //    model.district_ddl = "dropdownIlce";
        //    model.quarter_ddl = "dropdownMahalle";
        //    model.CityData = new SelectList(sehirler, "adress_desc_id", "adress_name");
        //    model.DistrictData = new SelectList(ilceSec, "adress_desc_id", "adress_name");
        //    model.QuarterData = new SelectList(mahalleSec, "adress_desc_id", "adress_name");
        //    model.selectedCityId = -1;
        //    model.SelectedDistrictId = -1;
        //    model.SelectedQuarterId = -1;

        //    return View(model);
        //}
        //// POST: Place/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "place_id,user_id,adress_desc_id,placeName,placeImage,createdOn,IsActive")] Place place)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(place).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.adress_desc_id = new SelectList(db.Adress_Descriptions, "adress_desc_id", "adress_name", place.adress_desc_id);
        //    ViewBag.user_id = new SelectList(db.Users, "user_id", "username", place.user_id);
        //    return View(place);
        //}
        //// POST: Place/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Place place = db.Places.Find(id);
        //    db.Places.Remove(place);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
    }
}