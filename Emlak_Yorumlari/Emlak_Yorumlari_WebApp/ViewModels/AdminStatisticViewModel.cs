using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emlak_Yorumlari_WebApp.ViewModels
{
    public class AdminStatisticViewModel
    {
        public int totalUser { get; set; }
        public int totalActiveUser { get; set; }
        public int totalPlace { get; set; }
        public int totalComment { get; set; }
        public float activeUserPlaceRatio { get; set; }
        public float activeUserCommentRatio { get; set; }
    }
}