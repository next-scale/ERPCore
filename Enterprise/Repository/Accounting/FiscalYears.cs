
using ERPCore.Enterprise.Repository.Company;
using ERPCore.Enterprise.DataBase;
using ERPCore.Enterprise.Models.Accounting;
using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.Accounting.FiscalYears;
using ERPCore.Enterprise.Models.ChartOfAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ERPCore.Enterprise.Repository.Accounting
{
    public class FiscalYears : ERPNodeDalRepository
    {
        public FiscalYears(Organization organization) : base(organization)
        {
            transactionType = TransactionTypes.FiscalYearClosing;
        }

        
        public List<FiscalYear> Periods { get; set; }
        public List<FiscalYear> All => erpNodeDBContext.FiscalYears.ToList();
        public List<FiscalYear> ReadyForPost => erpNodeDBContext.FiscalYears
            .Where(s => s.Status == EnumFiscalYearStatus.Close)
            .Where(s => s.PostStatus == LedgerPostStatus.ReadyToPost)
            .ToList();
        public FiscalYear FirstPeriod => All.OrderBy(f => f.StartDate).FirstOrDefault();
        public FiscalYear CurrentPeriod => Find(DateTime.Today);
        public List<FiscalYear> GetHistoryList() => erpNodeDBContext.FiscalYears
            .Where(period => period.StartDate <= DateTime.Today)
            .OrderBy(period => period.StartDate)
            .ToList();
        public FiscalYear Find(Guid uid) => erpNodeDBContext.FiscalYears.Find(uid);
        public FiscalYear Find(DateTime date)
        {
            if (date < organization.DataItems.FirstDate)
                return null;

            if (this.Periods == null)
                this.Periods = this.All;


            FiscalYear fiscalYear = this.Periods
                        .Where(f => date >= f.StartDate && date <= f.EndDate)
                        .FirstOrDefault();

            if (fiscalYear == null)
                fiscalYear = this.Create(date);

            return fiscalYear;
        }

        public void UpdatePreviousFiscalYear(FiscalYear fiscalYear)
        {
            var previousFiscal = this.Find(fiscalYear.StartDate.AddDays(-1));
            fiscalYear.PreviousFiscal = previousFiscal;
            this.SaveChanges();
        }

        public FiscalYear Create(DateTime date)
        {
            if (date < organization.DataItems.FirstDate)
                return null;

            var firstDate = organization.DataItems.FirstDate;

            DateTime tagetFiscalFirstDate = new DateTime(date.Year, firstDate.Month, firstDate.Day);

            if (tagetFiscalFirstDate > date)
                tagetFiscalFirstDate = new DateTime(date.Year - 1, firstDate.Month, firstDate.Day);

            var fiscalYear = erpNodeDBContext.FiscalYears
                .Where(p => p.StartDate == tagetFiscalFirstDate.Date)
                .FirstOrDefault();

            if (fiscalYear == null)
            {
                fiscalYear = new FiscalYear()
                {
                    StartDate = tagetFiscalFirstDate,
                    Status = EnumFiscalYearStatus.Open
                };


                erpNodeDBContext.FiscalYears.Add(fiscalYear);
                erpNodeDBContext.SaveChanges();
            }

            return fiscalYear;
        }
        public bool IsFirstPeriod(FiscalYear fiscalYear) => fiscalYear.Id == this.FirstPeriod.Id;
        public void Save(FiscalYear fiscalYear)
        {
            var existFiscalYear = Find(fiscalYear.Id);

            fiscalYear.ClosingAccountGuid = fiscalYear.ClosingAccountGuid ?? this.organization.SystemAccounts.RetainedEarning?.Id;

            if (existFiscalYear.Status != EnumFiscalYearStatus.Close)
                existFiscalYear.ClosingAccountGuid = fiscalYear.ClosingAccountGuid;
            else
                throw new System.Exception("Cannot save closed periods.");

            erpNodeDBContext.SaveChanges();
        }
        public void CalculateOpeningEntries(FiscalYear period)
        {
            period.PreparePeriodAccountBalance(organization.ChartOfAccount.GetAccountsList());
            period.ClearOpeningAccountsBalance();

            erpNodeDBContext.SaveChanges();

            if (period.PreviousFiscal == null)
                this.CopyOpeningBalanceToFirstPeriod(period);
            else
                period.CopyAccountsBalanceFromPrevius();

            period.OpeningAccountsCalculateDateTime = DateTime.Today;
            erpNodeDBContext.SaveChanges();
        }
        public void CopyOpeningBalanceToFirstPeriod(FiscalYear period)
        {
            var openingAccounts = erpNodeDBContext.Accounts
                    .Where(a => a.OpeningDebitBalance != 0 || a.OpeningCreditBalance != 0)
                    .ToList();

            openingAccounts.ForEach(account =>
            {
                period.SetOpenBalance(account, account.OpeningDebitBalance, account.OpeningCreditBalance);
            });

            erpNodeDBContext.SaveChanges();
        }
        public void CalculatePeriodAccountsBalance(FiscalYear period)
        {
            if (period.IsPostClosingEntriesLedger)
                return;

            period.PreparePeriodAccountBalance(organization.ChartOfAccount.GetAccountsList());
            period.ResetPeriodAccountsBalance();
            this.UpdatePeriodBalances(period);
            period.UpdateProfit();
            this.SaveChanges();
        }


        public void Close(FiscalYear period)
        {
            this.CalculatePeriodAccountsBalance(period);
            period.Status = EnumFiscalYearStatus.Close;

            this.erpNodeDBContext.SaveChanges();
        }
        private void UpdatePeriodBalances(FiscalYear fiscalYear)
        {
            var tempClosingBalances = erpNodeDBContext.Ledgers
                .Where(j => j.TransactionDate >= fiscalYear.StartDate && j.TransactionDate <= fiscalYear.EndDate)
                .Where(j => j.TransactionType != TransactionTypes.FiscalYearClosing && j.TransactionType != TransactionTypes.OpeningEntry)
                .GroupBy(o => o.AccountId)
                .Select(go => new TempClosingEntry
                {
                    Id = Guid.NewGuid(),
                    FiscalYearId = fiscalYear.Id,
                    AccountGuid = go.Key,
                    Account = go.FirstOrDefault().accountItem,
                    Credit = go.Select(l => l.Credit)
                      .DefaultIfEmpty(0)
                      .Sum() ?? 0,
                    Debit = go.Select(l => l.Debit)
                     .DefaultIfEmpty(0)
                     .Sum() ?? 0,
                })
                .ToList();

            foreach (var tempBalance in tempClosingBalances)
                fiscalYear.UpdatePeriodBalance(tempBalance.Account, tempBalance.Debit, tempBalance.Credit);

            fiscalYear.ClosingAccountsCalculateDateTime = DateTime.Today;


            erpNodeDBContext.SaveChanges();
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

            sqlCommand = "UPDATE [dbo].[ERP_Fiscal_Years] SET  [PostStatus] = '0'";
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);

        }
        public void UnPostLedger(FiscalYear fy)
        {
            organization.LedgersDal.RemoveTransaction(fy.Id);
            fy.PostStatus = LedgerPostStatus.ReadyToPost;

            erpNodeDBContext.SaveChanges();
        }
        public void Reopen(FiscalYear fy)
        {
            fy.Status = EnumFiscalYearStatus.Open;
            this.erpNodeDBContext.SaveChanges();
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
        public bool PostLedger(FiscalYear tr, bool SaveImmediately = true)
        {
            if (tr.PostStatus == LedgerPostStatus.Posted)
                return false;

            var trLedger = new LedgerGroup()
            {
                Id = tr.Id,
                TransactionDate = tr.EndDate,
                TransactionName = tr.Name,
                TransactionType = transactionType
            };

            tr.PeriodAccountBalances
                .Where(b => b.Account.Type == AccountTypes.Income || b.Account.Type == AccountTypes.Expense)
                .ToList().ForEach(b =>
                {
                    trLedger.AddDebit(b.Account, b.TotalCredit);
                    trLedger.AddCredit(tr.ClosingAccount, b.TotalCredit);

                    trLedger.AddCredit(b.Account, b.TotalDebit);
                    trLedger.AddDebit(tr.ClosingAccount, b.TotalDebit);
                });

            if (trLedger.FinalValidate())
            {
                erpNodeDBContext.LedgerGroups.Add(trLedger);
                tr.PostStatus = LedgerPostStatus.Posted;
            }
            else return false;


            if (SaveImmediately)
                erpNodeDBContext.SaveChanges();

            return true;
        }

    }
}