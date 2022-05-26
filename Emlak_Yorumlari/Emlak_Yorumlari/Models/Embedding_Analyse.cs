using Emlak_Yorumlari_Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emlak_Yorumlari.Models
{
    public class Embedding_Analyse
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsUnique = true)]
        public int analyse_id { get; set; }

        public float kl_divergenceValue { get; set; }

        public DateTime lastAnalyseDate { get; set; }

        public bool isActive { get; set; }

        [MaxLength(1000)]
        public string trained_path { get; set; }

        [MaxLength(1000)]
        public string nontrained_path { get; set; }
    }
}
