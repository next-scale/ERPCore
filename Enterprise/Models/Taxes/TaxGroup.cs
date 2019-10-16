using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Models.Taxes
{
    [Table("ERP_Taxes_TaxGroups")]
    public class TaxGroup
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public bool isDefault { get; set; }
        public String Code { get; private set; }


        public TaxGroup()
        {
            this.Id = Guid.NewGuid();
        }

        public void Update(TaxGroup taxGroup)
        {
            this.Name = taxGroup.Name;
            this.Code = taxGroup.Code;
            this.Description = taxGroup.Description;
            this.isDefault = taxGroup.isDefault;


        }

    }
}
