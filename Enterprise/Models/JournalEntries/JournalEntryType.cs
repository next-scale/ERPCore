
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Models.AccountingEntries
{
    using ERPCore.Enterprise.Models.Accounting;

    [Table("ERP_JournalEntry_Tpye")]
    public class JournalEntryType
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Detail { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }

        public virtual ICollection<JournalEntry> JournalEntries { get; set; }

        public virtual int EntryCount => this.JournalEntries?.Count() ?? 0;

        public void Update(JournalEntryType journalEntryType)
        {
            this.Name = journalEntryType.Name;
            this.Detail = journalEntryType.Detail;
        }
    }
}