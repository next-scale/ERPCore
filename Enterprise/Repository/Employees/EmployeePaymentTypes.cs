
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
    public class EmployeePaymentTypes : ERPNodeDalRepository
    {
        public EmployeePaymentTypes(Organization organization) : base(organization)
        {

        }


        public List<EmployeePaymentType> ListAll => erpNodeDBContext.EmployeePaymentTypes.ToList();



        public EmployeePaymentType Find(Guid id) => erpNodeDBContext.EmployeePaymentTypes.Find(id);

        public EmployeePaymentType CreateNew(EmployeePaymentType paymentType)
        {
            paymentType.PaymentTypeGuid = Guid.NewGuid();
            erpNodeDBContext.EmployeePaymentTypes.Add(paymentType);

            return paymentType;
        }
    }
}