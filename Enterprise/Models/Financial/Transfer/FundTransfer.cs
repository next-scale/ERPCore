using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Transactions;

namespace ERPCore.Enterprise.Models.Financial.Transfer
{

    [Table("ERP_Finance_Transfer")]
    public class FundTransfer
    {
        [Key]
        public Guid Id { get; set; }

        public Models.Accounting.Enums.TransactionTypes TransactionType = Accounting.Enums.TransactionTypes.FundTransfer;


        public string Reference { get; set; }
        public DateTime TransactionDate { get; set; }
        public FundTransferStatus Status { get; set; }

        public Guid? FiscalYearId { get; set; }
        [ForeignKey("FiscalYearId")]
        public Accounting.FiscalYear FiscalYear { get; set; }



        public string Name =>
                string.Format("{0}/{1}/{2}", DocumentCode, DocumentGroup, No.ToString().PadLeft(3, '0'));
        public string DocumentGroup => this.TransactionDate.ToString("yyMM");
        public string DocumentCode => TransactionHelper.TrCode(this.TransactionType);

        public int No { get; set; }



        public Decimal AmountwithDraw { get; set; }
        public Decimal AmountFee { get; set; }
        public Decimal AmountDeposit
        {
            get
            {
                return this.AmountwithDraw - this.AmountFee;
            }
        }



        /// <summary>
        /// Chart Of Account Detail
        /// </summary>
        public Guid? WithDrawAccountGuid { get; set; }
        [ForeignKey("WithDrawAccountGuid")]
        public virtual Account WithDrawAccount { get; set; }
        public Guid? DepositAccountGuid { get; set; }
        [ForeignKey("DepositAccountGuid")]
        public virtual Account DepositAccount { get; set; }
        public Guid? BankFeeAccountGuid { get; set; }
        [ForeignKey("BankFeeAccountGuid")]
        public virtual Account BankFeeAccount { get; set; }




        public String Memo { get; set; }
        public LedgerPostStatus PostStatus { get; set; }

    }
}