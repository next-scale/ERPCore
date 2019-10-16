using ERPCore.Enterprise.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ERPCore.Enterprise.Models.Transactions.Commercials;
using ERPCore.Enterprise.Models.Financial;
using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.Financial.Payments;
using ERPCore.Enterprise.Models.Financial.Payments.Enums;

namespace ERPCore.Enterprise.Repository.Financial
{
    public class ReceivePayments : ERPNodeDalRepository
    {

        public ReceivePayments(Organization organization) : base(organization)
        {
            transactionType = TransactionTypes.ReceivePayment;
        }

        public IQueryable<ReceivePayment> Query => erpNodeDBContext.ReceivePayments;
        public List<ReceivePayment> ListAll => erpNodeDBContext.ReceivePayments.ToList();

        public int NextNumber => (erpNodeDBContext.ReceivePayments
         .Max(e => (int?)e.No) ?? 0) + 1;

        public void Reorder()
        {
            var payments = this.Query
                    .OrderBy(t => t.TransactionDate)
                    .ThenBy(t => t.No)
                    .ToList();


            this.erpNodeDBContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;


            int i = 1;

            foreach (var transfer in payments)
            {
                transfer.No = i++;
                transfer.TransactionType = TransactionTypes.ReceivePayment;
                transfer.UpdateBalance();
            }

            this.erpNodeDBContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            this.erpNodeDBContext.ChangeTracker.DetectChanges();
            this.erpNodeDBContext.SaveChanges();


            this.SaveChanges();
        }

        public ReceivePayment Create(Models.Profiles.Profile profile, Commercial com, DateTime workingDate)
        {
            var payment = new ReceivePayment()
            {
                TransactionDate = workingDate,
                Profile = profile,
                AssetAccount = organization.SystemAccounts.Cash,
                TransactionType = TransactionTypes.ReceivePayment,
                No = this.NextNumber
            };
            erpNodeDBContext.ReceivePayments.Add(payment);

            if (com != null && com.CommercialPayment == null)
            {
                payment.AddCommercial(com);
            }



            erpNodeDBContext.SaveChanges();


            return payment;
        }
        public ReceivePayment Save(ReceivePayment payment)
        {
            var existPayment = erpNodeDBContext.ReceivePayments.Find(payment.Id);

            if (existPayment == null)
                return payment;

            else if (existPayment != null)
            {
                if (existPayment.PostStatus == LedgerPostStatus.Posted)
                    return existPayment;

                existPayment.TransactionType = this.transactionType;
                existPayment.CompanyProfile = organization.SelfProfile;
                existPayment.TransactionDate = payment.TransactionDate;
                existPayment.ClearingDelayDayCount = payment.ClearingDelayDayCount;
                existPayment.DiscountAmount = payment.DiscountAmount;
                existPayment.BankFeeAmount = payment.BankFeeAmount;
                existPayment.AssetAccountId = payment.AssetAccountId;

                erpNodeDBContext.SaveChanges();

                return existPayment;
            }


            return null;
        }

        public bool Delete(Guid id)
        {

            var payment = erpNodeDBContext.ReceivePayments.Find(id);

            if (payment.PostStatus == LedgerPostStatus.Posted)
                return false;

            payment.RemoveAllCommercial();
            erpNodeDBContext.ReceivePayments.Remove(payment);

            erpNodeDBContext.SaveChanges();

            return true;
        }

        public ReceivePayment Find(Guid id) => erpNodeDBContext.ReceivePayments.Find(id);

        public List<ReceivePayment> FindList(Guid id) => erpNodeDBContext.ReceivePayments
                                    .Where(s => s.Id == id)
                                    .ToList();
        private List<ReceivePayment> ReadyForPost => erpNodeDBContext.ReceivePayments
                                    .Where(s => s.PostStatus == LedgerPostStatus.ReadyToPost)
                                    .ToList();



        public void PostLedger()
        {
            var unPostTransactions = this.ReadyForPost;
            Console.WriteLine("> {0} Post {2} [{1}]", DateTime.Now.ToLongTimeString(), unPostTransactions.Count(), this.transactionType.ToString());
            erpNodeDBContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            unPostTransactions.ForEach(s => this.PostLedger(s, false));
            erpNodeDBContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            erpNodeDBContext.ChangeTracker.DetectChanges();
            erpNodeDBContext.SaveChanges();
        }
        public bool PostLedger(ReceivePayment tr, bool SaveImmediately = true)
        {

            if (tr.PostStatus == LedgerPostStatus.Posted)
                throw new Exception("Posted Transaction");
            if (tr.CommercialCount == 0 || tr.TotalCommercialAmount == 0)
                throw new Exception("Zero commecial transaction");

            tr.AssetAccount = tr.AssetAccount ?? organization.SystemAccounts.Cash;
            tr.ReceivableAccount = organization.SystemAccounts.AccountReceivable;
            tr.UpdateBalance();

            var trLedger = new Models.Accounting.LedgerGroup()
            {
                Id = tr.Id,
                TransactionDate = tr.TransactionDate,
                TransactionName = tr.Name,
                TransactionNo = tr.No,
                TransactionType = transactionType,
                ProfileName = tr.Profile?.Name,
            };

            string memo = "";
            //Cr.
            trLedger.AddCredit(tr.ReceivableAccount, tr.TotalCommercialAmount, memo);

            //Dr.
            tr.PaymentRetentions.ToList().ForEach(pr =>
                trLedger.AddDebit(pr.RetentionType.RetentionToAccount, pr.RetentionAmount, memo));
            trLedger.AddDebit(organization.SystemAccounts.DiscountGiven, tr.DiscountAmount, memo);
            trLedger.AddDebit(tr.AssetAccount, tr.AmountPaymentReceive, memo);


            if (tr.BankFeeAmount > 0)
            {
                trLedger.AddDebit(organization.SystemAccounts.BankFee, tr.BankFeeAmount);
                trLedger.AddCredit(tr.AssetAccount, tr.BankFeeAmount);
            }

            if (trLedger.FinalValidate())
            {
                erpNodeDBContext.LedgerGroups.Add(trLedger);
                tr.PostStatus = LedgerPostStatus.Posted;
            }
            else if (tr.PostStatus != LedgerPostStatus.Posted)
            {
                return false;
            }

            return true;


        }
        public void UnPostLedger(ReceivePayment financialPayment)
        {
            Console.WriteLine("> Un Posting,");
            organization.LedgersDal.RemoveTransaction(financialPayment.Id);
            financialPayment.PostStatus = LedgerPostStatus.ReadyToPost;
            erpNodeDBContext.SaveChanges();
        }

        public void UnPostAllLedger()
        {
            organization.LedgersDal.UnPostAllLedgers(this.transactionType);

            string sqlCommand = "UPDATE [dbo].[ERP_Financial_General_Payments] SET  [PostStatus] = '0' WHERE Discriminator = '{0}'";
            sqlCommand = string.Format(sqlCommand, transactionType.ToString());
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);
            erpNodeDBContext.SaveChanges();
        }

    }
}
