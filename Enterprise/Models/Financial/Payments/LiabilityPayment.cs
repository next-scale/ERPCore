using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Transactions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ERPCore.Enterprise.Models.Financial.Payments
{
    public class LiabilityPayment : GeneralPayment
    {
        public void Update(LiabilityPayment debtPayment)
        {
            if (this.PostStatus == LedgerPostStatus.Posted)
                throw new Exception("Aleardy posted");

            this.Amount = debtPayment.Amount;
            this.BankFeeAmount = debtPayment.BankFeeAmount;
            this.AssetAccountId = debtPayment.AssetAccountId;
            this.TransactionDate = debtPayment.TransactionDate;

            debtPayment.UpdateBalance();

        }

        public decimal AmountLiabilityPayment => (this.Amount + this.BankFeeAmount) - (this.AmountRetention + this.DiscountAmount);
        public decimal AmountLiabilityPayFromPrimaryAcc => this.AmountLiabilityPayment - this.AmountPayFrom;

    }

}




