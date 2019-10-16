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

namespace ERPCore.Enterprise.Repository.Employees
{
    public class EmployeePaymentLines : ERPNodeDalRepository
    {
        public EmployeePaymentLines(Organization organization) : base(organization)
        {

        }

        public Models.Employees.EmployeePaymentItem Find(Guid id) => erpNodeDBContext.EmployeePaymentLines.Find(id);
        public IQueryable<EmployeePaymentItem> Query => erpNodeDBContext.EmployeePaymentLines;

        public void Remove(EmployeePaymentItem exEmployeePaymentItem)
        {
            erpNodeDBContext.EmployeePaymentLines.Remove(exEmployeePaymentItem);
            erpNodeDBContext.SaveChanges();
        }
    }
}