using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Emlak_Yorumlari.Models;

namespace Emlak_Yorumlari_WebApp.ViewModels
{
    public class MLopsModelViewModel
    {
        public List<Model> models { get; set; }
        public int activeModel_id { get; set; }

        public int epoch { get; set; }
        public int maxlen { get; set; }
        public int batch_size { get; set; }
        public string model_type { get; set; }
        
        public bool? currTrainStatus { get; set; }
    }
}