using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emlak_Yorumlari_Entities.Models
{
    [Table("Comment")]
    public class Comment
    {
        [MaxLength(10, ErrorMessage = "Maximum {0} karakter olmalı!")]
        [Key]
        public int user_id { get; set; }
        public virtual User user { get; set; }

        [MaxLength(10, ErrorMessage = "Maximum {0} karakter olmalı!")]
        [Key]
        public int place_id { get; set; }
        public virtual Place place { get; set; }

        [StringLength(10000, ErrorMessage = "text alanı max. {0} karakter olmalıdır.")]
        public string text { get; set; }


        public DateTime createdOn { get; set; }


        public bool IsActive { get; set; }

    }
}
