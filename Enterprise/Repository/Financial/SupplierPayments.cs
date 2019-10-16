using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.Financial;
using ERPCore.Enterprise.Models.Financial.Payments;
using ERPCore.Enterprise.Models.Financial.Payments.Enums;
using ERPCore.Enterprise.Models.Profiles;
using ERPCore.Enterprise.Models.Transactions;
using ERPCore.Enterprise.Models.Transactions.Commercials;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPCore.Enterprise.Repository.Financial
{
    public class SupplierPayments : ERPNodeDalRepository
    {
        public SupplierPayments(Organization organization) : base(organization)
        {
            transactionType = TransactionTypes.SupplierPayment;
        }
        public IQueryable<SupplierPayment> Query => erpNodeDBContext.SupplierPayments;
        public List<SupplierPayment> ListAll => erpNodeDBContext.SupplierPayments.ToList();

        public int NextNumber => (erpNodeDBContext.SupplierPayments
         .Max(e => (int?)e.No) ?? 0) + 1;


        public bool PostLedger(SupplierPayment tr, bool SaveImmediately = true)
        {
            if (tr.PostStatus == LedgerPostStatus.Posted)
                return false;


            tr.AssetAccount = tr.AssetAccount ?? organization.SystemAccounts.Cash;
            tr.LiabilityAccount = tr.LiabilityAccount ?? organization.SystemAccounts.AccountPayable;
            tr.UpdateBalance();


            var trLedger = new Models.Accounting.LedgerGroup()
            {
                Id = tr.Id,
                TransactionDate = tr.TransactionDate,
                TransactionName = tr.Name,
                TransactionNo = tr.No,
                TransactionType = transactionType,
                ProfileName = tr.Profile.DisplayName,
            };



            tr.LiabilityAccount = organization.SystemAccounts.AccountPayable;
            trLedger.AddDebit(tr.LiabilityAccount, tr.TotalCommercialAmount);
            tr.PaymentRetentions.ToList()
                .ForEach(pr => trLedger.AddCredit(pr.RetentionType.RetentionToAccount, pr.RetentionAmount));

            tr.PaymentFromAccounts.ToList()
                .ForEach(payFrom =>
                trLedger.AddCredit(payFrom.AccountItem, payFrom.PayAmount));
            if (tr.DiscountAmount > 0)
                trLedger.AddCredit(organization.SystemAccounts.DiscountTaken, tr.DiscountAmount);

            trLedger.AddCredit(tr.AssetAccount, tr.AmountBillPayFromPrimaryAcc);



            if (tr.BankFeeAmount > 0)
            {
                trLedger.AddDebit(organization.SystemAccounts.BankFee, tr.BankFeeAmount);
                trLedger.AddCredit(tr.AssetAccount, tr.BankFeeAmount);
            }



            if (trLedger.FinalValidate())
            {
                tr.PostStatus = LedgerPostStatus.Posted;
                erpNodeDBContext.LedgerGroups.Add(trLedger);
            }
            if (SaveImmediately)
                erpNodeDBContext.SaveChanges();
            return true;
        }

        public SupplierPayment CreateNew(Guid? id)
        {
            var payment = new SupplierPayment()
            {
                Id = Guid.NewGuid(),
                TransactionDate = DateTime.Today,
                LiabilityAccountId = id,
                AssetAccount = organization.SystemAccounts.Cash
            };
            erpNodeDBContext.SupplierPayments.Add(payment);

            return payment;
        }

        public void RunCheck()
        {
            var payments = erpNodeDBContext.SupplierPayments.ToList();
            payments.ForEach(p => p.UpdateBalance());

            erpNodeDBContext.SaveChanges();
        }

        public void Reorder()
        {
            var payments = erpNodeDBContext.SupplierPayments
                 .OrderBy(t => t.TransactionDate)
                 .ThenBy(t => t.No)
                 .ToList();
            erpNodeDBContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;


            int i = 1;
            foreach (var payment in payments)
            {
                payment.No = i++;
                payment.TransactionType = TransactionTypes.SupplierPayment;
                payment.UpdateBalance();
            }

            erpNodeDBContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            erpNodeDBContext.ChangeTracker.DetectChanges();
            erpNodeDBContext.SaveChanges();
        }

        public bool Delete(Guid id)
        {
            var payment = erpNodeDBContext.SupplierPayments.Find(id);

            if (payment.PostStatus != LedgerPostStatus.Posted)
            {
                payment.RemoveAllCommercial();
                erpNodeDBContext.SupplierPayments.Remove(payment);
                erpNodeDBContext.SaveChanges();
                return true;
            }

            return false;
        }
        public SupplierPayment Find(Guid id) => erpNodeDBContext.SupplierPayments.Find(id);
        public SupplierPayment Save(SupplierPayment payment)
        {
            var existPayment = erpNodeDBContext.SupplierPayments.Find(payment.Id);

            if (existPayment == null && payment.LiabilityAccountId != null)
            {
                payment.Id = Guid.NewGuid();
                payment.AssetAccount = payment.AssetAccount ?? organization.SystemAccounts.Cash;
                erpNodeDBContext.SupplierPayments.Add(payment);
                erpNodeDBContext.SaveChanges();

                return payment;
            }
            else if (existPayment != null)
            {
                if (existPayment.PostStatus == LedgerPostStatus.Posted)
                    return existPayment;

                existPayment.TransactionDate = payment.TransactionDate;
                existPayment.LiabilityAccountId = payment.LiabilityAccountId;
                existPayment.DiscountAmount = payment.DiscountAmount;
                existPayment.BankFeeAmount = payment.BankFeeAmount;
                existPayment.AssetAccountId = payment.AssetAccountId;
                existPayment.UpdateBalance();
                erpNodeDBContext.SaveChanges();
            }
            return existPayment;
        }
        public SupplierPayment Create(Profile profile, Purchase purchase, DateTime WorkingDate)
        {
            var payment = new SupplierPayment()
            {
                TransactionDate = WorkingDate,
                Profile = profile,
                AssetAccount = organization.SystemAccounts.Cash,
                TransactionType = TransactionTypes.SupplierPayment,
                No = NextNumber
            };

            erpNodeDBContext.SupplierPayments.Add(payment);

            if (purchase != null && purchase.CommercialPayment == null)
                payment.AddCommercial(purchase);

            erpNodeDBContext.SaveChanges();

            return payment;
        }
        private List<SupplierPayment> ReadyForPost => erpNodeDBContext.SupplierPayments
                                    .Where(s => s.PostStatus == LedgerPostStatus.ReadyToPost)
                                    .ToList();
        public void PostLedger()
        {
            var unPostTransactions = ReadyForPost;
            Console.WriteLine("> {0} Post {2} [{1}]", DateTime.Now.ToLongTimeString(), unPostTransactions.Count(), transactionType.ToString());
            erpNodeDBContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            unPostTransactions.ForEach(s =>
            {
                PostLedger(s, false);
            });
            erpNodeDBContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            erpNodeDBContext.ChangeTracker.DetectChanges();
            erpNodeDBContext.SaveChanges();

            //Console.WriteLine("");
        }
        public void UnPostLedger(SupplierPayment payment)
        {
            Console.WriteLine("> Un Posting,");
            organization.LedgersDal.RemoveTransaction(payment.Id);
            payment.PostStatus = LedgerPostStatus.ReadyToPost;
            erpNodeDBContext.SaveChanges();
        }

        public void UnPostAllLedger()
        {
            Console.WriteLine("> Un Post " + transactionType.ToString());

            string sqlCommand = "DELETE FROM [dbo].[ERP_Ledgers] WHERE TransactionType = {0} ";
            sqlCommand = string.Format(sqlCommand, (int)transactionType);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);

            sqlCommand = "DELETE FROM [dbo].[ERP_Ledger_Transactions] WHERE TransactionType =  {0}";
            sqlCommand = string.Format(sqlCommand, (int)transactionType);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);


            sqlCommand = "UPDATE [dbo].[ERP_Financial_General_Payments] SET  [PostStatus] = '0' WHERE Discriminator = '{0}'";
            sqlCommand = string.Format(sqlCommand, transactionType.ToString());
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);
            erpNodeDBContext.SaveChanges();
        }
    }
}
