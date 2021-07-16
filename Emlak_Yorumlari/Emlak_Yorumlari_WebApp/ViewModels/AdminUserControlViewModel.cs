using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Emlak_Yorumlari_Entities;

namespace Emlak_Yorumlari_WebApp.ViewModels
{
    public class AdminUserControlViewModel
    {
        public User user { get; set; }
        public int placesCount { get; set; }
        public int commentsCount { get; set; }

        public string SearchText { get; set; }
        public bool isActivate { get; set; }


        public List<AdminUserControlViewModel> ClassList { get; set; }


    }
}