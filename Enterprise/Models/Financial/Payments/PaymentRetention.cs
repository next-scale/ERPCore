using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Financial.Payments;
using ERPCore.Enterprise.Models.Transactions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace ERPCore.Enterprise.Models.Financial.Payments
{
    [Table("ERP_Financial_General_Payments_PaymentRetention")]
    public class PaymentRetention
    {
        [Key]
        public Guid Id { get; set; }
        public decimal PayAmount { get; set; }
        public decimal RetentionAmount { get; set; }

        public Guid? RetentionTypeId { get; set; }
        [ForeignKey("RetentionTypeId")]
        public virtual RetentionType RetentionType { get; set; }

        public Guid? GeneralPaymentId { get; set; }
        [ForeignKey("GeneralPaymentId")]
        public virtual GeneralPayment GeneralPayment { get; set; }

    }
}
