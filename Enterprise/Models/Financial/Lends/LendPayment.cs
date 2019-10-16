using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.ChartOfAccount;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace ERPCore.Enterprise.Models.Financial.Lends
{
    [Table("ERP_Finance_Lend_Payments")]
    public class LendPayment
    {
        [Key]
        public Guid Id { get; set; }
        public Models.Accounting.Enums.TransactionTypes TransactionType = Accounting.Enums.TransactionTypes.LendPayment;

        public DateTime TransactionDate { get; set; }

        public int No { get; set; }


        public String Detail { get; set; }

        public Guid? LendGuid { get; set; }
        [ForeignKey("LendGuid")]
        public virtual Lend Lend { get; set; }

        public Guid? AssetAccountGuid { get; set; }
        [ForeignKey("AssetAccountGuid")]
        public virtual Account AssetAccount { get; set; }



        public Guid? LiabilityAccountGuid { get; set; }
        [ForeignKey("LiabilityAccountGuid")]
        public virtual ChartOfAccount.Account LiabilityAccount { get; set; }


        public LedgerPostStatus PostStatus { get; set; }




    }

}
