
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
    public class EmployeePositions : ERPNodeDalRepository
    {
        public EmployeePositions(Organization organization) : base(organization)
        {

        }

        public List<EmployeePosition> ListAll => erpNodeDBContext.EmployeePositions.ToList();



        public EmployeePosition Find(Guid id) => erpNodeDBContext.EmployeePositions.Find(id);

        public EmployeePosition CreateNew(EmployeePosition position)
        {
            position.Id = Guid.NewGuid();
            erpNodeDBContext.EmployeePositions.Add(position);
            return position;
        }
    }
}