
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
    [Table("ERP_Ledger_Transactions")]
    public class LedgerGroup
    {
        [Key]
        public Guid Id { get; set; }
        public Enums.TransactionTypes TransactionType { get; set; }
        [Index]
        public Guid? TransactionId { get; set; }

        [Column(TypeName = "Date")]
        public DateTime TransactionDate { get; set; }
        public Guid? FiscalYearId { get; set; }
        public string TransactionName { get; set; }
        public int TransactionNo { get; set; }
        public String Name => TransactionName;

        public String Reference { get; set; }
        public String ReportDisplayName => string.Format("{0} {1} · {2}{4}{3}", this.TransactionType.ToString(), this.TransactionName, this.Reference ,this.ProfileName,Environment.NewLine);

        public string ProfileName { get; set; }
        public Guid? ProfileGuid { get; set; }
        public Decimal? BaseAmount { get; set; }

        [MaxLength(500)]
        public String Memo { get; set; }

        [Column(TypeName = "Money")]
        public Decimal TotalDebit { get; set; }

        [Column(TypeName = "Money")]
        public Decimal TotalCredit { get; set; }

        public virtual ICollection<LedgerLine> LedgerLines { get; set; }
        public virtual ICollection<LedgerLine> DrLedgers() => this.LedgerLines.Where(l => l.Debit != null).ToList();
        public virtual ICollection<LedgerLine> CrLedgers() => this.LedgerLines.Where(l => l.Credit != null).ToList();

        public LedgerGroup()
        {
            this.Id = Guid.NewGuid();
        }

        public void AddDebit(Account AccountItem, decimal amount, string memo = "")
        {
            if (amount == 0 || AccountItem == null)
                return;

            Models.Accounting.LedgerLine ledger = new Models.Accounting.LedgerLine()
            {
                TransactionId = this.Id,
                TransactionDate = this.TransactionDate,
                TransactionName = this.TransactionName,
                TransactionType = this.TransactionType,
                Debit = amount,
                Credit = null,
                AccountId = AccountItem.Id,
                AccountName = AccountItem.Name,
                accountItem = AccountItem,
                Memo = memo,
                ledgerPostType = LedgerPostType.Debit,

            };

            if (this.LedgerLines == null)
                this.LedgerLines = new HashSet<LedgerLine>();

            this.LedgerLines.Add(ledger);
        }
        public void AddCredit(Account AccountItem, decimal amount, string memo = "")
        {
            if (amount == 0 || AccountItem == null)
                return;

            Models.Accounting.LedgerLine ledger = new Models.Accounting.LedgerLine()
            {
                TransactionId = this.Id,
                TransactionDate = this.TransactionDate,
                TransactionName = this.TransactionName,
                TransactionType = this.TransactionType,
                Debit = null,
                Credit = amount,
                AccountId = AccountItem.Id,
                AccountName = AccountItem.Name,
                accountItem = AccountItem,
                Memo = memo,
                ledgerPostType = LedgerPostType.Credit,
            };

            if (this.LedgerLines == null)
                this.LedgerLines = new HashSet<LedgerLine>();

            this.LedgerLines.Add(ledger);
        }
        public void RemoveAllLedgerLines()
        {
            if (this.LedgerLines == null)
                return;

            var clearLedgers = this.LedgerLines.ToList();
            clearLedgers.ForEach(ledger =>
            {
                this.LedgerLines.Remove(ledger);
            });

            TotalDebit = 0;
            TotalCredit = 0;
        }
        public bool FinalValidate(bool AcknowledgeRequried = false)
        {
            if (this.LedgerLines == null)
            {
                Console.WriteLine("ERROR fail posting, Empty tr, {0}-{1}", this.TransactionType, this.Name);
                return false;
            }


            TotalDebit = this.LedgerLines.Sum(l => l.Debit) ?? 0;
            TotalCredit = this.LedgerLines.Sum(l => l.Credit) ?? 0;

            if (TotalDebit == TotalCredit && TotalCredit != 0)
                return true;
            else
            {
                if (TotalDebit != TotalCredit)
                    Console.WriteLine("ERROR fail posting, debit!=credit, {0}-{1}", this.TransactionType, this.Name);
                else if (TotalCredit == 0)
                    Console.WriteLine("ERROR fail posting, amount=0, {0}-{1}", this.TransactionType, this.Name);
                else
                    Console.WriteLine("ERROR fail posting, other, {0}-{1}", this.TransactionType, this.Name);


                this.LedgerLines.ToList().ForEach(l =>
                {
                    Console.WriteLine("{0}\t\tDr.{1}\t\tCr.{2}", l.accountItem.Code, (l.Debit ?? 0).ToString("N2"), (l.Credit ?? 0).ToString("N2"));
                });

                this.RemoveAllLedgerLines();
                return false;
            }
        }

    }
}