using ERPCore.Enterprise.Models.ChartOfAccount;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ERPCore.Enterprise.Models.Accounting;
using ERPCore.Enterprise.Models.Accounting.Enums;

namespace ERPCore.Enterprise.Models.Financial.Loans
{
    [Table("ERP_Finance_Loans")]
    public class Loan
    {
        [Key]
        public Guid Id { get; set; }
        public Models.Accounting.Enums.TransactionTypes TransactionType = Accounting.Enums.TransactionTypes.Loan;
        public Models.Transactions.CommercialStatus Status { get; set; }

        public DateTime TransactionDate { get; set; }
        public Guid? FiscalYearId { get; set; }
        [ForeignKey("FiscalYearId")]
        public Accounting.FiscalYear FiscalYear { get; set; }

        public int No { get; set; }

        public string Name =>
            string.Format("{0}/{1}", this.TransactionDate.ToString("yyMM"), this.No.ToString().PadLeft(3, '0'));

        public String Detail { get; set; }

        public decimal Amount { get; set; }

        public Guid? AssetAccountGuid { get; set; }
        [ForeignKey("AssetAccountGuid")]
        public virtual Account AssetAccount { get; set; }



        public Guid? LiabilityAccountGuid { get; set; }
        [ForeignKey("LiabilityAccountGuid")]
        public virtual ChartOfAccount.Account LiabilityAccount { get; set; }


        public virtual ICollection<LoanPayment> Payments { get; set; }


        public LedgerPostStatus PostStatus { get; set; }

    }

}
