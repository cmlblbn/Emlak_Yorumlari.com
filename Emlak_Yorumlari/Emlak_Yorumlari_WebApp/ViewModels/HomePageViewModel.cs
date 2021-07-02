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



        public string Keyword { get; set; }

        [DisplayName("Şehir"), Required(ErrorMessage = "{0} alanı boş geçilemez!")]
        public string city_ddl { get; set; }
        [DisplayName("İlçe"), Required(ErrorMessage = "{0} alanı boş geçilemez!")]
        public string district_ddl { get; set; }
        [DisplayName("Mahalle"), Required(ErrorMessage = "{0} alanı boş geçilemez!")]
        public string quarter_ddl { get; set; }


        public int selectedCityId { get; set; }
        public int SelectedDistrictId { get; set; }
        public int SelectedQuarterId { get; set; }

        public SelectList CityData { get; set; }
        public SelectList DistrictData { get; set; }
        public SelectList QuarterData { get; set; }

    }
}