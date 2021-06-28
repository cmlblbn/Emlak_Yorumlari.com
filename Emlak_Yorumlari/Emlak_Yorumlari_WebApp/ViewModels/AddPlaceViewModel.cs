using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Emlak_Yorumlari_Entities.Models;
namespace Emlak_Yorumlari_WebApp.ViewModels
{
    public class AddPlaceViewModel
    {
        [DisplayName("Site adı"), Required(ErrorMessage = "{0} alanı boş geçilemez!"), StringLength(100, ErrorMessage = "{0} max {1} karater olmalı!")]
        public string placename { get; set; }

        [DisplayName("Şehir"), Required(ErrorMessage = "{0} alanı boş geçilemez!")]
        public string city_ddl { get; set; }
        [DisplayName("İlçe"), Required(ErrorMessage = "{0} alanı boş geçilemez!")]
        public string district_ddl { get; set; }
        [DisplayName("Mahalle"), Required(ErrorMessage = "{0} alanı boş geçilemez!")]
        public string quarter_ddl { get; set; }

        public byte[] placeImage { get; set; }


        public int selectedCityId { get; set; }
        public int SelectedDistrictId { get; set; }
        public int SelectedQuarterId { get; set; }

        public SelectList CityData { get; set; }
        public SelectList DistrictData { get; set; }
        public SelectList QuarterData { get; set; }


    }
}