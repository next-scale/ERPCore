using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Models.Departments
{

    [Table("ERP_Rules")]
    public class Rule
    {
        [Key]
        public Guid Id { get; set; }
        public String Title { get; set; }

        public Company.Enums.RuleType Type
        {
            get; set;
        }

        public Guid? ParentGuid { get; set; }
        [ForeignKey("ParentGuid")]
        public virtual Rule Parent { get; set; }
    }
}
