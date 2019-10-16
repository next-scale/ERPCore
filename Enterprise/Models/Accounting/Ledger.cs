
using ERPCore.Enterprise.Models.ChartOfAccount;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Models.Accounting
{
    public enum LedgerPostType
    {
        Debit = 0,
        Credit = 1
    }


    [Table("ERP_Ledgers")]
    public class LedgerLine
    {
        [Key]
        public Guid Id { get; set; }

        public Enums.TransactionTypes TransactionType { get; set; }

        public Guid LedgerGroupId { get; set; }
        [ForeignKey("LedgerGroupId")]
        public virtual LedgerGroup LedgerGroup { get; set; }

        public LedgerPostType ledgerPostType { get; set; }


        public int Index { get; set; }

        [Index]
        public Guid? TransactionId { get; set; }

        [Column(TypeName = "Date")]
        public DateTime TransactionDate { get; set; }
        public int TransactionNo { get; set; }
        public string TransactionName { get; set; }
        public String Name => TransactionName;




        public string ProfileName { get; set; }
        public Guid? ProfileGuid { get; set; }

        public Guid? FiscalYearId { get; set; }


        public int OrderNo { get; set; }

        [Column("AccountName")]
        public String AccountName { get; set; }
        public Guid AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual Account accountItem { get; set; }

        public String ReportDisplayAccountName
        {
            get
            {
                if (this.ledgerPostType == LedgerPostType.Credit)
                    return "    " + accountItem.Name;
                else
                    return accountItem.Name;
            }
        }

        [MaxLength(500)]
        public String Memo { get; set; }

        [Column(TypeName = "Money")]
        
        public Decimal? Debit { get; set; }

        [Column(TypeName = "Money")]
        
        public Decimal? Credit { get; set; }


        [Column(TypeName = "Money")]
        [DisplayFormat(DataFormatString = "N2")]
        public virtual Decimal Balance
        {
            get
            {
                switch (this.accountItem.Type)
                {
                    case AccountTypes.Asset:
                    case AccountTypes.Expense:
                        return (Debit ?? 0) - (Credit ?? 0);

                    case AccountTypes.Liability:
                    case AccountTypes.Capital:
                    case AccountTypes.Income:
                        return (Credit ?? 0) - (Debit ?? 0);
                    default:
                        return 0;
                }
            }
        }

        [DisplayFormat(DataFormatString = "N2")]
        
        public virtual Decimal TotalBalance
        {
            get; set;
        }

        public LedgerLine()
        {
            this.Id = Guid.NewGuid();
        }
    }
}