
using ERPCore.Enterprise.Models.Employees;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPCore.Enterprise.Repository.Profiles
{
    public class Employees : ERPNodeDalRepository
    {
        public Employees(Organization organization) : base(organization)
        {

        }

        public IQueryable<Employee> GetAll => erpNodeDBContext.Employees;

        public Employee Create(Models.Profiles.Profile newEmployeeProfile)
        {
            if (newEmployeeProfile.Employee == null)
            {
                newEmployeeProfile.Employee = new Employee()
                {
                    Status = EmployeeStatus.Active,
                    OnBoardDate = DateTime.Today,
                };
            }
            erpNodeDBContext.SaveChanges();
            return newEmployeeProfile.Employee;
        }

        public Employee Find(Guid id) => erpNodeDBContext.Employees.Find(id);

        public void Delete(Guid id)
        {
            var employee = organization.Employees.Find(id);
            erpNodeDBContext.Employees.Remove(employee);
            organization.SaveChanges();
        }
    }
}