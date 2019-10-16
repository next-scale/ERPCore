using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Models.Profiles
{


    [Table("ERP_Profiles_Groups")]
    public class ProfileGroup
    {
        [Key]
        public Guid ProfileGroupGuid { get; set; }
        public EnumProfileGroupType GroupType { get; set; }

        [MaxLength(255)]
        public String Title { get; set; }
        public String Detail { get; set; }
        public virtual ICollection<Profile> Members { get; set; }

        public int MemberCount
        {
            get
            {
                //if (Profiles != null)
                //    return Profiles.Count;
                //else
                return 0;
            }
        }


        public ProfileGroup()
        {
            ProfileGroupGuid = Guid.NewGuid();
        }
    }
}