using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Taxes;
using ERPCore.Enterprise.Models.Transactions.Commercials;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;


namespace ERPCore.Enterprise.Models.Estimations
{
    [Table("ERP_Estimations_Items")]
    public partial class EstimateItem
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? EstimateGuid { get; set; }
        [ForeignKey("EstimateGuid")]
        public virtual Estimate Estimate { get; set; }


        [Index]
        public Guid? ItemGuid { get; set; }
        [ForeignKey("ItemGuid")]

        public virtual ERPCore.Enterprise.Models.Items.Item Item { get; set; }

        public String ItemPartNumber { get; set; }
        [MaxLength(2048)]
        public String ItemDescription { get; set; }

        public int Order { get; set; }
        public String Memo { get; set; }

        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }



        public Decimal DiscountPercent { get; set; }
        public Decimal UnitPriceAfterDiscount => this.UnitPrice * (((decimal)100 - DiscountPercent) / (decimal)100);
        public decimal LineTotal => this.UnitPriceAfterDiscount * this.Amount;

        public void Update(EstimateItem estimateItem)
        {
            this.Amount = estimateItem.Amount;
            this.ItemDescription = estimateItem.ItemDescription;
            this.ItemPartNumber = estimateItem.ItemPartNumber;
            this.UnitPrice = estimateItem.UnitPrice;
            this.DiscountPercent = estimateItem.DiscountPercent;
            this.Order = estimateItem.Order;
            this.Memo = estimateItem.Memo;
        }
    }
}





