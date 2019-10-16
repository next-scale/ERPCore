
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Models.AccountingEntries
{

    [Table("ERP_Journal_JournalEntryItem")]
    public class JournalEntryLine
    {
        [Key]
        public Guid Id { get; set; }


        public Guid JournalEntryId { get; set; }
        [ForeignKey("JournalEntryId")]
        public virtual Models.AccountingEntries.JournalEntry JournalEntry { get; set; }


        [Column("AccountId")]
        public Guid AccountId { get; set; }
        [ForeignKey("AccountId")]
        public virtual ChartOfAccount.Account Account { get; set; }
        public Decimal? Debit { get; set; }
        public Decimal? Credit { get; set; }

        public String Memo { get; set; }

        public JournalEntryLine()
        {
            this.Id = Guid.NewGuid();
        }

        public void Update(JournalEntryLine item)
        {
            this.AccountId = item.AccountId;
            this.Debit = item.Debit;
            this.Credit = item.Credit;
            this.Memo = item.Memo;
            this.JournalEntry.UpdateAmount();
        }
    }
}