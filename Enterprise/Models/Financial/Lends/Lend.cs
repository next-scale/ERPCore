using ERPCore.Enterprise.Models.ChartOfAccount;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ERPCore.Enterprise.Models.Transactions;
using ERPCore.Enterprise.Models.Accounting.Enums;

namespace ERPCore.Enterprise.Models.Financial.Lends
{
    [Table("ERP_Finance_Lends")]
    public class Lend
    {
        [Key]
        public Guid Id { get; set; }
        public const Models.Accounting.Enums.TransactionTypes TransactionType = Accounting.Enums.TransactionTypes.Lend;


        public DateTime TransactionDate { get; set; }
        public Guid? FiscalYearId { get; set; }
        [ForeignKey("FiscalYearId")]
        public Accounting.FiscalYear FiscalYear { get; set; }
        public int No { get; set; }
        public string Name =>
            string.Format("{0}/{1}", this.TransactionDate.ToString("yyMM"), this.No.ToString().PadLeft(3, '0'));
        public String Detail { get; set; }
        public Guid? AssetAccountGuid { get; set; }
        [ForeignKey("AssetAccountGuid")]
        public virtual Account AssetAccount { get; set; }


        public Guid? LendingAssetAccountGuid { get; set; }
        [ForeignKey("LendingAssetAccountGuid")]
        public virtual ChartOfAccount.Account LendingAssetAccount { get; set; }
        public virtual ICollection<LendPayment> Payments { get; set; }


        public LedgerPostStatus PostStatus { get; set; }


        
        public decimal Amount { get; set; }
        public CommercialStatus Status { get; set; }
    }

}
