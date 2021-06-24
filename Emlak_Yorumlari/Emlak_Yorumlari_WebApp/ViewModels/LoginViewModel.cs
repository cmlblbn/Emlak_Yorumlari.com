using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Emlak_Yorumlari_WebApp.ViewModels
{
    public class LoginViewModel
    {
        [DisplayName("Kullanıcı adı"), Required(ErrorMessage = "{0} alanı boş geçilemez!"), StringLength(25, ErrorMessage = "{0} max {1} karater olmalı!")]
        public string username { get; set; }

        [DisplayName("Şifre"), Required(ErrorMessage = "{0} alanı boş geçilemez!"), DataType(DataType.Password), StringLength(12, ErrorMessage = "{0} max {1} karater olmalı!")]
        public string password { get; set; }
    }
}