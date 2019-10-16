using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace ERPCore.Enterprise.Models.Employees
{

    [Table("ERP_Employee_Payment_Types")]
    public class EmployeePaymentType
    {
        [Key, Column("Id")]
        public Guid PaymentTypeGuid { get; set; }


        public String Name { get; set; }

        public String Description { get; set; }

        public bool IsActive { get; set; }

        public PayDirection PayDirection { get; set; }

        public Guid? AccountGuid { get; set; }
        [ForeignKey("AccountGuid")]
        public virtual Models.ChartOfAccount.Account Account { get; set; }

        public Guid? RetentionTypeGuid { get; set; }
        [ForeignKey("RetentionTypeGuid")]
        public virtual Models.Financial.Payments.RetentionType RetentionType { get; set; }

        public void Update(EmployeePaymentType paymentType)
        {
            this.Name = paymentType.Name;
            this.PayDirection = paymentType.PayDirection;
            this.Description = paymentType.Description;
            this.AccountGuid = paymentType.AccountGuid;
        }
    }
}
