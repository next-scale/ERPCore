
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Financial.Loans;
using ERPCore.Enterprise.Models.Financial.Transfer;
using ERPCore.Enterprise.Models.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Models.Accounting.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERPCore.Enterprise.Repository.Financial
{
    public class Loans : ERPNodeDalRepository
    {

        public Loans(Organization organization) : base(organization)
        {
            transactionType = TransactionTypes.Loan;
        }


        public List<Loan> ListAll => erpNodeDBContext.Loans.ToList();
        public List<Loan> ReadyForPost => erpNodeDBContext.Loans.Where(s => s.PostStatus == LedgerPostStatus.ReadyToPost).ToList();
        public Loan Find(Guid id) => erpNodeDBContext.Loans.Find(id);
        private int NextNumber => (erpNodeDBContext.Loans.Max(e => (int?)e.No) ?? 0) + 1;



        public void Add(Loan entity) => erpNodeDBContext.Loans.Add(entity);

        public void ReOrder()
        {
            var loans = erpNodeDBContext.Loans
                .OrderBy(t => t.TransactionDate)
                .ToList();

            int i = 1;
            foreach (var loan in loans)
            {
                loan.No = i;
                i++;

            }

            erpNodeDBContext.SaveChanges();
        }



        public void Save(Loan loan)
        {
            var existLoan = erpNodeDBContext.Loans.Find(loan.Id);

            if (existLoan == null)
            {
                loan.FiscalYear = organization.FiscalYears.Find(loan.TransactionDate);
                loan.TransactionType = Models.Accounting.Enums.TransactionTypes.Loan;
                erpNodeDBContext.Loans.Add(loan);
            }
            else
            {
                if (existLoan.PostStatus == LedgerPostStatus.Posted)
                {
                    existLoan.FiscalYear = organization.FiscalYears.Find(existLoan.TransactionDate);
                    existLoan.TransactionDate = loan.TransactionDate;
                    existLoan.AssetAccountGuid = loan.AssetAccountGuid;
                    existLoan.LiabilityAccountGuid = loan.LiabilityAccountGuid;
                    existLoan.Amount = loan.Amount;
                }
            }

            erpNodeDBContext.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var tr = erpNodeDBContext.Loans.Find(id);
            erpNodeDBContext.Loans.Remove(tr);
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

            sqlCommand = "UPDATE [dbo].[ERP_Finance_Loans] SET  [PostStatus] = '0'";
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);
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


        public void UnPostLedger(Loan loan)
        {
            organization.LedgersDal.RemoveTransaction(loan.Id);
            loan.PostStatus = LedgerPostStatus.ReadyToPost;
        }
        public bool PostLedger(Loan tr, bool SaveImmediately = true)
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


            trLedger.AddDebit(tr.AssetAccount, tr.Amount);
            trLedger.AddCredit(tr.LiabilityAccount, tr.Amount);

            if (trLedger.FinalValidate())
            {
                erpNodeDBContext.LedgerGroups.Add(trLedger);
                tr.PostStatus = LedgerPostStatus.Posted;
            }

            if (SaveImmediately)
                erpNodeDBContext.SaveChanges();
            return true;
        }

    }
}