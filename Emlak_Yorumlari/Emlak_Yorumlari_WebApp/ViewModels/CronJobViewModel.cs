using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emlak_Yorumlari_WebApp.ViewModels
{
    public class CronJobViewModel
    {
        public int maxlen { get; set; }
        public string type { get; set; }
        public int batch_size { get; set; }
        public int epoch { get; set; }
    }
}