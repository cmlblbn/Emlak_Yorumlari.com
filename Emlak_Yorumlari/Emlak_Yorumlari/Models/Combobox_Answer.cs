using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Emlak_Yorumlari_Entities.Models;

namespace Emlak_Yorumlari.Models
{
    [Table("Combobox_Answer")]
    public class Combobox_Answer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int question_answer_id { get; set; }

        public int question_id { get; set; }
        [ForeignKey("question_id")]
        public virtual Question_Definition question_Definition { get; set; }

        public string question_answer { get; set; }

        public bool IsActive { get; set; }
    }
}
