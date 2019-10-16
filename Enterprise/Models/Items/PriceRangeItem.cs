using ERPCore.Enterprise.Models.ChartOfAccount;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ERPCore.Enterprise.Models.Items
{



    [Table("ERP_Items_PriceRangeItems")]
    public class PriceRangeItem
    {
        [Key]
        public Guid Id { get; set; }
        public int MinAmount { get; set; }
        public int MaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }


        public Guid PriceRangeGuid { get; set; }
        [ForeignKey("PriceRangeGuid")]
        public virtual PriceRange PriceRange { get; set; }
    }
}