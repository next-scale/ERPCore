using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace ERPCore.Enterprise.Models.Transactions
{

    [Table("ERP_Transactions_Commercial_Items")]
    public class CommercialItem
    {
        [Key]
        public Guid Id { get; set; }


        [Index]
        [Column("ItemId")]
        public Guid ItemGuid { get; set; }
        [ForeignKey("ItemGuid")]
        public virtual ERPCore.Enterprise.Models.Items.Item Item { get; set; }

        public Accounting.Enums.TransactionTypes TransactionType { get; set; }

        public String ItemPartNumber { get; set; }
        //[MaxLength(2048)]
        public String ItemDescription { get; set; }

        [Column("TransactionId")]
        public Guid? TransactionId { get; set; }
        [ForeignKey("TransactionId")]
        public virtual Commercial Commercial { get; set; }

        public int Order { get; set; }
        [Column("UnitPrice")]
        public Decimal UnitPrice { get; set; }

        public Decimal DiscountPercent { get; set; }
        public Decimal UnitPriceAfterDiscount => this.UnitPrice * (((decimal)100 - DiscountPercent) / (decimal)100);



        public int Amount { get; set; }
        public string Memo { get; set; }
        public string FIFOCostingMemo { get; set; }
        public string SerialNumber { get; set; }



        [DisplayFormat(DataFormatString = "N2")]
        
        public Decimal LineTotal => Math.Round(this.UnitPriceAfterDiscount * (decimal)this.Amount, 2, MidpointRounding.ToEven);






        //COSG Section 
        public int InputAmount { get; set; }
        public decimal InputValue => this.UnitPrice * InputAmount;
        public int OutputAmount { get; set; }

       

        public void Update(CommercialItem commercialItem)
        {
            if (commercialItem == null)
                return;

            this.Amount = commercialItem.Amount;
            this.ItemDescription = commercialItem.ItemDescription;
            this.ItemPartNumber = commercialItem.ItemPartNumber;
            this.UnitPrice = commercialItem.UnitPrice;
            this.DiscountPercent = commercialItem.DiscountPercent;
            this.Order = commercialItem.Order;
            this.SerialNumber = commercialItem.SerialNumber;
            this.Memo = commercialItem.Memo;

            this.UpdateInventory();
        }

        public void UpdateInventory()
        {
            if (this.Item.ItemType != Items.Enums.ItemTypes.Inventory)
                return;

            switch (this.Commercial.TransactionType)
            {
                case Models.Accounting.Enums.TransactionTypes.Sale:
                case Models.Accounting.Enums.TransactionTypes.PurchaseReturn:
                    this.InputAmount = 0;
                    this.OutputAmount = this.Amount;
                    break;

                case Models.Accounting.Enums.TransactionTypes.Purchase:
                case Models.Accounting.Enums.TransactionTypes.SalesReturn:
                default:
                    this.InputAmount = this.Amount;
                    this.OutputAmount = 0;
                    break;
            }
        }

        public CommercialItem()
        {
            Id = Guid.NewGuid();
        }
    }
}
