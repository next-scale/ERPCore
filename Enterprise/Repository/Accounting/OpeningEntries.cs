
using ERPCore.Enterprise.Models.ChartOfAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Repository.Company;
using ERPCore.Enterprise.DataBase;
using ERPCore.Enterprise.Models.Accounting;
using ERPCore.Enterprise.Models.Accounting.FiscalYears;
using ERPCore.Enterprise.Models.Accounting.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERPCore.Enterprise.Repository.Accounting
{
    public class OpeningEntries : ERPNodeDalRepository
    {

        public OpeningEntries(Organization organization) : base(organization)
        {
            transactionType = TransactionTypes.OpeningEntry;
        }

        public List<Account> ReadyForPost => organization.ChartOfAccount
            .GetAccountsList()
            .Where(s => s.PostStatus == LedgerPostStatus.ReadyToPost)
            .ToList();

        public bool PostLedger()
        {
            Console.WriteLine("> {0} Post {2} [{1}]", DateTime.Now.ToLongTimeString(), this.ReadyForPost.Count(), this.transactionType.ToString());

            this.UnPostLedger();

            var trLedger = new LedgerGroup()
            {
                Id = Guid.NewGuid(),
                TransactionDate = organization.DataItems.FirstDate,
                TransactionName = "Open Entry",
                TransactionNo = 0,
                TransactionType = transactionType
            };
            erpNodeDBContext.LedgerGroups.Add(trLedger);


            this.ReadyForPost.ForEach(a =>
            {
                trLedger.AddDebit(a, a.OpeningDebitBalance);
                trLedger.AddCredit(a, a.OpeningCreditBalance);
                a.PostStatus = LedgerPostStatus.Posted;
            });

            var result = trLedger.FinalValidate();

            if (result == false)
                this.erpNodeDBContext.LedgerGroups.Remove(trLedger);

            this.erpNodeDBContext.SaveChanges();
            return true;
        }



        public void UnPostLedger()
        {
            Console.WriteLine("> Un Post" + this.transactionType.ToString());

            string sqlCommand = "DELETE FROM [dbo].[ERP_Ledgers] WHERE TransactionType = {0} ";
            sqlCommand = string.Format(sqlCommand, (int)this.transactionType);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);

            sqlCommand = "DELETE FROM [dbo].[ERP_Ledger_Transactions] WHERE TransactionType =  {0}";
            sqlCommand = string.Format(sqlCommand, (int)this.transactionType);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);


            erpNodeDBContext.Accounts.ToList()
             .ForEach(s => s.PostStatus = LedgerPostStatus.ReadyToPost);

            erpNodeDBContext.SaveChanges();
        }


    }
}