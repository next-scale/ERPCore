using ERPCore.Enterprise.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Models.Financial;
using ERPCore.Enterprise.Models.Transactions.Commercials;
using ERPCore.Enterprise.Models.Assets;
using ERPCore.Enterprise.Models.Accounting;
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Accounting.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERPCore.Enterprise.Repository.Assets
{
    public class DeprecateSchedules : ERPNodeDalRepository
    {
        public DeprecateSchedules(Organization organization) : base(organization)
        {
            transactionType = TransactionTypes.DeprecateSchedule;
        }

        public IQueryable<Models.Assets.DeprecateSchedule> All => erpNodeDBContext.DeprecateSchedules;
        public List<Models.Assets.DeprecateSchedule> ListAll => erpNodeDBContext.DeprecateSchedules.ToList();

        public DeprecateSchedule Find(Guid transactionId) => erpNodeDBContext.DeprecateSchedules.Find(transactionId);

        public void Clean()
        {
            //Remove null Schedule
            var nullSchedule = erpNodeDBContext.DeprecateSchedules.Where(s => s.FixedAssetId == null);
            erpNodeDBContext.DeprecateSchedules.RemoveRange(nullSchedule);
            erpNodeDBContext.SaveChanges();
        }

        public DeprecateSchedule Save(DeprecateSchedule fixedAssetSchedule)
        {
            var existDepreciationSchedule = erpNodeDBContext.DeprecateSchedules.Find(fixedAssetSchedule.Id);


            if (existDepreciationSchedule != null)
            {
                existDepreciationSchedule.FiscalYear = organization.FiscalYears.Find(existDepreciationSchedule.EndDate);

                if (existDepreciationSchedule.PostStatus == LedgerPostStatus.Posted)
                    return existDepreciationSchedule;

                erpNodeDBContext.SaveChanges();
                return existDepreciationSchedule;
            }
            else
            {
                fixedAssetSchedule.FiscalYear = organization.FiscalYears.Find(existDepreciationSchedule.EndDate);

                erpNodeDBContext.DeprecateSchedules.Add(fixedAssetSchedule);
                erpNodeDBContext.SaveChanges();

                return fixedAssetSchedule;
            }




        }

        public void RemoveAll()
        {
            var deprecatedAssets = erpNodeDBContext.DeprecateSchedules.ToList();

            foreach (var deprecatedAsset in deprecatedAssets)
            {
                erpNodeDBContext.DeprecateSchedules.Remove(deprecatedAsset);
            }

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

            erpNodeDBContext.DeprecateSchedules.ToList().ForEach(s => s.PostStatus = LedgerPostStatus.ReadyToPost);
            erpNodeDBContext.SaveChanges();
        }
        public List<DeprecateSchedule> ReadyForPost
        {
            get
            {
                organization.FixedAssets.RemoveNullSchedule();
                DateTime firstPostDate = organization.DataItems.FirstDate;

                return erpNodeDBContext.DeprecateSchedules
                    .Where(s => s.PostStatus == LedgerPostStatus.ReadyToPost)
                    .Where(s => s.BeginDate >= firstPostDate)
                    .Where(s => s.EndDate <= DateTime.Today)
                    .ToList();

            }
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


        }


        public void UnPostLedger(DeprecateSchedule fixedAssetSchedule)
        {
            organization.LedgersDal.RemoveTransaction(fixedAssetSchedule.Id);
            fixedAssetSchedule.PostStatus = LedgerPostStatus.ReadyToPost;
        }
        public bool PostLedger(DeprecateSchedule tr, bool SaveImmediately = true)
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
           

            trLedger.AddDebit(tr.FixedAsset.FixedAssetType.AmortizeExpenseAccount, tr.DepreciationValue);
            trLedger.AddCredit(tr.FixedAsset.FixedAssetType.AccumulateDeprecateAcc, tr.DepreciationValue);

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