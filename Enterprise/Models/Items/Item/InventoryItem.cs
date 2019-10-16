using ERPCore.Enterprise.Models.ChartOfAccount;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ERPCore.Enterprise.Models.Items
{
    /// <summary>
    /// Inventory Part
    /// </summary>



    public partial class Item
    {

        [DefaultValue(Enums.CostingMethods.None)]
        public Enums.CostingMethods CostingMethod { get; set; }

        [DefaultValue(Enums.CostPosingMethods.None)]
        public Enums.CostPosingMethods CostPosingMethod { get; set; }



        
        public int? AmountOnQuotation { get; set; }

        
        public int? AmountOnSaleOerder { get; set; }

        
        public int? AmountOnPurchaseOrder { get; set; }

        
        public int? AmountReorder { get; set; }

      

        public decimal LastestPurchaseCost { get; set; }
        
        public int AmountSold { get; set; }
        
        public int AmountPurchase { get; set; }

        [NotMapped]
        public int AmountOnHand => AmountPurchase - AmountSold;



        public int ReorderRequiredAmount
        {
            get
            {
                if (this.AmountOnHand < (this.AmountReorder ?? 0))
                    return (this.AmountReorder ?? 0) - this.AmountOnHand;
                else
                    return 0;
            }
        }


        
        public int OpeningQty { get; set; }
        public Decimal OpeningCostPerUnit { get; set; }



        [DisplayName("COGS Account")]
        [Column("COGSAccountGuid")]
        public Guid? COGSAccountGuid { get; set; }
        [ForeignKey("COGSAccountGuid")]
        public virtual Account COGSAccount { get; set; }


    }
}

