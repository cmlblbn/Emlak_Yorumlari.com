using Emlak_Yorumlari_Entities;
using Emlak_Yorumlari_Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emlak_Yorumlari.Models
{
    [Table("Survey_Log")]
    public class Survey_Log
    {
        [Key]
        [Column(Order = 1)]
        public int user_id { get; set; }
        public virtual User user { get; set; }


        [Key]
        [Column(Order = 2)]
        public int question_id { get; set; }
        public virtual Question_Definition question_definitioın { get; set; }


        [Key]
        [Column(Order = 3)]
        public int place_id { get; set; }
        public virtual Place place { get; set; }


        public float score { get; set; }

        public int toxic_type { get; set; }

        public DateTime createdOn { get; set; }


        public bool IsActive { get; set; }
    }
}
