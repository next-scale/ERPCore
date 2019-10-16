using ERPCore.Enterprise.DataBase;
using ERPCore.Enterprise.Models.Accounting;
using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.Accounting.FiscalYears;
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ERPCore.Enterprise.Repository.Items
{
    public class ItemsCOSG : ERPNodeDalRepository
    {
        public ItemsCOSG(Organization organization) : base(organization)
        {
            transactionType = TransactionTypes.ItemCOGS;
        }

        public List<PeriodItemCOGS> GetList() => erpNodeDBContext
            .PeriodItemsCOGS
            .ToList();

        public List<PeriodItemCOGS> GetList(FiscalYear fiscal) => erpNodeDBContext
            .PeriodItemsCOGS
            .Where(p => p.FiscalYearId == fiscal.Id)
            .ToList();


        public void CreateCOSG()
        {
            Console.WriteLine("Create COSG Period");

            organization.FiscalYears.GetHistoryList()
                .ForEach(fiscalYear =>
                {
                    Console.WriteLine(" =>" + fiscalYear.Name);
                    this.ClearCOGS(fiscalYear);
                    this.CreateCOSG(fiscalYear);
                    this.UpdateOpeningCOSG(fiscalYear);
                });
        }

        public void CreateCOSG(FiscalYear fiscalYear)
        {
            Console.WriteLine(" + Create Period Cost " + fiscalYear.Name);

            erpNodeDBContext.CommercialItems
                 .Where(ci => ci.Item.ItemType == Models.Items.Enums.ItemTypes.Inventory)
                 .Where(ci => ci.Commercial.TransactionDate >= fiscalYear.StartDate && ci.Commercial.TransactionDate <= fiscalYear.EndDate)
                 .ToList()
                 .ForEach(ci => ci.UpdateInventory());

            var averageCOGSLines = erpNodeDBContext.CommercialItems
                 .Where(ci => ci.Item.ItemType == Models.Items.Enums.ItemTypes.Inventory)
                 .Where(ci => ci.Commercial.TransactionDate >= fiscalYear.StartDate && ci.Commercial.TransactionDate <= fiscalYear.EndDate)
                 .GroupBy(ci => ci.ItemGuid)
                 .ToList()
                 .Select(ci => new
                 {
                     ItemGuid = ci.Key,
                     InputAmount = ci.Sum(i => i.InputAmount),
                     InputCost = ci.Sum(i => i.InputValue),
                     OutputAmount = ci.Sum(i => i.OutputAmount),
                 })
                 .ToList();


            var ItemCOSG = fiscalYear.PeriodItemsCOGS.ToList();

            averageCOGSLines.ForEach(a =>
            {
                var itemCosg = ItemCOSG
                .Where(c => c.ItemGuid == a.ItemGuid)
                .First();

                itemCosg.InputAmount = a.InputAmount;
                itemCosg.InputValue = a.InputCost;
                itemCosg.OutputAmount = a.OutputAmount;
                itemCosg.LastCalculateDate = DateTime.Today;
            });

            erpNodeDBContext.SaveChanges();
        }

        public void UpdateOpeningCOSG(FiscalYear fiscalYear)
        {
            //First Year, copy opening COSG
            if (fiscalYear.PreviousFiscal == null)
                return;


            var previusFiscalCOSG = fiscalYear.PreviousFiscal.PeriodItemsCOGS.ToList();


            GetList(fiscalYear).ForEach(ItemCOSG =>
            {
                var previusFiscalItemCOSG = previusFiscalCOSG
                .Where(prLine => prLine.ItemGuid == ItemCOSG.ItemGuid)
                .FirstOrDefault();

                if (previusFiscalItemCOSG != null)
                {
                    ItemCOSG.OpeningAmount = previusFiscalItemCOSG.RemainAmount;
                    ItemCOSG.OpeningValue = previusFiscalItemCOSG.RemainValue;
                }
            });

            SaveChanges();
        }




        public List<PeriodItemCOGS> GetInventory(DateTime? endDate)
        {

            if (endDate == null)
                endDate = DateTime.Today;

            var AverageItemsCOGSLines = erpNodeDBContext.CommercialItems
                 .Where(ci => ci.Item.ItemType == Models.Items.Enums.ItemTypes.Inventory)
                 .Where(ci => ci.Commercial.TransactionDate <= endDate)
                 .Include(ci => ci.Item)
                 .GroupBy(ci => ci.ItemGuid)
                 .ToList()
                 .Select(ci => new
                 {
                     ItemGuid = ci.Key,
                     ci.First().Item,
                     OutputAmount = ci.Sum(i => i.OutputAmount),
                     InputAmount = ci.Sum(i => i.InputAmount),
                     InputCost = ci.Sum(i => i.InputValue),
                 }).ToList();

            var PeriodItemCOGSList = new List<PeriodItemCOGS>();

            AverageItemsCOGSLines.ForEach(a =>
            {
                var averageItemCOGS = new PeriodItemCOGS()
                {
                    Id = Guid.NewGuid(),
                    ItemGuid = a.ItemGuid,
                    Item = a.Item,
                    InputAmount = a.InputAmount,
                    InputValue = a.InputCost,
                    OutputAmount = a.OutputAmount,
                    LastCalculateDate = DateTime.Today
                };

                PeriodItemCOGSList.Add(averageItemCOGS);
            });

            return PeriodItemCOGSList;
        }

        public void ClearCOGS(FiscalYear fiscalYear)
        {
            var periodCost = fiscalYear.PeriodItemsCOGS.ToList();

            periodCost.ForEach(pc =>
            {
                if (pc.PostStatus == LedgerPostStatus.Posted)
                    UnPostLedger(pc);
                erpNodeDBContext.PeriodItemsCOGS.Remove(pc);
            });

            this.SaveChanges();

            organization.Items.GetInventories.ToList()
                .ForEach(item =>
                {
                    var periodItemCOGS = new PeriodItemCOGS()
                    {
                        Id = Guid.NewGuid(),
                        FiscalYear = fiscalYear,
                        ItemGuid = item.Id,
                        LastCalculateDate = DateTime.Today
                    };
                    fiscalYear.PeriodItemsCOGS.Add(periodItemCOGS);
                });

            this.SaveChanges();
        }



        public List<PeriodItemCOGS> ReadyForPost => erpNodeDBContext
            .PeriodItemsCOGS
            .Where(s => s.PostStatus == LedgerPostStatus.ReadyToPost)
            .ToList();

        public void PostLedger()
        {
            var unPostTransactions = ReadyForPost;
            Console.WriteLine("> {0} Post {2} [{1}]", DateTime.Now.ToLongTimeString(), unPostTransactions.Count(), transactionType.ToString());

            erpNodeDBContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            unPostTransactions.ForEach(s =>
            {
                PostLedger(s, false);
            });

            erpNodeDBContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            erpNodeDBContext.ChangeTracker.DetectChanges();
            SaveChanges();
        }
        public bool PostLedger(PeriodItemCOGS tr, bool SaveImmediately = true)
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
            erpNodeDBContext.LedgerGroups.Add(trLedger);

            trLedger.AddCredit(tr.Item.PurchaseAccount, tr.OutputCost);
            trLedger.AddDebit(tr.Item.COGSAccount, tr.OutputCost);

            tr.PostStatus = LedgerPostStatus.Posted;

            if (SaveImmediately)
                erpNodeDBContext.SaveChanges();

            return true;
        }
        public void UnPostAllLedger()
        {
            Console.WriteLine("> Un Post" + transactionType.ToString());

            string sqlCommand = "DELETE FROM [dbo].[ERP_Ledgers] WHERE TransactionType = {0} ";
            sqlCommand = string.Format(sqlCommand, (int)transactionType);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);

            sqlCommand = "DELETE FROM [dbo].[ERP_Ledger_Transactions] WHERE TransactionType =  {0}";
            sqlCommand = string.Format(sqlCommand, (int)transactionType);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);

            erpNodeDBContext.PeriodItemsCOGS.ToList().ForEach(s => s.PostStatus = LedgerPostStatus.ReadyToPost);
            erpNodeDBContext.SaveChanges();

        }
        public void UnPostLedger(PeriodItemCOGS averageItemCOGS)
        {
            organization.LedgersDal.RemoveTransaction(averageItemCOGS.Id);
            averageItemCOGS.PostStatus = LedgerPostStatus.ReadyToPost; ;
        }

    }
}
