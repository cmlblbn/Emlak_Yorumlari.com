using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Emlak_Yorumlari_Entities;
using Emlak_Yorumlari_Entities.Models;

namespace Emlak_Yorumlari_WebApp.ViewModels
{
    public class PlaceProfileViewModel
    {
        public virtual Place place { get; set; }
        public float guven_puani_mainscore { get; set; }
        public float aktivite_alani_mainscore { get; set; }
        public float yonetim_memnuniyeti_mainscore { get; set; }




        [DisplayName("Yorum"), Required(ErrorMessage = "{0} alanı boş geçilemez!"), StringLength(10000, ErrorMessage = "{0} alanı max {1} karater olmalı!")]
        public string text { get; set; }

        [DisplayName("Güven Puanı"), Required(ErrorMessage = "{0} alanı boş geçilemez!")]
        public float guven_puani_score { get; set; }

        [DisplayName("Aktivite Alanı"), Required(ErrorMessage = "{0} alanı boş geçilemez!")]
        public float aktivite_alani_score { get; set; }

        [DisplayName("Yönetim Memnuniyeti"), Required(ErrorMessage = "{0} alanı boş geçilemez!")]
        public float yonetim_memnuniyeti_score { get; set; }


        
    }
}