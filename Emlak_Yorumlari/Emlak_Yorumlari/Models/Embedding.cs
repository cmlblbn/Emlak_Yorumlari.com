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
    public class Embedding
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsUnique = true)]
        public int embedding_id { get; set; }

        [StringLength(10000, ErrorMessage = "text alanı max. {0} karakter olmalıdır.")]
        public string text { get; set; }

        public int prediction_sentiment { get; set; }

        public int actual_sentiment { get; set; }

        public bool isTrained { get; set; }

        public DateTime createdOn { get; set; }

        public bool isActive { get; set; }

    }
}
