
using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Equity;
using ERPCore.Enterprise.Models.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPCore.Enterprise.Repository.Investors
{
    public class CapitalActivities : ERPNodeDalRepository
    {
        public CapitalActivities(Organization organization) : base(organization)
        {
            transactionType = Models.Accounting.Enums.TransactionTypes.CapitalActivity;
        }


        public List<CapitalActivity> ListAll => erpNodeDBContext.CapitalActivities.ToList();

        public CapitalActivity Find(Guid transactionId) => erpNodeDBContext.CapitalActivities.Find(transactionId);

        public CapitalActivity CreateNew(Guid investorProfileId)
        {
            var newInvestment = new CapitalActivity()
            {
                Id = Guid.NewGuid(),
                InvestorId = investorProfileId,
                TransactionDate = DateTime.Today
            };

            erpNodeDBContext.CapitalActivities.Add(newInvestment);
            erpNodeDBContext.SaveChanges();
            return newInvestment;
        }

        public int NextNumber => (erpNodeDBContext.CapitalActivities.Max(e => (int?)e.No) ?? 0) + 1;




        private List<CapitalActivity> ReadyForPost => erpNodeDBContext.CapitalActivities
              .Where(s => s.PostStatus == LedgerPostStatus.ReadyToPost)
              .ToList()
              .Where(t => t.TransactionDate >= organization.DataItems.FirstDate)
              .ToList();



        public void PostLedger()
        {
            var unPostTransactions = this.ReadyForPost;

            string logTitle = string.Format("> Post {0} [{1}]", this.trString, unPostTransactions.Count());
            organization.EventLogs.NewEventLog(EventLogLevel.Information,"00", logTitle, null, "");

        

            unPostTransactions.ForEach(s =>
            {
                this.PostLedger(s, true);
            });

            erpNodeDBContext.SaveChanges();
        }

        public void ReOrder()
        {
            var transactions = erpNodeDBContext.CapitalActivities
                .OrderBy(t => t.TransactionDate)
                .OrderBy(t => t.No)
                .ToList();

            int i = 1;

            foreach (var transaction in transactions)
            {
                transaction.No = i++;
            }

            erpNodeDBContext.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var capitalActivity = organization.CapitalActivities.Find(id);
            erpNodeDBContext.CapitalActivities.Remove(capitalActivity);
            organization.SaveChanges();
        }

        public CapitalActivity Save(CapitalActivity capitalInvestment)
        {
            var existCapitalActivity = erpNodeDBContext.CapitalActivities.Find(capitalInvestment.Id);

            if (existCapitalActivity == null)
            {
                existCapitalActivity = capitalInvestment;
                existCapitalActivity.FiscalYear = organization.FiscalYears.Find(existCapitalActivity.TransactionDate);
                existCapitalActivity.Id = Guid.NewGuid();
                existCapitalActivity.No = this.NextNumber;
                existCapitalActivity.AssetAccountGuid = existCapitalActivity.AssetAccountGuid ?? organization.SystemAccounts.Cash.Id;
                existCapitalActivity.EquityAccountGuid = existCapitalActivity.EquityAccountGuid ?? organization.SystemAccounts.EquityStock.Id;
                existCapitalActivity.CaluculateTotal();
                erpNodeDBContext.CapitalActivities.Add(existCapitalActivity);
            }
            else
            {
                if (existCapitalActivity.PostStatus != LedgerPostStatus.Posted)
                {
                    existCapitalActivity.FiscalYear = organization.FiscalYears.Find(existCapitalActivity.TransactionDate);
                    existCapitalActivity.TransactionDate = capitalInvestment.TransactionDate;
                    existCapitalActivity.Type = capitalInvestment.Type;
                    existCapitalActivity.AssetAccountGuid = capitalInvestment.AssetAccountGuid ?? organization.SystemAccounts.Cash.Id;
                    existCapitalActivity.EquityAccountGuid = capitalInvestment.EquityAccountGuid ?? organization.SystemAccounts.EquityStock.Id;
                    existCapitalActivity.StockAmount = capitalInvestment.StockAmount;
                    existCapitalActivity.EachStockParValue = capitalInvestment.EachStockParValue;
                    existCapitalActivity.InvestorId = capitalInvestment.InvestorId;
                    existCapitalActivity.CaluculateTotal();
                }
            }




            erpNodeDBContext.SaveChanges();

            return existCapitalActivity;
        }


        public void UnPostLedger(CapitalActivity capitalInvestment)
        {
            Console.WriteLine("> Un Posting " + capitalInvestment.No);
            organization.LedgersDal.RemoveTransaction(capitalInvestment.Id);
            capitalInvestment.PostStatus = LedgerPostStatus.ReadyToPost;

            erpNodeDBContext.SaveChanges();
        }


        public void UnPostAllLedger()
        {
            Console.WriteLine("> Un Post" + Models.Accounting.Enums.TransactionTypes.CapitalActivity.ToString());

            string sqlCommand = "DELETE FROM [dbo].[ERP_Ledgers] WHERE TransactionType =  {0}";
            sqlCommand = string.Format(sqlCommand, (int)this.transactionType);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);

            sqlCommand = "DELETE FROM [dbo].[ERP_Ledger_Transactions] WHERE TransactionType =  {0}";
            sqlCommand = string.Format(sqlCommand, (int)this.transactionType);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);


            var trs = erpNodeDBContext.CapitalActivities.ToList();
            trs.ForEach(s =>
            {
                s.PostStatus = LedgerPostStatus.ReadyToPost;
            });
            erpNodeDBContext.SaveChanges();
        }

        public void PostLedger(CapitalActivity tr, bool SaveImmediately = true)
        {
            if (tr.PostStatus == LedgerPostStatus.Posted)
                return;

            var trLedger = new Models.Accounting.LedgerGroup()
            {
                Id = tr.Id,
                TransactionDate = tr.TransactionDate,
                TransactionName = tr.Name,
                TransactionNo = tr.No ?? 0,
                TransactionType = transactionType
            };
            erpNodeDBContext.LedgerGroups.Add(trLedger);


            if (tr.Type == Models.Equity.Enums.CapitalActivityType.Invest)
            {
                trLedger.AddDebit(tr.AssetAccount, tr.TotalStockParValue);
                trLedger.AddCredit(tr.EquityAccount, tr.TotalStockParValue);
            }
            else
            {
                trLedger.AddCredit(tr.AssetAccount, Math.Abs(tr.TotalStockParValue));
                trLedger.AddDebit(tr.EquityAccount, Math.Abs(tr.TotalStockParValue));
            }

            tr.PostStatus = LedgerPostStatus.Posted;

            if (SaveImmediately)
                erpNodeDBContext.SaveChanges();
        }


    }
}