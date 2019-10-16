
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Financial;
using ERPCore.Enterprise.Models.Financial.Transfer;
using ERPCore.Enterprise.Models.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Models.Employees;

namespace ERPCore.Enterprise.Repository.Employees
{
    public class EmployeePaymentPeriods : ERPNodeDalRepository
    {
        public EmployeePaymentPeriods(Organization organization) : base(organization)
        {

        }

        public List<EmployeePaymentPeriod> ListAll => erpNodeDBContext.EmployeePaymentPeriods.ToList();
        public EmployeePaymentPeriod Find(Guid id) => erpNodeDBContext.EmployeePaymentPeriods.Find(id);

        public EmployeePaymentPeriod CreateNew(DateTime date)
        {
            var newPeriod = new EmployeePaymentPeriod()
            {
                Id = Guid.NewGuid(),
                Name = date.ToString("yyMM"),
                TransactionDate = date.Date
            };
            erpNodeDBContext.EmployeePaymentPeriods.Add(newPeriod);
            erpNodeDBContext.SaveChanges();

            return newPeriod;
        }

        public void AutoAssignPayment(Guid id)
        {
            var paymentPeriod = this.Find(id);
            var payments = organization.EmployeePayments.Query
                .Where(p => p.EmployeePaymentPeriodId == null)
                .ToList()
                .Where(p => p.TransactionDate.Date == paymentPeriod.TransactionDate.Date)
                .ToList();

            payments.ForEach(p =>
            {
                p.EmployeePaymentPeriodId = id;
            });


            organization.SaveChanges();
        }

        internal EmployeePaymentPeriod FindByDate(DateTime date, bool createIfNotFound)
        {
            var period = this.erpNodeDBContext.EmployeePaymentPeriods
                .Where(p => p.TransactionDate == date)
                .FirstOrDefault();

            if (period != null)
                return period;
            else if (createIfNotFound)
                return this.CreateNew(date);
            else return null;
        }
    }
}