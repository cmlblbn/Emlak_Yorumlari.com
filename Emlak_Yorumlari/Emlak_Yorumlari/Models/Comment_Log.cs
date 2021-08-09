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
    [Table("Comment_Log")]
    public class Comment_Log
    {
        [Key]
        [Column(Order = 1)]
        public int user_id { get; set; }
        public virtual User user { get; set; }


        [Key]
        [Column(Order = 2)]
        public int place_id { get; set; }
        public virtual Place place { get; set; }

        [StringLength(10000, ErrorMessage = "text alanı max. {0} karakter olmalıdır.")]
        public string text { get; set; }

        public int toxic_type { get; set; }

        public DateTime createdOn { get; set; }


        public bool IsActive { get; set; }
    }
}
