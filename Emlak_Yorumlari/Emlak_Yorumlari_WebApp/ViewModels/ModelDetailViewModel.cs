using Emlak_Yorumlari.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emlak_Yorumlari_WebApp.ViewModels
{
    public class ModelDetailViewModel
    {
        public Model model { get; set; }
        public string predictText { get; set; }
        public string predict { get; set; }

    }
}