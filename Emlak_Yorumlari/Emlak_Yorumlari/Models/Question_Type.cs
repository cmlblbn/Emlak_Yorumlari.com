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
    [Table("Question_Type")]
    public class Question_Type
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int question_type_id { get; set; }


        public string question_type { get; set; }

        public bool IsActive { get; set; }

        public virtual List<Question_Definition> question_definitions { get; set; }
    }
}
