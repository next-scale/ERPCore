using ERPCore.Enterprise.Repository.Transactions;
using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Employees;
using ERPCore.Enterprise.Models.Transactions;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Web;
using Microsoft.EntityFrameworkCore;

namespace ERPCore.Enterprise.Repository.Employees
{
    public class EmployeePayments : ERPNodeDalRepository
    {
        public EmployeePayments(Organization organization) : base(organization)
        {
            transactionType = TransactionTypes.EmployeePayment;
        }

        public Models.Employees.EmployeePayment Find(Guid id) => erpNodeDBContext.EmployeePayments.Find(id);

        public IQueryable<EmployeePayment> Query => erpNodeDBContext.EmployeePayments;

        public EmployeePayment Save(EmployeePayment employeePayment)
        {
            var existPayment = erpNodeDBContext.EmployeePayments.Find(employeePayment.Id);

            existPayment.FiscalYear = organization.FiscalYears.Find(existPayment.TransactionDate);

            existPayment.EmployeePaymentPeriodId = employeePayment.EmployeePaymentPeriodId;
            existPayment.TransactionDate = employeePayment.TransactionDate;
            existPayment.PayFromAccountGuid = employeePayment.PayFromAccountGuid ?? organization.SystemAccounts.Cash.Id;
            SaveChanges();

            return existPayment;
        }

        private int NextNumber => (erpNodeDBContext.EmployeePayments.Max(e => (int?)e.No) ?? 0) + 1;

        public List<EmployeePayment> ReadyForPost => erpNodeDBContext.EmployeePayments
                    .Where(s => s.PostStatus == LedgerPostStatus.ReadyToPost)
                    .Where(s => s.Status != EmployeePaymentStatus.Void)
                    .ToList()
                      .Where(t => t.TransactionDate >= organization.DataItems.FirstDate)
                      .ToList();


        public void AutoAssignPeriod()
        {
            var payments = Query.Where(p => p.EmployeePaymentPeriodId == null).ToList();
            payments.ForEach(p =>
            {
                var paymentPeriod = organization.EmployeePaymentPeriods.FindByDate(p.TransactionDate.Date, true);
                p.EmployeePaymentPeriodId = paymentPeriod.Id;
            });


            organization.SaveChanges();
        }

        public void ReAssignPeriod()
        {
            var payments = Query.ToList();

            payments.ForEach(p =>
            {
                var paymentPeriod = organization.EmployeePaymentPeriods.FindByDate(p.TransactionDate.Date, true);
                p.EmployeePaymentPeriodId = paymentPeriod.Id;
            });


            organization.SaveChanges();
        }

        public void AutoAssignPeriod(Guid id)
        {
            var payment = Find(id);
            var paymentPeriod = organization.EmployeePaymentPeriods.FindByDate(payment.TransactionDate.Date, true);

            payment.EmployeePaymentPeriodId = paymentPeriod.Id;
            organization.SaveChanges();
        }

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
            erpNodeDBContext.SaveChanges();
        }


        public bool PostLedger(EmployeePayment tr, bool SaveImmediately = true)
        {
            if (tr.PostStatus == LedgerPostStatus.Posted)
                return false;

            var trLedger = new Models.Accounting.LedgerGroup()
            {
                Id = tr.Id,
                TransactionDate = tr.TransactionDate,
                TransactionName = tr.Name,
                TransactionNo = tr.No ?? 0,
                TransactionType = transactionType
            };

            if (tr.PayFromAccount == null)
                tr.PayFromAccount = organization.SystemAccounts.Cash;

            trLedger.AddCredit(tr.PayFromAccount, tr.TotalPayment);
            PostEanringToGL(trLedger, tr);
            PostDeductionToGL(trLedger, tr);

            if (trLedger.FinalValidate())
            {
                tr.PostStatus = LedgerPostStatus.Posted;
                erpNodeDBContext.LedgerGroups.Add(trLedger);
            }

            if (SaveImmediately)
                erpNodeDBContext.SaveChanges();

            return true;
        }

        public bool Delete(Guid id)
        {
            var payment = organization.EmployeePayments.Find(id);

            if (payment.PostStatus != LedgerPostStatus.Posted)
            {
                erpNodeDBContext.EmployeePayments.Remove((EmployeePayment)payment);
                organization.SaveChanges();
                return true;
            }
            return false;
        }


        public EmployeePayment Copy(EmployeePayment originalEmployeePayment, DateTime trDate)
        {
            var cloneEmployeePayment = erpNodeDBContext.EmployeePayments
                    .AsNoTracking()
                    .Include(p => p.PaymentItems)
                    .FirstOrDefault(x => x.Id == originalEmployeePayment.Id);

            cloneEmployeePayment.Id = Guid.NewGuid();
            cloneEmployeePayment.TransactionDate = trDate;
            cloneEmployeePayment.No = organization.EmployeePayments.NextNumber;
            cloneEmployeePayment.PostStatus = LedgerPostStatus.ReadyToPost;
            cloneEmployeePayment.PaymentItems.ToList().ForEach(ci => ci.Id = Guid.NewGuid());
            cloneEmployeePayment.EmployeePaymentPeriodId = null;

            erpNodeDBContext.EmployeePayments.Add(cloneEmployeePayment);
            erpNodeDBContext.SaveChanges();

            return cloneEmployeePayment;
        }





        private void PostEanringToGL(Models.Accounting.LedgerGroup trLedger, EmployeePayment employeePayment)
        {
            foreach (var paymentItem in employeePayment.PaymentItems.Where(t => t.PaymentType.PayDirection == Models.Employees.PayDirection.Eanring).ToList())
            {
                trLedger.AddDebit(paymentItem.PaymentType.Account, paymentItem.Amount);
            }
        }

        public EmployeePayment CreateNew(Guid id)
        {
            var newEmployeePayment = new EmployeePayment()
            {
                Id = Guid.NewGuid(),
                EmployeeId = id,
                TransactionDate = DateTime.Today
            };

            erpNodeDBContext.EmployeePayments.Add(newEmployeePayment);
            organization.SaveChanges();

            return newEmployeePayment;
        }

        private void PostDeductionToGL(Models.Accounting.LedgerGroup trLedger, EmployeePayment employeePayment)
        {
            var paymentDeductionItems = employeePayment.PaymentItems
                .Where(t => t.PaymentType.PayDirection == Models.Employees.PayDirection.Deduction)
                .ToList();

            foreach (var paymentItem in paymentDeductionItems)
            {
                trLedger.AddCredit(paymentItem.PaymentType.Account, paymentItem.Amount);
            }
        }


        public void UnPostLedger(EmployeePayment employeePayment)
        {
            organization.LedgersDal.RemoveTransaction(employeePayment.Id);
            employeePayment.PostStatus = LedgerPostStatus.ReadyToPost;
            organization.SaveChanges();

        }
        public void UnPostAllLedger()
        {
            Console.WriteLine("> Un Post " + transactionType.ToString());

            string sqlCommand = "DELETE FROM [dbo].[ERP_Ledgers] WHERE TransactionType = {0} ";
            sqlCommand = string.Format(sqlCommand, (int)transactionType);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);

            sqlCommand = "DELETE FROM [dbo].[ERP_Ledger_Transactions] WHERE TransactionType =  {0}";
            sqlCommand = string.Format(sqlCommand, (int)transactionType);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);

            erpNodeDBContext.EmployeePayments.ToList().ForEach(b => b.PostStatus = LedgerPostStatus.ReadyToPost);
            erpNodeDBContext.SaveChanges();
        }


    }
}