using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPCore.Enterprise.Models.Employees
{
    [Table("ERP_Employees")]
    public class Employee
    {
        [Key, ForeignKey("Profile")]
        public Guid ProfileId { get; set; }

        public virtual Profiles.Profile Profile { get; set; }

        public int EmployeeNumber { get; set; }
        public EmployeeStatus Status { get; set; }
        public DateTime? OnBoardDate { get; set; }
        public double OnBoardDaysCount =>
            (DateTime.Today - (DateTime)(OnBoardDate ?? DateTime.Today)).TotalDays;
        public Decimal CurrentSalary { get; set; }
        public String Memo { get; set; }
        public Guid? PositionGuid { get; set; }
        [ForeignKey("PositionGuid")]
        public virtual EmployeePosition Position { get; set; }

        public Decimal TotalIncome { get; set; }
        public virtual ICollection<EmployeePayment> EmployeePayments { get; set; }
        public virtual ICollection<EmployeeLeave> EmployeeLeaves { get; set; }

        public void CalculateTotalIncome()
        {
            TotalIncome = EmployeePayments.ToList().Sum(ep => ep.TotalEarning);
        }

        public void SetStatus(EmployeeStatus newStatus)
        {
            this.Status = newStatus;
            this.Memo = string.Format("{0}{1}{2}", this.Memo, "> Change status to " + newStatus.ToString(), Environment.NewLine);
        }

        public void SetActive()
        {
            this.Status = EmployeeStatus.Active;
        }

        public void CreateLeave(DateTime date, EmployeeLeaveType type)
        {
            var newLeave = new EmployeeLeave()
            {
                Id = Guid.NewGuid(),
                TransactionDate = date,
                Type = type,
            };


            this.EmployeeLeaves.Add(newLeave);
        }
    }
}
