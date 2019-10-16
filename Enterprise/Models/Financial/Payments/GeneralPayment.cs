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
    [Table("ERP_Financial_General_Payments")]
    public class GeneralPayment
    {
        [Key]
        public Guid Id { get; set; }
        public Enums.PaymentMethod PaymentMethod { get; set; }
        public Enums.PaymentStatus Status { get; set; }
        public TransactionTypes TransactionType { get; set; }
        public int No { get; set; }
        public String Memo { get; set; }
        public DateTime TransactionDate { get; set; }
        public int ClearingDelayDayCount { get; set; }
        public DateTime? ClearingDate { get; set; }

        public DateTime ClearingDelay => TransactionDate.AddDays(ClearingDelayDayCount);


        public string Name =>
        string.Format("{0}/{1}/{2}", DocumentCode, DocumentGroup, No.ToString().PadLeft(2, '0'));

        public string DocumentGroup => this.TransactionDate.ToString("yyMM");
        public string DocumentCode => TransactionHelper.TrCode(this.TransactionType);

        public virtual ICollection<PaymentFromAccount> PaymentFromAccounts { get; set; }
        public Decimal AmountPayFrom { get; set; }
        public decimal AmountCommercial { get; private set; }
        public virtual ICollection<PaymentRetention> PaymentRetentions { get; set; }
        public Decimal AmountRetention { get; set; }




        public virtual ICollection<Commercial> Commercials { get; set; }
        public virtual int CommercialCount => this.Commercials?.Count() ?? 0;
        public virtual decimal TotalCommercialAmount => this.Commercials?.Sum(c => c.Total) ?? 0;

        public Guid? ProfileGuid { get; set; }
        [ForeignKey("ProfileGuid")]
        public virtual Profiles.Profile Profile { get; set; }
        public string ProfileName { get; set; }
        public Guid? CompanyProfileGuid { get; set; }
        [ForeignKey("CompanyProfileGuid")]
        public virtual Profiles.Profile CompanyProfile { get; set; }


        [Column("TotalPayment")]
        public Decimal Amount { get; set; }



        public String ThaiTotalCommercialAmount => new ERPKeeper.Helpers.Currency.Thai.Baht().Process(this.TotalCommercialAmount.ToString("N2"));


        public Decimal DiscountAmount { get; set; }
        public Decimal BankFeeAmount { get; set; }




        public Decimal OverPaymentAmount { get; set; }

        public Guid? AssetAccountId { get; set; }
        [ForeignKey("AssetAccountId")]
        public virtual Account AssetAccount { get; set; }


        public Guid? OptionalAssetAccountId { get; set; }



        public Guid? LiabilityAccountId { get; set; }
        [ForeignKey("LiabilityAccountId")]
        public virtual Account LiabilityAccount { get; set; }

        public Guid? ReceivableAccountId { get; set; }
        [ForeignKey("ReceivableAccountId")]
        public virtual Account ReceivableAccount { get; set; }

        /// <summary>
        /// Retention Section
        /// </summary>
        public Guid? RetentionTypeGuid { get; set; }
        [ForeignKey("RetentionTypeGuid")]
        public virtual RetentionType RetentionType { get; set; }
        public LedgerPostStatus PostStatus { get; set; }




        public GeneralPayment()
        {
            Id = Guid.NewGuid();
        }

        public void UpdateBalance()
        {
            if (this.PostStatus == LedgerPostStatus.Posted)
            {
                Console.WriteLine("Transaction Posted");
                // throw new Exception("Transaction Posted");
            }


            this.ProfileName = this.Profile?.Name ?? this.ProfileName;
            this.AmountRetention = this.PaymentRetentions?.Sum(c => c.RetentionAmount) ?? 0;
            this.AmountPayFrom = this.PaymentFromAccounts?.Sum(c => c.PayAmount) ?? 0;
            this.AmountCommercial = this.Commercials?.Sum(c => c.Total) ?? 0;
        }

        public bool AddCommercial(Commercial commercial)
        {
            if (this.PostStatus == LedgerPostStatus.Posted)
                throw new Exception("Transaction Posted");

            if (commercial.CommercialPaymentId != null)
                return false;

            if (this.Commercials == null)
                this.Commercials = new HashSet<Commercial>();

            this.Commercials.Add(commercial);

            commercial.CommercialPaymentId = this.Id;
            commercial.CommercialPayment = this;
            commercial.UpdatePayment();
            this.UpdateBalance();
            return true;
        }

        public void RemovePayFromAccount(Guid payFromAccountId)
        {
            if (this.PostStatus == LedgerPostStatus.Posted)
                throw new Exception("Transaction Posted");

            var payFromAccount = this.PaymentFromAccounts
                .Where(pf => pf.Id == payFromAccountId)
                .First();

            this.PaymentFromAccounts.Remove(payFromAccount);
            this.UpdateBalance();
        }

        public void RemoveCommercial(Commercial commercial)
        {
            if (commercial == null)
                return;

            this.Commercials.Remove(commercial);
            commercial.CommercialPayment = null;
            commercial.CommercialPaymentId = null;
            commercial.UpdatePayment();

            this.UpdateBalance();
        }
        public void RemoveAllCommercial()
        {

            var removeCommercials = this.Commercials.ToList();
            removeCommercials.ForEach(c =>
            {
                this.Commercials.Remove(c);
                c.CommercialPayment = null;
                c.CommercialPaymentId = null;
                c.UpdatePayment();
            });

            this.UpdateBalance();
        }

        public bool AddPayFrom(Account account, decimal amount = 0, bool isDefault = false)
        {
            if (account == null)
                throw new Exception("Account is Empty");

            if (this.PaymentFromAccounts == null)
                this.PaymentFromAccounts = new HashSet<PaymentFromAccount>();

            var paymentFromAccountLine = this.PaymentFromAccounts
                .Where(p => p.AccountItemId == account.Id)
                .FirstOrDefault();

            if (paymentFromAccountLine != null)
                throw new Exception("Account aleardy exist");

            paymentFromAccountLine = new PaymentFromAccount()
            {
                Id = Guid.NewGuid(),
                AccountItem = account,
                AccountItemId = account.Id,
                PayAmount = amount,
                IsDefault = isDefault
            };
            this.PaymentFromAccounts.Add(paymentFromAccountLine);

            this.UpdateBalance();

            return true;
        }
        public bool AddPaymentRetention(RetentionType retentionType, decimal? amount = null)
        {
            if (retentionType == null)
                return false;

            if (this.PaymentRetentions == null)
                this.PaymentRetentions = new HashSet<PaymentRetention>();

            var paymentRetention = this.PaymentRetentions
                .Where(p => p.RetentionTypeId == retentionType.Id)
                .FirstOrDefault();

            if (paymentRetention != null)
                throw new Exception("Account aleardy exist");

            paymentRetention = new PaymentRetention()
            {
                Id = Guid.NewGuid(),
                RetentionType = retentionType,
                RetentionTypeId = retentionType.Id,
                RetentionAmount = amount ?? (this.TotalCommercialAmount * (retentionType.Rate ?? 0) / 100),
            };
            this.PaymentRetentions.Add(paymentRetention);
            this.UpdateBalance();

            return true;
        }
        public void RemovePaymentRetention(PaymentRetention paymentRetention)
        {
            if (paymentRetention == null)
                return;

            this.PaymentRetentions.Remove(paymentRetention);

            this.UpdateBalance();
        }
    }
}




