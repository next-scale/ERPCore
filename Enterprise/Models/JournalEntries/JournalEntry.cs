
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ERPCore.Enterprise.Models.Accounting;
using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.Transactions;

namespace ERPCore.Enterprise.Models.AccountingEntries
{

    [Table("ERP_JournalEntry")]
    public class JournalEntry
    {
        [Key]
        [Column("GID")]
        public Guid Id { get; set; }
        public int No { get; set; }
        public string Name => string.Format("{0}/{1}", this.TransactionDate.ToString("yyMM"), this.No.ToString().PadLeft(3, '0'));

        public const Accounting.Enums.TransactionTypes TransactionType = Accounting.Enums.TransactionTypes.JournalEntry;
        public DateTime TransactionDate { get; set; }
        public Guid? JournalEntryTypeGuid { get; set; }
        [ForeignKey("JournalEntryTypeGuid")]
        public virtual JournalEntryType JournalEntryType { get; set; }
        public String Memo { get; set; }

        public Decimal Debit { get; set; }
        public Decimal Credit { get; set; }



        public virtual ICollection<ERPCore.Enterprise.Models.AccountingEntries.JournalEntryLine> Items { get; set; }

        public LedgerPostStatus PostStatus { get; set; }



        public DateTime? JournalPostDate { get; set; }
        public CommercialStatus Status { get; set; }

        public Guid? FiscalYearId { get; set; }
        [ForeignKey("FiscalYearId")]
        public FiscalYear FiscalYear { get; set; }

        public JournalEntry()
        {
            this.Id = Guid.NewGuid();
        }


        public virtual void UpdateAmount()
        {
            Debit = (Items?.Select(i => i.Debit).DefaultIfEmpty(0).Sum()) ?? 0;
            Credit = (Items?.Select(i => i.Credit).DefaultIfEmpty(0).Sum()) ?? 0;
        }

        public void AddAcount(Guid accountId)
        {
            var jourmalEntryItem = new JournalEntryLine()
            {
                Id = Guid.NewGuid(),
                AccountId = accountId
            };

            this.Items.Add(jourmalEntryItem);
            this.UpdateAmount();
        }
    }
}