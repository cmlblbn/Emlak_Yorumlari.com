using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Emlak_Yorumlari_Entities.Models;
using Emlak_Yorumlari_Entities;

namespace Emlak_Yorumlari.Models
{
    [Table("Place_Statistics")]
    public class Place_Statistics
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int statistic_id { get; set; }

        public int place_id { get; set; }
        [ForeignKey("place_id")]
        public virtual Place place { get; set; }

        public int male_count { get; set; }
        public int female_count { get; set; }
        public int otherSex_count { get; set; }
        public int primarySchool_count { get; set; }
        public int middleSchool_count { get; set; }
        public int highSchool_count { get; set; }
        public int degree_count { get; set; }
        public int masterDegree_count { get; set; }
        public int married_count { get; set; }
        public int single_count { get; set; }
        public int divorced_count { get; set; }
        public int widow_count { get; set; }
        public int average_rent { get; set; }
        public int age_lower_18 { get; set; }
        public int age_between_18_34 { get; set; }
        public int age_between_34_55 { get; set; }
        public int age_upper_55 { get; set; }

        public DateTime createdOn { get; set; }

        public bool IsActive { get; set; }


    }
}
