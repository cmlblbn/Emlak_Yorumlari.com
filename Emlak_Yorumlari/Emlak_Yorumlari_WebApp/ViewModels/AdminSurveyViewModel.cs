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
    public class AdminSurveyViewModel
    {
        [DisplayName("Soru Adı"), Required(ErrorMessage = "{0} alanı boş geçilemez!"), StringLength(50, ErrorMessage = "{0} alanı max {1} karater olmalı!")]
        public string questionName { get; set; }

        [DisplayName("Soru Tipi"), Required(ErrorMessage = "{0} alanı boş geçilemez!")]
        public string questionType { get; set; }

        [DisplayName("Combo Box Cevapları"), Required(ErrorMessage = "{0} alanı boş geçilemez!"), StringLength(50, ErrorMessage = "{0} alanı max {1} karater olmalı!")]
        public string comboBoxAnswers { get; set; }

        public List<Question_Definition> question_list { get; set; }
    }
}