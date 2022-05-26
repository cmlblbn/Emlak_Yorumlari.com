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
    public class Crobjob_Parameter
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsUnique = true)]
        public int cronjob_id { get; set; }

        public string type { get; set; }

        public int batch_size { get; set; }

        public int epoch { get; set; }

        public int maxlen { get; set; }

        public DateTime createedOn { get; set; }

        public bool isActive { get; set; }
    }
}
