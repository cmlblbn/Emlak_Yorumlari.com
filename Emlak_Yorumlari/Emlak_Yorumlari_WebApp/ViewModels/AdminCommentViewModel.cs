using Emlak_Yorumlari.Models;
using Emlak_Yorumlari_Entities;
using Emlak_Yorumlari_Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emlak_Yorumlari_WebApp.ViewModels
{
    public class AdminCommentViewModel
    {
        public string SearchText { get; set; }
        
        public User user { get; set; }
        public Place place { get; set; }
        public Comment_Log toxicComment { get; set; }

        public List<AdminCommentViewModel> ClassList { get; set; }
    }
}