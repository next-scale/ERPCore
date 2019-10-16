using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.ChartOfAccount.Enums;
using ERPCore.Enterprise.Models.Company;
using ERPCore.Enterprise.Models.Taxes;
using ERPCore.Enterprise.Models.Transactions;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using ERPCore.Enterprise.Models.Taxes.Enums;

namespace ERPCore.Enterprise.Repository.Taxes
{
    public class TaxPeriods : ERPNodeDalRepository
    {
        public TaxPeriods(Organization organization) : base(organization)
        {
            transactionType = Models.Accounting.Enums.TransactionTypes.TaxPeriod;
        }

        public List<TaxPeriod> ALL => erpNodeDBContext.TaxPeriods.ToList();

        public Models.Taxes.TaxPeriod Find(Guid id) => erpNodeDBContext.TaxPeriods.Find(id);

        public List<TaxPeriod> GetAssignAble(TransactionTypes transactionType, DateTime transactionDate)
        {
            var taxPeriods = erpNodeDBContext.TaxPeriods
                .Where(s => s.TransactionDate >= transactionDate)
                .OrderBy(s => s.TransactionDate)
                .ToList();
            return taxPeriods;
        }

        public List<TaxPeriod> List => erpNodeDBContext.TaxPeriods.ToList();


        public TaxPeriod Save(Models.Taxes.TaxPeriod salesTax)
        {
            var existSalesTax = erpNodeDBContext.TaxPeriods.Find(salesTax.Id);

            if (existSalesTax.PostStatus == LedgerPostStatus.Posted)
                return existSalesTax;

            existSalesTax.TransactionDate = new DateTime(salesTax.TransactionDate.Year, salesTax.TransactionDate.Month, 1).AddMonths(1).AddDays(-1);
            existSalesTax.CloseToAccountGuid = salesTax.CloseToAccountGuid;
            erpNodeDBContext.SaveChanges();

            return existSalesTax;
        }

        public TaxPeriod Create()
        {
            var newTaxPeriod = new TaxPeriod()
            {
                Id = Guid.NewGuid(),
                TransactionDate = DateTime.Today,
            };
            erpNodeDBContext.TaxPeriods.Add(newTaxPeriod);
            erpNodeDBContext.SaveChanges();

            return newTaxPeriod;
        }

        public void ReCalculate()
        {
            this.ALL.ToList().ForEach(a => a.ReCalculate());
            erpNodeDBContext.SaveChanges();
        }

        public void UpdateStartDate()
        {
            Console.WriteLine("> Generate Sales Tax");

            this.ALL.ToList()
                .ForEach(a =>
                {
                    this.Save(a);
                });

            erpNodeDBContext.SaveChanges();
        }

        public List<TaxPeriod> ReadyForPost => erpNodeDBContext.TaxPeriods
            .Where(s => s.PostStatus == LedgerPostStatus.ReadyToPost)
            .Where(s => s.CloseToAccountGuid != null)
            .ToList();



        public List<CommercialTax> GetUnassignCommercialTaxes(TaxPeriod taxPeriod)
        {
            var unassignCommercials = organization.CommercialTaxes.GetAssignAbleCommercialTaxesList()
                .Where(com => com.Commercial.TransactionDate <= taxPeriod.TransactionDate)
                .ToList();

            return unassignCommercials;
        }

        public List<CommercialTax> GetUnassignCommercials() => erpNodeDBContext.CommercialTaxes
                .Where(com => com.TaxPeriodId == null)
                .ToList();


        public int AssignCommercialTaxs(TaxPeriod taxPeriod, string commercialsId)
        {
            List<Guid> commercialsIdList = commercialsId?.Trim().Split(',')
                .Select(s => Guid.Parse(s))
                .ToList();

            var unassignComTaxes = erpNodeDBContext.CommercialTaxes
                    .Where(c => commercialsIdList.Contains(c.Id))
                    .ToList();

            int assignCount = 0;

            unassignComTaxes.ForEach(comTax =>
            {
                assignCount++;
                taxPeriod.AddCommercialTax(comTax, false);
            });

            taxPeriod.ReCalculate();
            erpNodeDBContext.SaveChanges();

            return assignCount;
        }

        public List<TaxPeriod> GetLedger(TaxPeriod taxPeriod) => erpNodeDBContext.TaxPeriods
            .Where(tr => tr.Id == taxPeriod.Id)
            .ToList();

        public void AutoAssignCommercial()
        {
            Console.WriteLine("> Generate Sales Tax");

            this.ALL.Where(s => s.PostStatus == LedgerPostStatus.Posted).ToList()
                .ForEach(a => this.AutoAssignCommercial(a));

            erpNodeDBContext.SaveChanges();
        }

