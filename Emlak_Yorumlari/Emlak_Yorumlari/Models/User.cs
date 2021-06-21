using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Emlak_Yorumlari_Entities.Models;


namespace Emlak_Yorumlari_Entities
{
    [Table("User")]
    public class User
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsUnique = true)]
        public int user_id { get; set; }

        [StringLength(25, ErrorMessage = "username alanı max. {0} karakter olmalıdır."),Required]
        [Index(IsUnique = true)]
        public string username { get; set; }

        [StringLength(60, ErrorMessage = "email alanı max. {0} karakter olmalıdır."), Required]
        [Index(IsUnique = true)]
        public string email { get; set; }

        [StringLength(60, ErrorMessage = "password alanı max. {0} karakter olmalıdır."), Required]
        public string password { get; set; }

        
        public byte[] profileImage { get; set; }

        public DateTime createOn { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsActive { get; set; }


        public virtual List<Place> places { get; set; }
        public virtual List<Survey> surveys { get; set; }
        public virtual List<Comment> comments { get; set; }

    }
}
