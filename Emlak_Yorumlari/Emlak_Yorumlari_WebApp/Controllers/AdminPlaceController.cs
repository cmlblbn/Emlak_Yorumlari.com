using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Emlak_Yorumlari_Entities;
using Emlak_Yorumlari_Entities.Models;
using Emlak_Yorumlari_WebApp.ViewModels;

namespace Emlak_Yorumlari_WebApp.Controllers
{
    public class AdminPlaceController : Controller
    {
        private MyContext db = new MyContext();

        // GET: AdminPlace
        public ActionResult Adminplace()
        {
            if (Session["User"] == null && Session["Admin"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            HomePageViewModel model = new HomePageViewModel();
            model.places = db.Places.ToList();
            Adress_Description mahalle = new Adress_Description();
            Adress_Description ilce = new Adress_Description();
            Adress_Description sehir = new Adress_Description();
            MyContext sorgu = new MyContext();

            string birlesmisAdres;
            model.birlesmisAdresDict = new Dictionary<int, string>();
            foreach (var place in model.places)
            {
                birlesmisAdres = "";
                mahalle = sorgu.Adress_Descriptions.Where(x => x.adress_desc_id == place.adress_desc_id)
                    .FirstOrDefault();
                ilce = sorgu.Adress_Descriptions.Where(x => x.adress_desc_id == mahalle.parent_id).FirstOrDefault();
                sehir = sorgu.Adress_Descriptions.Where(x => x.adress_desc_id == ilce.parent_id).FirstOrDefault();

                birlesmisAdres = birlesmisAdres + sehir.adress_name + " / ";
                birlesmisAdres = birlesmisAdres + ilce.adress_name + " / ";
                birlesmisAdres = birlesmisAdres + mahalle.adress_name;
                model.birlesmisAdresDict[place.place_id] = birlesmisAdres;
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


            return View(model);


        }

        [HttpPost]
        public ActionResult AdminPlace(HomePageViewModel model)
        {
            if (Session["User"] == null && Session["Admin"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            int? selectedCityId = null;
            int? selectedDistrictId = null;
            int? selectedQuarterId = null;

            if (model.city_ddl != null)
            {
                selectedCityId = int.Parse(model.city_ddl);
            }
            if (model.district_ddl != null)
            {
                selectedDistrictId = int.Parse(model.district_ddl);
            }
            if (model.quarter_ddl != null)
            {
                selectedQuarterId = int.Parse(model.quarter_ddl);
            }


            if (model.SearchText == null && selectedCityId == null && selectedDistrictId == null && selectedQuarterId == null)
            {
                if (model.isActivate)
                {
                    var query = from p in db.Places.ToList() where p.IsActive == model.isActivate select p;
                    model.places = query.ToList();
                }
                else
                {
                    var query = from p in db.Places.ToList() select p;
                    model.places = query.ToList();
                }

            }
            else if (model.SearchText == null && selectedCityId != null && selectedDistrictId == 0 && selectedQuarterId == null)
            {
                List<Adress_Description> sorguMahalleList = new List<Adress_Description>();
                List<Adress_Description> sorguMahalle = new List<Adress_Description>();
                var sorguIlce = db.Adress_Descriptions.Where(x => x.parent_id == selectedCityId).ToList();
                foreach (var data in sorguIlce)
                {
                    sorguMahalle = db.Adress_Descriptions.Where(x => x.parent_id == data.adress_desc_id).ToList();
                    sorguMahalleList.AddRange(sorguMahalle);
                }

                Place q = new Place();
                List<Place> filter = new List<Place>();
                if (model.isActivate)
                {
                    foreach (var mahalleiter in sorguMahalleList)
                    {
                        q = db.Places.Where(x => x.adress_desc_id == mahalleiter.adress_desc_id && x.IsActive == model.isActivate).FirstOrDefault();
                        if (q != null)
                        {
                            filter.Add(q);
                        }

                    }

                }
                else
                {
                    foreach (var mahalleiter in sorguMahalleList)
                    {
                        q = db.Places.Where(x => x.adress_desc_id == mahalleiter.adress_desc_id).FirstOrDefault();
                        if (q != null)
                        {
                            filter.Add(q);
                        }

                    }

                }



                model.places = filter;

            }
            else if (model.SearchText == null && selectedCityId != null && selectedDistrictId != null && selectedQuarterId == 0)
            {
                var sorguMahalle = db.Adress_Descriptions.Where(x => x.parent_id == selectedDistrictId).ToList();
                Place q = new Place();
                List<Place> filter = new List<Place>();
                if (model.isActivate)
                {
                    foreach (var mahalleiter in sorguMahalle)
                    {
                        q = db.Places.Where(x => x.adress_desc_id == mahalleiter.adress_desc_id && x.IsActive == model.isActivate).FirstOrDefault();
                        if (q != null)
                        {
                            filter.Add(q);
                        }
                    }
                }
                else
                {
                    foreach (var mahalleiter in sorguMahalle)
                    {
                        q = db.Places.Where(x => x.adress_desc_id == mahalleiter.adress_desc_id).FirstOrDefault();
                        if (q != null)
                        {
                            filter.Add(q);
                        }
                    }
                }


                model.places = filter;
            }
            else if (model.SearchText == null && selectedCityId != null && selectedDistrictId != null && selectedQuarterId > 0)
            {
                if (model.isActivate)
                {
                    var query = from p in db.Places
                        where p.adress_desc_id == selectedQuarterId && p.IsActive == model.isActivate
                        select p;
                    model.places = query.ToList();
                }
                else
                {
                    var query = from p in db.Places
                        where p.adress_desc_id == selectedQuarterId
                        select p;
                    model.places = query.ToList();
                }
                


            }
            else if (model.SearchText != null && selectedCityId == null && selectedDistrictId == null && selectedQuarterId == null)
            {
                if (model.isActivate)
                {
                    var query = from p in db.Places
                        where p.placeName.Contains(model.SearchText) && p.IsActive == model.isActivate
                        select p;
                    model.places = query.ToList();
                }
                else
                {
                    var query = from p in db.Places
                        where p.placeName.Contains(model.SearchText)
                        select p;
                    model.places = query.ToList();
                }
                
            }


            else if (model.SearchText != null && selectedCityId != null && selectedDistrictId == 0 && selectedQuarterId == null)
            {
                List<Adress_Description> sorguMahalleList = new List<Adress_Description>();
                List<Adress_Description> sorguMahalle = new List<Adress_Description>();
                var sorguIlce = db.Adress_Descriptions.Where(x => x.parent_id == selectedCityId).ToList();
                foreach (var data in sorguIlce)
                {
                    sorguMahalle = db.Adress_Descriptions.Where(x => x.parent_id == data.adress_desc_id).ToList();
                    sorguMahalleList.AddRange(sorguMahalle);
                }

                Place q = new Place();
                List<Place> filter = new List<Place>();
                if (model.isActivate)
                {
                    foreach (var mahalleiter in sorguMahalleList)
                    {
                        q = db.Places.Where(x => x.placeName.Contains(model.SearchText) && x.adress_desc_id == mahalleiter.adress_desc_id && x.IsActive == model.isActivate).FirstOrDefault();
                        if (q != null)
                        {
                            filter.Add(q);
                        }

                    }
                }
                else
                {
                    foreach (var mahalleiter in sorguMahalleList)
                    {
                        q = db.Places.Where(x => x.placeName.Contains(model.SearchText) && x.adress_desc_id == mahalleiter.adress_desc_id).FirstOrDefault();
                        if (q != null)
                        {
                            filter.Add(q);
                        }

                    }
                }


                model.places = filter;
            }


            else if (model.SearchText != null && selectedCityId != null && selectedDistrictId != null && selectedQuarterId == 0)
            {

                var sorguMahalle = db.Adress_Descriptions.Where(x => x.parent_id == selectedDistrictId).ToList();
                Place q = new Place();
                List<Place> filter = new List<Place>();
                if (model.isActivate)
                {
                    foreach (var mahalleiter in sorguMahalle)
                    {
                        q = db.Places.Where(x => x.placeName.Contains(model.SearchText) && x.adress_desc_id == mahalleiter.adress_desc_id && x.IsActive == model.isActivate).FirstOrDefault();
                        if (q != null)
                        {
                            filter.Add(q);
                        }
                    }
                }
                else
                {
                    foreach (var mahalleiter in sorguMahalle)
                    {
                        q = db.Places.Where(x => x.placeName.Contains(model.SearchText) && x.adress_desc_id == mahalleiter.adress_desc_id).FirstOrDefault();
                        if (q != null)
                        {
                            filter.Add(q);
                        }
                    }
                }

                model.places = filter;
            }


            else
            {
                if (model.isActivate)
                {
                    var query = from p in db.Places
                        where p.placeName.Contains(model.SearchText) && p.adress_desc_id == selectedQuarterId && p.IsActive == model.isActivate
                        select p;
                    model.places = query.ToList();
                }
                else
                {
                    var query = from p in db.Places
                        where p.placeName.Contains(model.SearchText) && p.adress_desc_id == selectedQuarterId
                        select p;
                    model.places = query.ToList();
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

            Adress_Description mahalle = new Adress_Description();
            Adress_Description ilce = new Adress_Description();
            Adress_Description sehir = new Adress_Description();
            MyContext sorgu = new MyContext();

            string birlesmisAdres;
            model.birlesmisAdresDict = new Dictionary<int, string>();
            foreach (var place in model.places)
            {
                birlesmisAdres = "";
                mahalle = sorgu.Adress_Descriptions.Where(x => x.adress_desc_id == place.adress_desc_id).FirstOrDefault();
                ilce = sorgu.Adress_Descriptions.Where(x => x.adress_desc_id == mahalle.parent_id).FirstOrDefault();
                sehir = sorgu.Adress_Descriptions.Where(x => x.adress_desc_id == ilce.parent_id).FirstOrDefault();

                birlesmisAdres = birlesmisAdres + sehir.adress_name + " / ";
                birlesmisAdres = birlesmisAdres + ilce.adress_name + " / ";
                birlesmisAdres = birlesmisAdres + mahalle.adress_name;
                model.birlesmisAdresDict[place.place_id] = birlesmisAdres;
            }

            return View(model);
        }

        public ActionResult AdminEdit(int? id)
        {
            if (Session["User"] == null && Session["Admin"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

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
        public ActionResult AdminEdit(Place place, HttpPostedFileBase uploadfile)
        {
            if (Session["User"] == null && Session["Admin"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

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

                    if (!uploadfile.FileName
                        .EndsWith(".png")) //| !uploadfile.FileName.EndsWith(".jpg") | !uploadfile.FileName.EndsWith(".jpeg"))
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
                return RedirectToAction("Adminplace");
            }

            //ViewBag.adress_desc_id = new SelectList(db.Adress_Descriptions, "adress_desc_id", "adress_name", place.adress_desc_id);
            //ViewBag.user_id = new SelectList(db.Users, "user_id", "username", place.user_id);
            return View(place);
        }

        public ActionResult AdminDelete(int? id)
        {
            if (Session["User"] == null && Session["Admin"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
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
            return RedirectToAction("Adminplace");
        }

        public ActionResult PlaceActivate(int? id)
        {
            if (id != null)
            {
                Place activatePlace = new Place();
                activatePlace = db.Places.Where(x => x.place_id == id).FirstOrDefault();
                activatePlace.IsActive = true;
                db.SaveChanges();
            }

            return RedirectToAction("Adminplace");
        }

        public ActionResult PlaceDeActivate(int? id)
        {
            if (id != null)
            {
                Place activatePlace = new Place();
                activatePlace = db.Places.Where(x => x.place_id == id).FirstOrDefault();
                activatePlace.IsActive = false;
                db.SaveChanges();
            }

            return RedirectToAction("Adminplace");
        }


    }
}