using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Emlak_Yorumlari_Entities.Models;

namespace Emlak_Yorumlari_Entities
{
    [Table("Place")]
    public class Place
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsUnique = true)]
        public int place_id { get; set; }

        public int user_id { get; set; }
        [ForeignKey("user_id")]
        public virtual User user { get; set; }

        public int adress_desc_id { get; set; }
        [ForeignKey("adress_desc_id")]
        public virtual Adress_Description adress_description { get; set; }

        [StringLength(100, ErrorMessage = "placeName alanı max. {0} karakter olmalıdır.")]
        public string placeName { get; set; }

        public byte[] placeImage { get; set; }

        public DateTime createdOn { get; set; }

        public bool IsActive { get; set; }




        public virtual List<Survey> surveys { get; set; }
        public virtual List<Comment> comments { get; set; }


    }
}
