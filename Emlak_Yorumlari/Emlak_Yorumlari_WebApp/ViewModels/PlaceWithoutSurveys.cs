using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Emlak_Yorumlari_Entities;

namespace Emlak_Yorumlari_WebApp.ViewModels
{
    public class PlaceWithoutSurveys
    {
        public Place place { get; set; }
        public Dictionary<string, List<string>> commentsAndPoints { get; set; }
        public User user { get; set; }
        public string question1_name { get; set; }
        public string question2_name { get; set; }
        public string question3_name { get; set; }
        public float guven_puani_mainscore { get; set; }
        public float aktivite_alani_mainscore { get; set; }
        public float yonetim_memnuniyeti_mainscore { get; set; }




        [DisplayName("Yorum"), Required(ErrorMessage = "{0} alanı boş geçilemez!"), StringLength(25, ErrorMessage = "{0} alanı max {1} karater olmalı!")]
        public string comment { get; set; }

        [DisplayName("Güven Puanı"), Required(ErrorMessage = "{0} alanı doldurulmalıdır!")]
        public float guven_puani_score { get; set; }
        [DisplayName("Aktivite Alanı"), Required(ErrorMessage = "{0} alanı doldurulmalıdır!")]
        public float aktivite_alani_score { get; set; }
        [DisplayName("Yönetim Memnuniyeti"), Required(ErrorMessage = "{0} alanı doldurulmalıdır!")]
        public float yonetim_memnuniyeti_score { get; set; }

    }
}