using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using ERPCore.Enterprise.Models.Financial.Payments.Enums;

namespace ERPCore.Enterprise.Models.Financial.Payments
{
    public class SupplierPayment : GeneralPayment
    {
        public SupplierPayment()
        {
            Id = Guid.NewGuid();
        }

        public decimal TotalBillPaymentAmount => (this.TotalCommercialAmount) - (this.AmountRetention + this.DiscountAmount);

        public decimal AmountBillPayFromPrimaryAcc => this.TotalBillPaymentAmount  - this.AmountPayFrom;
    }
}




