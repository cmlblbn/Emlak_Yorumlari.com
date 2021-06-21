using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emlak_Yorumlari_Entities.Models
{
    [Table("Adress_Type")]
    public class Adress_Type
    {
        
        [Key]
        public int adress_type_id { get; set; }

        [StringLength(10, ErrorMessage = "adres tip alanı max. {0} karakter olmalıdır.")]
        public string adress_type { get; set; }


        public bool IsActive { get; set; }

        public virtual List<Adress_Description> adress_description{ get; set; }
    }
}
