using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Emlak_Yorumlari_Entities;

namespace Emlak_Yorumlari_WebApp.ViewModels
{
    public class HomePageViewModel
    {
        public List<Place> places { get; set; }
        public Dictionary<int,float> mainPoints { get; set; }
        public Dictionary<int,string> birlesmisAdresDict { get; set; }
        public string SearchText { get; set; }

        public string city_ddl { get; set; }
        public string district_ddl { get; set; }
        public string quarter_ddl { get; set; }

        public SelectList CityData { get; set; }
        public SelectList DistrictData { get; set; }
        public SelectList QuarterData { get; set; }

        public bool isActivate { get; set; }

    }
}