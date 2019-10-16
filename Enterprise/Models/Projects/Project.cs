using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Models.Projects
{
    [Table("ERP_Projects")]
    public class Project
    {
        [Key]
        // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid? ParentGuid { get; set; }
        [ForeignKey("ParentGuid")]
        public virtual Project Parent { get; set; }

        public virtual ICollection<Project> Childs { get; set; }
        public virtual ICollection<Material> Materials { get; set; }
        public virtual ICollection<Transactions.Commercial> Commercials { get; set; }
        public virtual ICollection<Estimations.Estimate> Estimates { get; set; }

        [MaxLength(10)]
        public String Code { get; set; }
        public String Name { get; set; }
        public Enums.ProjectStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Age => (int)((DateTime.Today - CreatedDate).TotalDays);
        public String Detail { get; set; }


        public Guid? CustomerProfileGuid { get; set; }
        [ForeignKey("CustomerProfileGuid")]
        public virtual Profiles.Profile Customer { get; set; }


        public Guid? ResponsibleProfileGuid { get; set; }
        [ForeignKey("ResponsibleProfileGuid")]
        public virtual Profiles.Profile ResponsibleProfile { get; set; }



        public virtual void Close()
        {
            this.Status = Enums.ProjectStatus.Close;
        }

        public virtual void Open()
        {
            this.Status = Enums.ProjectStatus.Active;
        }


    }
}
