using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Emlak_Yorumlari_WebApp.ViewModels
{
    public class ForgotMyPassViewModel
    {
        [DisplayName("E - posta"), Required(ErrorMessage = "{0} alanı boş geçilemez!"), StringLength(60, ErrorMessage = "{0} max {1} karater olmalı!"),
         EmailAddress(ErrorMessage = "{0} alanı için geçerli bir e-posta adresi giriniz!")]
        public string email { get; set; }
    }
}