        public void AutoAssignCommercial(TaxPeriod taxPeriod)
        {
            if (taxPeriod.PostStatus == LedgerPostStatus.Posted)
                this.UnPostLedger(taxPeriod);

            var comTaxes = this.GetUnassignCommercialTaxes(taxPeriod);

            comTaxes.ToList().ForEach(comTax =>
            {
                taxPeriod.CommercialTaxes.Add(comTax);
            });

            taxPeriod.ReCalculate();
            erpNodeDBContext.SaveChanges();
        }

        public void Delete(TaxPeriod taxPeriod)
        {
            taxPeriod.CommercialTaxes.ToList().ForEach(c =>
            {
                taxPeriod.CommercialTaxes.Remove(c);
            });

            erpNodeDBContext.TaxPeriods.Remove(taxPeriod);

            organization.SaveChanges();
        }

        public void ReOrder()
        {
            var transactions = erpNodeDBContext.TaxPeriods
                .OrderBy(t => t.TransactionDate)
                .ToList();

            int i = 1;
            transactions.ForEach(tr => tr.No = i++);

            erpNodeDBContext.SaveChanges();
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

        public bool PostLedger(TaxPeriod taxPeriod, bool SaveImmediately = true)
        {
            if (taxPeriod.PostStatus == LedgerPostStatus.Posted)
                return false;

            taxPeriod.ReCalculate();

            var trLedger = new Models.Accounting.LedgerGroup()
            {
                Id = taxPeriod.Id,
                TransactionDate = taxPeriod.TransactionDate,
                TransactionName = taxPeriod.Name,
                TransactionNo = taxPeriod.No ?? 0,
                TransactionType = transactionType
            };

            taxPeriod.GetCommercialTaxGroups().ForEach(taxGroup =>
            {
                switch (taxGroup.TaxCode.TaxDirection)
                {
                    case TaxDirection.Input:
                        Console.WriteLine($"INP =>{taxGroup.TaxCode.TaxAccount.Name}{taxGroup.TaxBalance}");
                        trLedger.AddCredit(taxGroup.TaxCode.TaxAccount, taxGroup.TaxBalance);
                        break;
                    case TaxDirection.Output:
                        Console.WriteLine($"OUT =>{taxGroup.TaxCode.TaxAccount.Name}{taxGroup.TaxBalance}");
                        trLedger.AddDebit(taxGroup.TaxCode.TaxAccount, taxGroup.TaxBalance);
                        break;
                }
            });

            if (taxPeriod.CloseToAccount.Type == Models.ChartOfAccount.AccountTypes.Asset)
                trLedger.AddDebit(taxPeriod.CloseToAccount, Math.Abs(taxPeriod.ClosingAmount));
            else if (taxPeriod.CloseToAccount.Type == Models.ChartOfAccount.AccountTypes.Liability)
                trLedger.AddCredit(taxPeriod.CloseToAccount, Math.Abs(taxPeriod.ClosingAmount));

            if (trLedger.FinalValidate())
            {
                erpNodeDBContext.LedgerGroups.Add(trLedger);
                taxPeriod.PostStatus = LedgerPostStatus.Posted;
            }

            if (SaveImmediately && taxPeriod.PostStatus == LedgerPostStatus.Posted)
                erpNodeDBContext.SaveChanges();

            return true;
        }


        public void UnPostLedger(TaxPeriod salesTax)
        {
            // Console.WriteLine("> Un Posting ,salesTax" + salesTax.No);
            organization.LedgersDal.RemoveTransaction(salesTax.Id);
            salesTax.PostStatus = LedgerPostStatus.ReadyToPost;
            erpNodeDBContext.SaveChanges();
        }

        public void UnPostAllLedger()
        {
            Console.WriteLine("> Un Post" + Models.Accounting.Enums.TransactionTypes.TaxPeriod.ToString());

            string sqlCommand = "DELETE FROM [dbo].[ERP_Ledgers] WHERE TransactionType = {0} ";
            sqlCommand = string.Format(sqlCommand, (int)this.transactionType);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);

            sqlCommand = "DELETE FROM [dbo].[ERP_Ledger_Transactions] WHERE TransactionType =  {0}";
            sqlCommand = string.Format(sqlCommand, (int)this.transactionType);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);

            erpNodeDBContext.TaxPeriods.ToList().ForEach(s => s.PostStatus = LedgerPostStatus.ReadyToPost);
            erpNodeDBContext.SaveChanges();
        }


    }
}