using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Models.Taxes
{


    [Table("ERP_Taxes_TaxRate")]
    public class TaxRate
    {
        [Key]
        [Column("Id")]
        public Guid TaxRateGuid { get; set; }

        
        public Decimal Rate { get; set; }


        
        public DateTime AsOf { get; set; }

        public Guid? TaxCodeGuid { get; set; }
        [ForeignKey("TaxCodeGuid")]
        public virtual TaxCode TaxCode { get; set; }

        public TaxRate()
        {
            this.TaxRateGuid = Guid.NewGuid();
        }


    }
}