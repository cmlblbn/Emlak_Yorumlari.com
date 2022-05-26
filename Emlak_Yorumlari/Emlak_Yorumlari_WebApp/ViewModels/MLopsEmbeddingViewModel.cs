using Emlak_Yorumlari.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emlak_Yorumlari_WebApp.ViewModels
{
    public class MLopsEmbeddingViewModel
    {
        public List<Embedding_Analyse> data { get; set; }
        public DateTime lastAnalyseDate { get; set; }
        public string trained_path { get; set; }
        public string nontrained_path { get; set; }
    }
}