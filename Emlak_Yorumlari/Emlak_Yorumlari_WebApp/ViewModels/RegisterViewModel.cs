using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Emlak_Yorumlari_WebApp.ViewModels
{
    public class RegisterViewModel
    {
        [DisplayName("Kullanıcı adı"), Required(ErrorMessage = "{0} alanı boş geçilemez!"),StringLength(25,ErrorMessage = "{0} max {1} karater olmalı!")]
        public string username { get; set; }

        [DisplayName("E - posta"), Required(ErrorMessage = "{0} alanı boş geçilemez!"), StringLength(60, ErrorMessage = "{0} max {1} karater olmalı!"),
        EmailAddress(ErrorMessage = "{0} alanı için geçerli bir e-posta adresi giriniz!")]
        public string email { get; set; }

        [DisplayName("Şifre"), Required(ErrorMessage = "{0} alanı boş geçilemez!"), DataType(DataType.Password), StringLength(12, ErrorMessage = "{0} max {1} karater olmalı!")]
        public string password { get; set; }

        [DisplayName("Şifre"), Required(ErrorMessage = "{0} alanı boş geçilemez!"), DataType(DataType.Password), StringLength(12, ErrorMessage = "{0} max {1} karater olmalı!"),
        Compare("password",ErrorMessage = "{0} ile {1} uyuşmuyor!")]
        public string repassword { get; set; }


        public byte[] profileImage { get; set; }
    }
}