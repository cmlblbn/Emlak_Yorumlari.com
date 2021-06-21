using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emlak_Yorumlari_Entities.Models
{
    [Table("Adress_Description")]
    public class Adress_Description
    {
        
        [Key]
        [Index(IsUnique = true)]
        public int adress_desc_id { get; set; }

        [StringLength(50, ErrorMessage = "adress_name alanı max. {0} karakter olmalıdır.")]
        public string adress_name { get; set; }

        public int adress_type_id { get; set; }
        [ForeignKey("adress_type_id")]
        public virtual Adress_Type adress_type { get; set; }


        public int parent_id { get; set; }

        public bool IsActive { get; set; }


        
        public virtual List<Place> places { get; set; }



    }
}
