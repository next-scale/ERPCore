using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ERPCore.Enterprise.Models.Profiles;

namespace ERPCore.Enterprise.Models.Security
{
    [Table("ERP_Security_Permissions")]

    public class Member
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }
        public String Name { get; set; }
        public String ShotName { get; set; }
        public Enums.AccessLevel AccessLevel { get; set; }


        [Timestamp]
        public byte[] RowVersion { get; set; }
        public String Email { get; set; }



        public Guid? ProfileId { get; set; }
        [ForeignKey("ProfileId")]
        public virtual Profile Profile { get; set; }

        public void Update(Member member)
        {
            this.Name = member.Name;
            this.AccessLevel = member.AccessLevel;
            this.ShotName = member.ShotName;
        }

        public DateTime? LastDateSync { get; set; }
        public DateTime? LastDateUsage { get; set; }

        public Member()
        {

        }
    }
}
