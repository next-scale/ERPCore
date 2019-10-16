using ERPCore.Enterprise.Models.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ERPCore.Enterprise.Models.Accounting.Enums;

namespace ERPCore.Enterprise.Models.Accounting.FiscalYears
{
    [Table("ERP_Period_Item_COGS")]
    public class PeriodItemCOGS
    {
        public readonly TransactionTypes TransactionType = TransactionTypes.ItemCOGS;

        [Key]
        public Guid Id { get; set; }

        public DateTime TransactionDate => FiscalYear.EndDate;
        public string Name => string.Format("{0}/{1}", this.TransactionDate.ToString("yyMM"), this.No.ToString().PadLeft(3, '0'));
        public int No { get; set; }

        public Guid? ItemGuid { get; set; }
        [ForeignKey("ItemGuid")]
        public virtual Item Item { get; set; }


        public Guid? FiscalYearId { get; set; }
        [ForeignKey("FiscalYearId")]
        public virtual FiscalYear FiscalYear { get; set; }

        public Guid? PreviusAverageItemCOGSId { get; set; }

        //Input Section
        public int OpeningAmount { get; set; }
        public Decimal OpeningValue { get; set; }
        public int InputAmount { get; set; }
        public Decimal InputValue { get; set; }


        //Current Period
        public int CurrentPeriodAmount => this.OpeningAmount + this.InputAmount;
        public Decimal CurrentPeriodValue => this.OpeningValue + this.InputValue;
        public decimal CurrentPeriodUnitCost
        {
            get
            {
                if (CurrentPeriodAmount == 0)
                    return this.Item.UnitPrice;
                return CurrentPeriodValue / CurrentPeriodAmount;
            }
        }


        //Output Section
        public int OutputAmount { get; set; }
        public Decimal OutputCost => this.OutputAmount * this.CurrentPeriodUnitCost;



        //Remaining Section
        public int RemainAmount => this.CurrentPeriodAmount - this.OutputAmount;
        public Decimal RemainValue => this.CurrentPeriodValue - this.OutputCost;


        public DateTime LastCalculateDate { get; set; }
        public LedgerPostStatus PostStatus { get; set; }

    }
}
