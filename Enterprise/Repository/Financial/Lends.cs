
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Financial.Lends;
using ERPCore.Enterprise.Models.Financial.Transfer;
using ERPCore.Enterprise.Models.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Models.Accounting.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERPCore.Enterprise.Repository.Financial
{
    public class Lends : ERPNodeDalRepository
    {
        public Lends(Organization organization) : base(organization)
        {
            transactionType = Models.Accounting.Enums.TransactionTypes.Lend;
        }

        public List<Lend> GetAll => erpNodeDBContext.Lends.ToList();
        public List<Lend> ReadyForPost => erpNodeDBContext.Lends
            .Where(s => s.PostStatus == LedgerPostStatus.ReadyToPost)
            .ToList();
        public Lend Find(Guid id) => erpNodeDBContext.Lends.Find(id);

        private int NextNumber => (erpNodeDBContext.Lends.Max(e => (int?)e.No) ?? 0) + 1;

        public void Add(Lend entity)
        {
            erpNodeDBContext.Lends.Add(entity);
            erpNodeDBContext.SaveChanges();
        }

        public void ReOrder()
        {
            var lends = erpNodeDBContext.Lends
                .OrderBy(t => t.TransactionDate)
                .ToList();

            int i = 1;
            foreach (var lend in lends)
            {
                lend.No = i;
                i++;

            }

            erpNodeDBContext.SaveChanges();

        }


        public void Issue(Lend lend)
        {
            erpNodeDBContext.SaveChanges();
        }

        public void Save(Lend lend)
        {
            var existLend = erpNodeDBContext.Lends.Find(lend.Id);

            if (existLend == null)
                erpNodeDBContext.Lends.Add(lend);
            else
            {
                if (existLend.PostStatus == LedgerPostStatus.Posted)
                    return;

                existLend.TransactionDate = lend.TransactionDate;
                existLend.AssetAccountGuid = lend.AssetAccountGuid;
                existLend.LendingAssetAccountGuid = lend.LendingAssetAccountGuid;
                existLend.Amount = lend.Amount;
            }

            erpNodeDBContext.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var Transfer = erpNodeDBContext.Lends.Find(id);
            erpNodeDBContext.Lends.Remove(Transfer);
            this.SaveChanges();
        }

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

            //Console.WriteLine("");
        }
        public bool PostLedger(Lend tr, bool SaveImmediately = true)
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

            trLedger.AddCredit(tr.AssetAccount, tr.Amount);
            trLedger.AddDebit(tr.LendingAssetAccount, tr.Amount);


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

        public void UnPostAllLedger()
        {
            Console.WriteLine("> Un Post " + this.transactionType.ToString());

            string sqlCommand = "DELETE FROM [dbo].[ERP_Ledgers] WHERE TransactionType = {0} ";
            sqlCommand = string.Format(sqlCommand, (int)this.transactionType);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);

            sqlCommand = "DELETE FROM [dbo].[ERP_Ledger_Transactions] WHERE TransactionType =  {0}";
            sqlCommand = string.Format(sqlCommand, (int)this.transactionType);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);

            sqlCommand = "UPDATE [dbo].[ERP_Finance_Lends] SET  [PostStatus] = '0'";
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);

        }
        public void UnPostLedger(Lend lend)
        {
            organization.LedgersDal.RemoveTransaction(lend.Id);
            lend.PostStatus = LedgerPostStatus.ReadyToPost;

            erpNodeDBContext.SaveChanges();
        }


    }
}