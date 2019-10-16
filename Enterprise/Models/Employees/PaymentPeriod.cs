using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace ERPCore.Enterprise.Models.Employees
{

    [Table("ERP_Employee_Payment_Periods")]
    public class EmployeePaymentPeriod
    {
        [Key]
        public Guid Id { get; set; }
        public String Name { get; set; }
        public DateTime TransactionDate { get; set; }

        public string TrGroup => this.TransactionDate.ToString("yyMM");
        public virtual ICollection<EmployeePayment> EmployeePayments { get; set; }

       

        public virtual decimal TotalEarning => EmployeePayments?.Sum(e => e.TotalEarning) ?? 0;
        public virtual decimal TotalDeduction => EmployeePayments?.Sum(e => e.TotalDeduction) ?? 0;
        public virtual decimal TotalPayment => TotalEarning - TotalDeduction;

        public virtual int PaymentCount => EmployeePayments?.Count() ?? 0;
        public String Description { get; set; }


        public void Update(EmployeePaymentPeriod paymentType)
        {
            this.Name = paymentType.Name;
            this.Description = paymentType.Description;
            this.TransactionDate = paymentType.TransactionDate;
        }
    }
}
