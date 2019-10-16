
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
    public class EmployeePaymentTemplates : ERPNodeDalRepository
    {
        public EmployeePaymentTemplates(Organization organization) : base(organization)
        {

        }


        public List<EmployeePaymentTemplate> ListAll => erpNodeDBContext.EmployeePaymentTemplates.ToList();


        public EmployeePaymentType Find(Guid id) => erpNodeDBContext.EmployeePaymentTypes.Find(id);

        public void CreateNew(EmployeePaymentTemplate template)
        {
            template.Id = Guid.NewGuid();
            erpNodeDBContext.EmployeePaymentTemplates.Add(template);
        }
    }
}