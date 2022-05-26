using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Emlak_Yorumlari.Models;

namespace Emlak_Yorumlari_WebApp.ViewModels
{
    public class MLopsLabelViewModel
    {
        public List<Comment_Log> data { get; set; }
        public string Label { get; set; }

    }
}