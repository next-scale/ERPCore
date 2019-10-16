
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Financial;
using ERPCore.Enterprise.Models.Financial.Transfer;
using ERPCore.Enterprise.Models.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Models.Employees;
using ERPCore.Enterprise.Models.Transactions.Commercials;
using ERPCore.Enterprise.Models.Departments;

namespace ERPCore.Enterprise.Repository.Company
{
    public class Departments : ERPNodeDalRepository
    {
        public Departments(Organization organization) : base(organization)
        {

        }


        public List<Department> ListAll => erpNodeDBContext.Departments.ToList();
        public Department Find(Guid id) => erpNodeDBContext.Departments.Find(id);
        public IQueryable<Department> Query => erpNodeDBContext.Departments;

        public void CreateNew(Department department)
        {
            department.DepartmentGuid = Guid.NewGuid();
            erpNodeDBContext.Departments.Add(department);
        }

        public void Remove(Guid id)
        {
            var department = erpNodeDBContext.Departments.Find(id);
            erpNodeDBContext.Departments.Remove(department);
            organization.SaveChanges();
        }
    }
}