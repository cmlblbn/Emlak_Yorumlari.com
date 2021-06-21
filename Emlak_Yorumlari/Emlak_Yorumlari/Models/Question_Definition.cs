using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emlak_Yorumlari_Entities.Models
{
    [Table("Question_Definition")]
    public class Question_Definition
    {
        [MaxLength(10, ErrorMessage = "Maximum {0} karakter olmalı!")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsUnique = true)]
        public int question_id { get; set; }

        [StringLength(50, ErrorMessage = "question_name alanı max. {0} karakter olmalıdır.")]
        public string question_name { get; set; }


        public bool IsActive { get; set; }


        public virtual List<Survey> surveys { get; set; }
    }
}
