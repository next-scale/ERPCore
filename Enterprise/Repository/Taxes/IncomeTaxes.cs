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
    public class IncomeTaxes : ERPNodeDalRepository
    {
        public IncomeTaxes(Organization organization) : base(organization)
        {
            transactionType = TransactionTypes.IncomeTax;
        }

        public List<IncomeTax> ALL => erpNodeDBContext.IncomeTaxs.ToList();

        public Models.Taxes.IncomeTax Find(Guid id) => erpNodeDBContext.IncomeTaxs.Find(id);

        public IncomeTax Create()
        {
            var taxDate = DateTime.Today;

            var newIncomeTax = new IncomeTax()
            {
                Id = Guid.NewGuid(),
                TrDate = taxDate,
                FiscalYearId = organization.FiscalYears.Find(taxDate).Id,
            };
            erpNodeDBContext.IncomeTaxs.Add(newIncomeTax);
            erpNodeDBContext.SaveChanges();
            return newIncomeTax;

        }

        public bool Delete(Guid id)
        {
            var incomeTax = this.Find(id);
            if (incomeTax.PostStatus != LedgerPostStatus.Posted)
            {
                erpNodeDBContext.IncomeTaxs.Remove(incomeTax);
                this.SaveChanges();
                return true;
            }
            else
                return false;
        }

        public bool Edit(Guid id)
        {
            var incomeTax = this.Find(id);
            if (incomeTax.PostStatus == LedgerPostStatus.Posted)
            {
                this.UnPostLedger(incomeTax);
                this.SaveChanges();
                return true;
            }
            return false;
        }

        public List<IncomeTax> GetAssignAble(TransactionTypes transactionType, DateTime transactionDate)
        {
            var taxPeriods = erpNodeDBContext.IncomeTaxs
                .Where(s => s.TrDate >= transactionDate)
                .OrderBy(s => s.TrDate)
                .ToList();
            return taxPeriods;
        }

        public List<IncomeTax> List => erpNodeDBContext.IncomeTaxs.ToList();


        public IncomeTax Save(Models.Taxes.IncomeTax salesTax)
        {
            var existSalesTax = erpNodeDBContext.IncomeTaxs.Find(salesTax.Id);

            if (existSalesTax.PostStatus == LedgerPostStatus.Posted)
                return existSalesTax;

            existSalesTax.TrDate = new DateTime(salesTax.TrDate.Year, salesTax.TrDate.Month, 1).AddMonths(1).AddDays(-1);
            erpNodeDBContext.SaveChanges();

            return existSalesTax;
        }

        public IncomeTax Create(Guid taxCodeId)
        {
            var newCoperateTax = new IncomeTax()
            {
                Id = Guid.NewGuid(),
                TrDate = DateTime.Today
            };
            erpNodeDBContext.IncomeTaxs.Add(newCoperateTax);
            erpNodeDBContext.SaveChanges();

            return newCoperateTax;
        }



        public void UpdateStartDate()
        {
            Console.WriteLine("> Generate Sales Tax");

            this.ALL
                .ToList()
                .ForEach(a =>
                {
                    this.Save(a);
                });

            erpNodeDBContext.SaveChanges();
        }

        public List<IncomeTax> ReadyForPost => erpNodeDBContext.IncomeTaxs
            .Where(s => s.PostStatus == LedgerPostStatus.ReadyToPost).ToList();





        public List<IncomeTax> GetLedger(IncomeTax taxPeriod) => erpNodeDBContext.IncomeTaxs
            .Where(tr => tr.Id == taxPeriod.Id)
            .ToList();





        public void ReOrder()
        {
            var transactions = erpNodeDBContext.IncomeTaxs
                .OrderBy(t => t.TrDate)
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

        public bool PostLedger(IncomeTax tr, bool SaveImmediately = true)
        {
            if (tr.PostStatus == LedgerPostStatus.Posted)
                return false;

            var trLedger = new Models.Accounting.LedgerGroup()
            {
                Id = tr.Id,
                TransactionDate = tr.TrDate,
                TransactionName = tr.Name,
                TransactionNo = tr.No,
                TransactionType = transactionType
            };

            trLedger.AddCredit(tr.LiabilityAccount, tr.TaxAmount);
            trLedger.AddDebit(tr.IncomeTaxExpenAccount, tr.TaxAmount);

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


        public void UnPostLedger(IncomeTax tax)
        {
            if (tax.PostStatus == LedgerPostStatus.Posted)
            {


                organization.LedgersDal.RemoveTransaction(tax.Id);
                tax.PostStatus = LedgerPostStatus.ReadyToPost;
                erpNodeDBContext.SaveChanges();
            }
        }

        public void UnPostAllLedger()
        {
            Console.WriteLine("> Un Post" + TransactionTypes.IncomeTax.ToString());

            string sqlCommand = "DELETE FROM [dbo].[ERP_Ledgers] WHERE TransactionType = {0} ";
            sqlCommand = string.Format(sqlCommand, (int)this.transactionType);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);

            sqlCommand = "DELETE FROM [dbo].[ERP_Ledger_Transactions] WHERE TransactionType =  {0}";
            sqlCommand = string.Format(sqlCommand, (int)this.transactionType);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);

            erpNodeDBContext.IncomeTaxs.ToList().ForEach(s => s.PostStatus = LedgerPostStatus.ReadyToPost);
            erpNodeDBContext.SaveChanges();
        }


    }
}