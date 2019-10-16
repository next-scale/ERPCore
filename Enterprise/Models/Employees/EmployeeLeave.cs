using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPCore.Enterprise.Models.Employees
{

    public enum EmployeeLeaveType
    {
        FullDay = 0,
        HalfDay = 1,
        Last = 2,
        Other = 10
    }

    [Table("ERP_Employee_Leaves")]
    public class EmployeeLeave
    {
        [Key]
        public Guid Id { get; set; }

        public int? No { get; set; }

        public string Name => string.Format("{0}/{1}", this.TransactionDate.ToString("yyMM"), this.No?.ToString().PadLeft(3, '0'));

        public EmployeeLeaveType Type { get; set; }

        public DateTime TransactionDate { get; set; }

        public int AmountDay { get; set; }
        public int AmountHour { get; set; }


        [Column("EmployeeId")]
        public Guid EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employees.Employee Employee { get; set; }
    }


}