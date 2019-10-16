using ERPCore.Enterprise.Models.Accounting;
using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Taxes.Enums;
using ERPCore.Enterprise.Models.Transactions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Models.Employees
{


    [Table("ERP_Employee_Payments")]
    public class EmployeePayment
    {
        [Key]
        public Guid Id { get; set; }

        public static TransactionTypes TransactionType = TransactionTypes.EmployeePayment;
        public int? No { get; set; }

        public string Name =>
            string.Format("{0}/{1}/{2}", DocumentCode, DocumentGroup, No.ToString().PadLeft(2, '0'));
        public string DocumentGroup => TransactionDate.ToString("yyMM");
        public string DocumentCode => TransactionHelper.TrCode(TransactionType);

        public EmployeePaymentStatus Status { get; set; }

        public DateTime TransactionDate { get; set; }

        public Guid? FiscalYearId { get; set; }
        [ForeignKey("FiscalYearId")]
        public virtual FiscalYear FiscalYear { get; set; }


        public Guid? EmployeePaymentPeriodId { get; set; }
        [ForeignKey("EmployeePaymentPeriodId")]
        public virtual EmployeePaymentPeriod EmployeePaymentPeriod { get; set; }





        [Column("EmployeeId")]
        public Guid EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employees.Employee Employee { get; set; }

        public virtual ICollection<EmployeePaymentItem> PaymentItems { get; set; }

        public Decimal TotalEarning
        {
            get
            {
                try
                {
                    return PaymentItems.Where(pi => pi.PaymentType.PayDirection == PayDirection.Eanring)
                        .Sum(pi => pi.EarningAmount) ?? 0;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

    

        public Decimal TotalDeduction
        {
            get
            {
                try
                {
                    return PaymentItems.Where(pi => pi.PaymentType.PayDirection == PayDirection.Deduction).Sum(pi => pi.DeductionAmount) ?? 0;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public Decimal TotalPayment
        {
            get
            {
                return TotalEarning - TotalDeduction;
            }
        }

        public Guid? PayFromAccountGuid { get; set; }
        [ForeignKey("PayFromAccountGuid")]
        public virtual ChartOfAccount.Account PayFromAccount { get; set; }

        public EmployeePaymentItem addPaymentItems(Guid PaymentTypeGuid, decimal amount)
        {
            var employeePaymentItem = new EmployeePaymentItem()
            {
                Id = Guid.NewGuid(),
                PaymentTypeGuid = PaymentTypeGuid,
                Amount = Math.Abs(amount)
            };

            PaymentItems.Add(employeePaymentItem);

            return employeePaymentItem;
        }

        public LedgerPostStatus PostStatus { get; set; }
    }
}
