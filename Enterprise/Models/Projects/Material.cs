using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Models.Projects
{
    [Table("ERP_Projects_Materials")]
    public class Material
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }

        public Guid? ParentGuid { get; set; }
        [ForeignKey("ParentGuid")]
        public virtual Material Parent { get; set; }
        public virtual ICollection<Material> Childs { get; set; }

        public Guid? ItemId { get; set; }
        [ForeignKey("ItemId")]
        public virtual Items.Item Item { get; set; }

        public string Memo { get; set; }
        public int Amount { get; set; }

        public decimal UnitPrice { get; set; }


        public decimal LinePrice => Amount * UnitPrice;

        public decimal PercentageProfit { get; set; }

        public decimal UnitSellingPrice { get; set; }

        public decimal SellingLinePrice => Amount * UnitSellingPrice;

        public decimal Profit => SellingLinePrice - LinePrice;

        public DateTime RecordDate { get; set; }
    }
}
