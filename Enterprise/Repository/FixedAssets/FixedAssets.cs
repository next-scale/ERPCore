using ERPCore.Enterprise.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Models.Financial;
using ERPCore.Enterprise.Models.Transactions.Commercials;
using ERPCore.Enterprise.Models.Assets;
using ERPCore.Enterprise.Models.Accounting;
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Assets.Enums;
using ERPCore.Enterprise.Models.Accounting.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERPCore.Enterprise.Repository.Assets
{
    public class FixedAssets : ERPNodeDalRepository
    {
        public FixedAssets(Organization organization) : base(organization)
        {
            transactionType = Models.Accounting.Enums.TransactionTypes.FixedAsset;
        }

        public IQueryable<FixedAsset> Query => erpNodeDBContext.FixedAssets;

        public List<FixedAsset> ListAll => Query.ToList();


        public FixedAsset Find(Guid uid) => erpNodeDBContext.FixedAssets.Find(uid);

        public FixedAsset Save(FixedAsset fixedAsset)
        {
            var exFixedAsset = erpNodeDBContext.FixedAssets.Find(fixedAsset.Id);

            if (exFixedAsset != null)
            {
                if (exFixedAsset.PostStatus == LedgerPostStatus.Posted)
                    return exFixedAsset;

                exFixedAsset.FiscalYear = organization.FiscalYears.Find(fixedAsset.StartDeprecationDate);
                exFixedAsset.Reference = fixedAsset.Reference;
                exFixedAsset.Memo = fixedAsset.Memo;
                exFixedAsset.Name = fixedAsset.Name;
                exFixedAsset.Code = fixedAsset.Code;
                exFixedAsset.FixedAssetTypeId = fixedAsset.FixedAssetTypeId;
                exFixedAsset.StartDeprecationDate = fixedAsset.StartDeprecationDate;
                exFixedAsset.AssetValue = fixedAsset.AssetValue;
                exFixedAsset.SavageValue = fixedAsset.SavageValue;
                exFixedAsset.PreDepreciationValue = fixedAsset.PreDepreciationValue;


                erpNodeDBContext.SaveChanges();
                return exFixedAsset;
            }
            else
            {
                fixedAsset.FiscalYear = organization.FiscalYears.Find(fixedAsset.StartDeprecationDate);
                erpNodeDBContext.FixedAssets.Add(fixedAsset);
                erpNodeDBContext.SaveChanges();

                return fixedAsset;
            }
        }

        public List<FixedAsset> ListByStatus(FixedAssetStatus status) => erpNodeDBContext.FixedAssets.Where(d => d.Status == status).ToList();

        public void UpdateStatus()
        {
            this.ListAll.ForEach(fixedAsset =>
            {
                fixedAsset.UpdateStatus();
            });

            erpNodeDBContext.SaveChanges();
        }

        public IQueryable<DeprecateSchedule> GetDeprecateSchedule(DateTime date)
        {
            return erpNodeDBContext.DeprecateSchedules
               .Where(d => d.EndDate <= date)
               .GroupBy(s => s.FixedAssetId)
               .Select(s => s.OrderByDescending(x => x.EndDate).FirstOrDefault());
        }
        public IQueryable<Models.Accounting.FiscalYear> FiscalYearChoice()
        {
            return erpNodeDBContext.DeprecateSchedules
               .GroupBy(s => s.FiscalYear)
               .Select(s => s.Key);
        }
        public void CreateSchedule()
        {
            erpNodeDBContext.FixedAssets.ToList().ForEach(d =>
            {
                this.CreateDepreciationSchedule(d);
            });

            erpNodeDBContext.SaveChanges();
            this.RemoveNullSchedule();
            this.UpdateFiscal();
        }
        public void CreateDepreciationSchedule(FixedAsset fixedAsset)
        {
            this.RemoveDepreciationSchedule(fixedAsset);

            if (fixedAsset.FixedAssetType == null)
                return;

            if (organization.DataItems.FirstDate > fixedAsset.EndDeprecationDate)
                return;


            var periodStartDate = new DateTime(Math.Max(organization.DataItems.FirstDate.Date.Ticks, fixedAsset.StartDeprecationDate.Date.Ticks));
            var currentFiscal = organization.FiscalYears.Find(periodStartDate);
            var periodEndDate = new DateTime(Math.Min(currentFiscal.EndDate.Date.Ticks, fixedAsset.EndDeprecationDate.Date.Ticks));

            decimal depAcc = fixedAsset.PreDepreciationValue;
            int index = 1;

            while (depAcc < fixedAsset.TotalDepreciationValue)
            {
                Console.WriteLine(fixedAsset.Code + "::" + currentFiscal.Name);

                DeprecateSchedule period = new DeprecateSchedule()
                {
                    Id = Guid.NewGuid(),
                    FixedAssetId = fixedAsset.Id,
                    FixedAsset = fixedAsset,
                    BeginDate = periodStartDate,
                    EndDate = periodEndDate,
                    FiscalYear = currentFiscal,
                    No = index++,
                };
                fixedAsset.DepreciationSchedules.Add(period);

                var expectDeprecateValue = Decimal.Round((fixedAsset.DeprecatePerYear * (decimal)period.DayCount / (decimal)period.FiscalYear.DayCount), 2);

                if (depAcc + expectDeprecateValue > fixedAsset.TotalDepreciationValue)
                    expectDeprecateValue = fixedAsset.TotalDepreciationValue - depAcc;

                period.DepreciationValue = expectDeprecateValue;
                depAcc += period.DepreciationValue;
                period.DepreciateAccumulation = depAcc;


                periodStartDate = currentFiscal.EndDate.AddDays(1);
                currentFiscal = organization.FiscalYears.Find(periodStartDate);
                periodEndDate = new DateTime(Math.Min(currentFiscal.EndDate.Ticks, fixedAsset.EndDeprecationDate.Ticks));
            }
        }
        public void RemoveDepreciationSchedule(FixedAsset fixedAsset)
        {
            foreach (var schedule in fixedAsset.DepreciationSchedules.ToList())
            {
                fixedAsset.DepreciationSchedules.Remove(schedule);
            }
        }


        public void UpdateFiscal()
        {
            erpNodeDBContext.DeprecateSchedules.ToList().ForEach(d =>
            {
                d.FiscalYear = organization.FiscalYears.Find(d.EndDate);
            });

            erpNodeDBContext.SaveChanges();


        }
        public void ReOrder()
        {
            var assetGroups = erpNodeDBContext.FixedAssets.ToList()
                .GroupBy(a => a.StartDeprecationDate.Year)
                .Select(go => new
                {
                    year = go.Key,
                    Assets = go.OrderBy(a => a.StartDeprecationDate).ToList(),
                })
                .ToList();


            assetGroups.ForEach(g =>
            {
                int i = 0;
                g.Assets.ForEach(a =>
                {
                    a.No = ++i;
                    a.FiscalYear = organization.FiscalYears.Find(a.TransactionDate);

                });

            });

            erpNodeDBContext.SaveChanges();
        }

        public void RemoveNullSchedule()
        {
            var nullSchedules = erpNodeDBContext.DeprecateSchedules
                .Where(s => s.FixedAssetId == null);

            erpNodeDBContext.DeprecateSchedules.RemoveRange(nullSchedules);
            erpNodeDBContext.SaveChanges();
        }
        public void SyncTransaction()
        {
            var fixedAssetsTransactionItems = erpNodeDBContext
                .CommercialItems
                .Where(ti => ti.Item.ItemType == Models.Items.Enums.ItemTypes.Asset)
                .Where(ti => ti.Commercial.Status == CommercialStatus.Open)
                .ToList();

            foreach (var fixedAssetsTransactionItem in fixedAssetsTransactionItems)
            {
                Models.Assets.FixedAsset fixedAsset = new FixedAsset()
                {
                    Id = fixedAssetsTransactionItem.Id,
                    AssetValue = fixedAssetsTransactionItem.LineTotal,
                    Name = fixedAssetsTransactionItem.Item.PartNumber
                };

                erpNodeDBContext.FixedAssets.Add(fixedAsset);
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

            erpNodeDBContext.FixedAssets.ToList()
                .ForEach(s => s.PostStatus = LedgerPostStatus.ReadyToPost);
            erpNodeDBContext.SaveChanges();
        }
        public List<FixedAsset> ReadyForPost
        {
            get
            {
                var firstDate = organization.DataItems.FirstDate;
                return erpNodeDBContext.FixedAssets
                    .Where(s => s.PostStatus == LedgerPostStatus.ReadyToPost && s.StartDeprecationDate >= firstDate)
                    .OrderBy(s => s.StartDeprecationDate).ToList();
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
        public void UnPostLedger(FixedAsset fixedAsset)
        {
            organization.LedgersDal.RemoveTransaction(fixedAsset.Id);
            fixedAsset.PostStatus = LedgerPostStatus.ReadyToPost;
            erpNodeDBContext.SaveChanges();
        }
        public bool PostLedger(FixedAsset tr, bool SaveImmediately = true)
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
     

            trLedger.AddCredit(tr.FixedAssetType.AssetAccount, tr.AssetValue);
            trLedger.AddDebit(tr.FixedAssetType.AwaitDeprecateAccount, tr.AssetValue);


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