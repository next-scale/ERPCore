
using ERPCore.Enterprise.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Reports.CompanyandFinancial;
using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.Accounting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ERPCore.Enterprise.Repository.Accounting
{
    public class Ledgers : ERPNodeDalRepository
    {

        public Ledgers(Organization organization) : base(organization)
        {


        }

        public IQueryable<Models.Accounting.LedgerLine> All => erpNodeDBContext.Ledgers;

        public IQueryable<Models.Accounting.LedgerLine> GetLedgersByAccountId(Guid accountId, DateTime? viewDate)
        {


            var ledgers = All.Where(journal => journal.AccountId == accountId);

            if (viewDate != null)
                ledgers = ledgers.Where(i => i.TransactionDate == viewDate);

            ledgers.OrderBy(i => i.TransactionDate)
                .ThenBy(i => i.TransactionType)
                .ThenBy(i => i.TransactionName);

            return ledgers;
        }

        public IQueryable<Models.Accounting.LedgerLine> GetByTransactionId(Guid transactionId)
        {
            return All.Where(journal => journal.TransactionId == transactionId);
        }

        public List<Models.Accounting.LedgerLine> GetByTransactionType(TransactionTypes transactionType)
        {
            return erpNodeDBContext.Ledgers
                .Where(journal => journal.TransactionType == transactionType)
                .OrderBy(journal => journal.accountItem.No)
                .ToList();
        }

        public List<LedgerLine> GetList(Guid? id) => erpNodeDBContext.Ledgers
                     .Where(gl => gl.TransactionId == id)
                     .Include(gl => gl.accountItem)
                     .ToList();

        public IQueryable<Models.Accounting.LedgerLine> Query => erpNodeDBContext.Ledgers;
        public IQueryable<Models.Accounting.LedgerLine> QueryBy(enumTrialBalaceViewType type, Guid? fiscalYearId)
        {
            switch (type)
            {
                case enumTrialBalaceViewType.Default:
                    return erpNodeDBContext.Ledgers;
                case enumTrialBalaceViewType.Today:
                    return erpNodeDBContext.Ledgers.Where(gl => gl.TransactionDate == DateTime.Today);
                case enumTrialBalaceViewType.Last7Day:
                    var Last7Day = DateTime.Today.AddDays(-7);
                    return erpNodeDBContext.Ledgers.Where(gl => gl.TransactionDate > Last7Day);
                case enumTrialBalaceViewType.LastWeek:
                    return erpNodeDBContext.Ledgers;
                case enumTrialBalaceViewType.Last30Day:
                    var Last30Day = DateTime.Today.AddDays(-30);
                    return erpNodeDBContext.Ledgers.Where(gl => gl.TransactionDate > Last30Day);
                case enumTrialBalaceViewType.LastMonth:
                    return erpNodeDBContext.Ledgers.Where(gl => gl.TransactionDate.Year == DateTime.Today.Year && gl.TransactionDate.Month == DateTime.Today.Month);
                case enumTrialBalaceViewType.Yeartodate:
                    return erpNodeDBContext.Ledgers.Where(gl => gl.TransactionDate.Year == (DateTime.Today.Year));
                case enumTrialBalaceViewType.Lastyear:
                    return erpNodeDBContext.Ledgers.Where(gl => gl.TransactionDate.Year == (DateTime.Today.Year - 1));
                case enumTrialBalaceViewType.Last365:
                    var Last365 = DateTime.Today.AddYears(-1);
                    return erpNodeDBContext.Ledgers.Where(gl => gl.TransactionDate > Last365);
                case enumTrialBalaceViewType.FiscalYear:
                    var fiscalYear = erpNodeDBContext.FiscalYears.Find(fiscalYearId);
                    return erpNodeDBContext.Ledgers.Where(gl => gl.TransactionDate >= fiscalYear.StartDate && gl.TransactionDate <= fiscalYear.EndDate);
                default:
                    return erpNodeDBContext.Ledgers;
            }
        }

        public void RemoveLedgers(Models.Accounting.Enums.TransactionTypes transactionType)
        {
            var removeLedgers = erpNodeDBContext.Ledgers
                .Where(ji => ji.TransactionType == transactionType);
            erpNodeDBContext.Ledgers.RemoveRange(removeLedgers);
            erpNodeDBContext.SaveChanges();
        }

        public void RemoveTransaction(Guid trId)
        {
            var removeLdegers = erpNodeDBContext.Ledgers
                .Where(x => x.TransactionId == trId);
            erpNodeDBContext.Ledgers.RemoveRange(removeLdegers);
            erpNodeDBContext.SaveChanges();

            var removeTrLedger = erpNodeDBContext.LedgerGroups.Find(trId);
            if (removeTrLedger != null)
                erpNodeDBContext.LedgerGroups.Remove(removeTrLedger);
            erpNodeDBContext.SaveChanges();
        }

        public void UnPostAllLedgers()
        {


            string sqlCommand = "DELETE FROM [dbo].[ERP_Ledgers]";
            sqlCommand = string.Format(sqlCommand);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);


            sqlCommand = "DELETE FROM [dbo].[ERP_Ledger_Transactions] ";
            sqlCommand = string.Format(sqlCommand);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);


            organization.Purchases.UnPostAllLedger();
            organization.Sales.UnPostAllLedger();


            organization.ReceivePayments.UnPostAllLedger();
            organization.SupplierPayments.UnPostAllLedger();
            organization.LiabilityPayments.UnPostAllLedger();


            organization.FundTransfers.UnPostAllLedger();
            organization.CapitalActivities.UnPostAllLedger();


            organization.TaxPeriods.UnPostAllLedger();
            organization.IncomeTaxes.UnPostAllLedger();


            organization.Loans.UnPostAllLedger();
            organization.Lends.UnPostAllLedger();


            organization.EmployeePayments.UnPostAllLedger();
            organization.JournalEntries.UnPostAllLedger();
            organization.ItemsCOGS.UnPostAllLedger();
            organization.OpeningEntries.UnPostLedger();
            organization.FiscalYears.UnPostAllLedger();


            organization.FixedAssets.UnPostAllLedger();
            organization.FixedAssetDreprecateSchedules.UnPostAllLedger();


            Console.WriteLine("> UnPost Complete ");
        }

        public void PostLedgers()
        {
            organization.OpeningEntries.PostLedger();

            //Capital Activities
            organization.CapitalActivities.PostLedger();

            ////Commercial Section
            organization.Sales.PostLedger();
            organization.Purchases.PostLedger();

            //Financial Section
            organization.ReceivePayments.PostLedger();
            organization.SupplierPayments.PostLedger();
            organization.LiabilityPayments.PostLedger();

            organization.FundTransfers.PostLedger();
            organization.Loans.PostLedger();
            organization.Lends.PostLedger();

            //Taxes Section
            organization.TaxPeriods.PostLedger();
            organization.IncomeTaxes.PostLedger();

            //Employee Section
            organization.EmployeePayments.PostLedger();

            //Other Section
            organization.JournalEntries.PostLedger();

            //Assets
            organization.FixedAssets.PostLedger();
            organization.FixedAssetDreprecateSchedules.PostLedger();

            //InventoryCOSG
            organization.FiscalYears.PostLedger();
        }

        public void UnPostAllLedgers(TransactionTypes trType)
        {
            Console.WriteLine("> Un Post" + trType.ToString());

            string sqlCommand = "DELETE FROM [dbo].[ERP_Ledgers] WHERE TransactionType = {0} ";
            sqlCommand = string.Format(sqlCommand, (int)trType);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);

            sqlCommand = "DELETE FROM [dbo].[ERP_Ledger_Transactions] WHERE TransactionType =  {0}";
            sqlCommand = string.Format(sqlCommand, (int)trType);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);

            erpNodeDBContext.SaveChanges();
        }
    }
}