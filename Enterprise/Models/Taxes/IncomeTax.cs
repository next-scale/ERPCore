using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.ChartOfAccount;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Models.Taxes
{
    [Table("ERP_Taxes_IncomeTaxes")]
    public class IncomeTax
    {
        [Key]
        public Guid Id { get; set; }
        public int No { get; set; }

        public DateTime TrDate { get; set; }
        public string Name => string.Format("INT{0}-{1}", this.TrDate.ToString("yyMM"), this.No.ToString().PadLeft(3, '0'));

        public Decimal ProfitAmount { get; set; }
        public Decimal TaxAmount { get; set; }
        public String Memo { get; set; }

        public bool Update(IncomeTax incomeTax)
        {
            if (this.PostStatus != LedgerPostStatus.Posted)
            {
                this.TrDate = incomeTax.TrDate;
                this.ProfitAmount = incomeTax.ProfitAmount;
                this.TaxAmount = incomeTax.TaxAmount;
                this.IncomeTaxExpenseAccountGuid = incomeTax.IncomeTaxExpenseAccountGuid;
                this.LiabilityAccountGuid = incomeTax.LiabilityAccountGuid;
                return true;
            }

            return false;
        }

        public Guid? FiscalYearId { get; set; }
        [ForeignKey("FiscalYearId")]
        public virtual Accounting.FiscalYear FiscalYear { get; set; }

        public Guid? LiabilityAccountGuid { get; set; }
        [ForeignKey("LiabilityAccountGuid")]
        public virtual Account LiabilityAccount { get; set; }


        public Guid? IncomeTaxExpenseAccountGuid { get; set; }
        [ForeignKey("IncomeTaxExpenseAccountGuid")]
        public virtual Account IncomeTaxExpenAccount { get; set; }

        public Guid? PayFromAccountGuid { get; set; }
        public LedgerPostStatus PostStatus { get; set; }

    }
}
