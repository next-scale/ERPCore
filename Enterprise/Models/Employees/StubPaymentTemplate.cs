using ERPCore.Enterprise.Models.ChartOfAccount;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Models.Employees
{
    [Table("ERP_Employee_PaymentTemplates")]
    public class EmployeePaymentTemplate
    {
        [Key]
        public Guid Id { get; set; }
        public String Name { get; set; }

        public virtual ICollection<EmployeePaymentTemplateItem> Items { get; set; }
    }
}
