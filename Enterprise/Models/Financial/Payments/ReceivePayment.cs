using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Transactions.Commercials;

namespace ERPCore.Enterprise.Models.Financial.Payments
{
    public class ReceivePayment : GeneralPayment
    {
        public ReceivePayment()
        {
            Id = Guid.NewGuid();
        }

        public decimal AmountPaymentReceive =>
            this.TotalCommercialAmount - (this.AmountRetention + this.DiscountAmount);
    }
}




