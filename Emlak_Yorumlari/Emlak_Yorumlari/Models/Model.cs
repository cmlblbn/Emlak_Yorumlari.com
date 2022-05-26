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
    public class Model
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsUnique = true)]
        public int model_id { get; set; }

        public string modelName { get; set; }

        public string type { get; set; }

        public float Accuracy { get; set; }

        public float loss { get; set; }

        public int batch_size { get; set; }

        public int epoch { get; set; }

        public int maxlen { get; set; }

        public DateTime createdOn { get; set; }

        public bool isActive { get; set; }

    }
}
