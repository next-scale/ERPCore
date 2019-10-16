
using ERPCore.Enterprise.Models.ChartOfAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Repository.Company;
using ERPCore.Enterprise.Models.AccountingEntries;
using ERPCore.Enterprise.Models.Transactions;
using Microsoft.EntityFrameworkCore;
using ERPCore.Enterprise.Models.Accounting.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERPCore.Enterprise.Repository.Accounting
{

    public class JournalEntries : ERPNodeDalRepository
    {
        public JournalEntries(Organization organization) : base(organization)
        {
            this.transactionType = TransactionTypes.JournalEntry;
        }

        public List<JournalEntry> ListAll => erpNodeDBContext.JournalEntries.ToList();
        public IQueryable<JournalEntry> All => erpNodeDBContext.JournalEntries;
        public JournalEntry Find(Guid id) => erpNodeDBContext.JournalEntries.Find(id);
        public int NextNumber => (erpNodeDBContext.JournalEntries.Max(b => (int?)b.No) ?? 0) + 1;

        public void ReOrder()
        {
            var journalEntries = erpNodeDBContext.JournalEntries
                .OrderBy(t => t.TransactionDate)
                .ToList();

            int i = 1;
            foreach (var j in journalEntries)
            {
                j.No = i;
                i++;

            }

            erpNodeDBContext.SaveChanges();
        }

        public void Delete(JournalEntry jourmalEntry)
        {
            if (jourmalEntry.PostStatus == LedgerPostStatus.Posted)
                this.UnPostLedger(jourmalEntry);

            erpNodeDBContext.JournalEntries.Remove(jourmalEntry);
            erpNodeDBContext.SaveChanges();
        }

        public void Save(JournalEntry journalEntry)
        {
            var existJourmalEntry = this.Find(journalEntry.Id);

            existJourmalEntry.FiscalYear = organization.FiscalYears.Find(journalEntry.TransactionDate);
            existJourmalEntry.TransactionDate = journalEntry.TransactionDate;
            existJourmalEntry.Memo = journalEntry.Memo;
            existJourmalEntry.UpdateAmount();

            erpNodeDBContext.SaveChanges();
        }

        public void UnPostAllLedger()
        {
            Console.WriteLine("> Un Post" + this.transactionType.ToString());

            string sqlCommand = "DELETE FROM [dbo].[ERP_Ledgers] WHERE TransactionType = {0} ";
            sqlCommand = string.Format(sqlCommand, (int)this.transactionType);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);

            sqlCommand = "DELETE FROM [dbo].[ERP_Ledger_Transactions] WHERE TransactionType =  {0}";
            sqlCommand = string.Format(sqlCommand, (int)this.transactionType);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);

            erpNodeDBContext.JournalEntries.ToList().ForEach(s => s.PostStatus = LedgerPostStatus.ReadyToPost);
            erpNodeDBContext.SaveChanges();

        }

        public JournalEntry CreateNew(Guid entryTypeGuid)
        {
            var jourmalEntry = new JournalEntry();

            jourmalEntry.Id = Guid.NewGuid();
            jourmalEntry.JournalEntryTypeGuid = entryTypeGuid;
            jourmalEntry.TransactionDate = DateTime.Today;
            jourmalEntry.No = this.NextNumber;
            erpNodeDBContext.JournalEntries.Add(jourmalEntry);
            erpNodeDBContext.SaveChanges();

            return jourmalEntry;
        }

        public void UnPostLedger(JournalEntry journalEntry)
        {
            organization.LedgersDal.RemoveTransaction(journalEntry.Id);
            journalEntry.PostStatus = LedgerPostStatus.ReadyToPost;
            erpNodeDBContext.SaveChanges();
        }

        public List<JournalEntry> ReadyForPost => erpNodeDBContext.JournalEntries
                    .Where(s => s.PostStatus == LedgerPostStatus.ReadyToPost)
                    .Include(s => s.Items).ToList();


        public void PostLedger()
        {
            var unPostTransactions = this.ReadyForPost;
            Console.WriteLine("> {0} Post {2} [{1}]", DateTime.Now.ToLongTimeString(), unPostTransactions.Count(), this.transactionType.ToString());

            erpNodeDBContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            unPostTransactions.ForEach(s =>
            {
                this.PostLedger(s, false);
            });

            erpNodeDBContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            erpNodeDBContext.ChangeTracker.DetectChanges();
            erpNodeDBContext.SaveChanges();
        }
        public bool PostLedger(JournalEntry tr, bool SaveImmediately = true)
        {
            if (tr.PostStatus == LedgerPostStatus.Posted)
                return false;

            var trLedger = new Models.Accounting.LedgerGroup()
            {
                Id = tr.Id,
                TransactionDate = tr.TransactionDate,
                TransactionName = tr.Name,
                TransactionNo = tr.No,
                TransactionType = transactionType
            };

            foreach (var journalentryItem in tr.Items.ToList())
            {
                if (journalentryItem.Debit != null && journalentryItem.Debit > 0)
                    trLedger.AddDebit(journalentryItem.Account, journalentryItem.Debit ?? 0);
                else
                    trLedger.AddCredit(journalentryItem.Account, journalentryItem.Credit ?? 0);
            }


            if (trLedger.FinalValidate())
            {
                erpNodeDBContext.LedgerGroups.Add(trLedger);
                tr.PostStatus = LedgerPostStatus.Posted;
            }
            else
            {
                return false;
            }


            if (SaveImmediately)
                erpNodeDBContext.SaveChanges();

            return true;
        }
    }
}