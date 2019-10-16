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
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ERPCore.Enterprise.Repository.Financial
{
    public class LiabilityPayments : ERPNodeDalRepository
    {
        public LiabilityPayments(Organization organization) : base(organization)
        {
            transactionType = TransactionTypes.LiabilityPayment;
        }

        public LiabilityPayment Find(Guid id) => erpNodeDBContext.LiabilityPayments.Find(id);
        private int NextNumber => (erpNodeDBContext.LiabilityPayments.Max(e => (int?)e.No) ?? 0) + 1;
        public bool PostLedger(LiabilityPayment tr, bool SaveImmediately = true)
        {
            if (tr.PostStatus == LedgerPostStatus.Posted)
                return false;

            if (tr.LiabilityAccount == null)
                return false;


            var trLedger = new Models.Accounting.LedgerGroup()
            {
                Id = tr.Id,
                TransactionDate = tr.TransactionDate,
                TransactionName = tr.Name,
                TransactionNo = tr.No,
                TransactionType = transactionType
            };
            //Dr.
            trLedger.AddDebit(tr.LiabilityAccount, tr.Amount);

            //Cr.
            tr.PaymentRetentions.ToList().ForEach(pr =>
                trLedger.AddCredit(pr.RetentionType.RetentionToAccount, pr.RetentionAmount));
            tr.PaymentFromAccounts.ToList().ForEach(payFrom =>
                trLedger.AddCredit(payFrom.AccountItem, payFrom.PayAmount));
            trLedger.AddCredit(tr.AssetAccount, tr.AmountLiabilityPayFromPrimaryAcc);

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
            else
                return false;

            if (SaveImmediately)
                erpNodeDBContext.SaveChanges();
            return true;
        }

        public LiabilityPayment Copy(LiabilityPayment originalLiabilityPayment, DateTime trDate)
        {
            var cloneLiabilityPayment = this.erpNodeDBContext.LiabilityPayments
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == originalLiabilityPayment.Id);

            cloneLiabilityPayment.Id = Guid.NewGuid();
            cloneLiabilityPayment.TransactionDate = trDate;
            cloneLiabilityPayment.No = organization.LiabilityPayments.NextNumber;
            cloneLiabilityPayment.PostStatus = LedgerPostStatus.ReadyToPost;

            this.erpNodeDBContext.LiabilityPayments.Add(cloneLiabilityPayment);
            this.erpNodeDBContext.SaveChanges();

            return cloneLiabilityPayment;
        }


        public LiabilityPayment CreateNew(Guid liabilityAccountId, decimal liabilityAmount = 0)
        {
            var LiabilityPayment = new LiabilityPayment()
            {
                Id = Guid.NewGuid(),
                LiabilityAccountId = liabilityAccountId,
                TransactionDate = DateTime.Today,
                Amount = liabilityAmount,
                AssetAccount = organization.SystemAccounts.Cash,
                TransactionType = TransactionTypes.LiabilityPayment,
            };


            erpNodeDBContext.LiabilityPayments.Add(LiabilityPayment);
            erpNodeDBContext.SaveChanges();

            return LiabilityPayment;
        }

        public bool Remove(LiabilityPayment payment)
        {
            if (payment.PostStatus != LedgerPostStatus.Posted)
            {
                erpNodeDBContext.LiabilityPayments.Remove(payment);
                erpNodeDBContext.SaveChanges();
                return true;
            }
            else
            {
                throw new Exception("Posted Transaction");
            }
        }

        public void ReOrder()
        {
            int i = 1;
            erpNodeDBContext.LiabilityPayments
                .OrderBy(l => l.TransactionDate).ThenBy(l => l.No)
                .ToList().ForEach(l =>
            {
                l.No = i++;
            });
            erpNodeDBContext.SaveChanges();
        }

        public LiabilityPayment Save(LiabilityPayment payment)
        {
            var existPayment = erpNodeDBContext.LiabilityPayments.Find(payment.Id);

            if (existPayment == null && payment.LiabilityAccountId != null)
            {
                existPayment = organization.LiabilityPayments.CreateNew((Guid)payment.LiabilityAccountId, payment.Amount);
                erpNodeDBContext.SaveChanges();
                return payment;
            }


            else if (existPayment != null)
            {
                if (existPayment.PostStatus == LedgerPostStatus.Posted)
                    return existPayment;

                existPayment.TransactionDate = payment.TransactionDate;
                existPayment.LiabilityAccountId = payment.LiabilityAccountId;
                existPayment.Amount = payment.Amount;
                existPayment.BankFeeAmount = 0;

                erpNodeDBContext.SaveChanges();

                return existPayment;
            }
            return null;
        }

        public LiabilityPayment Create(Profile profile)
        {
            var payment = new LiabilityPayment()
            {
                TransactionDate = DateTime.Today,
            };

            erpNodeDBContext.LiabilityPayments.Add(payment);

            return payment;
        }

        private List<LiabilityPayment> ReadyForPost => erpNodeDBContext.LiabilityPayments
                                    .Where(s => s.PostStatus == LedgerPostStatus.ReadyToPost)
                                    .ToList();

        public IQueryable<LiabilityPayment> Query => erpNodeDBContext.LiabilityPayments;

        public List<LiabilityPayment> ListAll => this.Query.ToList();

        public void PostLedger()
        {
            var unPostTransactions = this.ReadyForPost;
            Console.WriteLine("> {0} Post {2} [{1}]", DateTime.Now.ToLongTimeString(), unPostTransactions.Count(), this.transactionType.ToString());

            erpNodeDBContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            unPostTransactions.ForEach(s =>
            {
                this.PostLedger(s, false);
            });
            erpNodeDBContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            erpNodeDBContext.ChangeTracker.DetectChanges();
            erpNodeDBContext.SaveChanges();
        }

        public void UnPostLedger(LiabilityPayment LiabilityPayment)
        {
            Console.WriteLine("> Un Posting,");
            organization.LedgersDal.RemoveTransaction(LiabilityPayment.Id);
            LiabilityPayment.PostStatus = LedgerPostStatus.ReadyToPost;
            erpNodeDBContext.SaveChanges();
        }

        public void UnPostAllLedger()
        {
            Console.WriteLine("> Un Post " + transactionType.ToString());

            string sqlCommand = "DELETE FROM [dbo].[ERP_Ledgers] WHERE TransactionType = {0} ";
            sqlCommand = string.Format(sqlCommand, (int)this.transactionType);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);

            sqlCommand = "DELETE FROM [dbo].[ERP_Ledger_Transactions] WHERE TransactionType =  {0}";
            sqlCommand = string.Format(sqlCommand, (int)this.transactionType);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);


            sqlCommand = "UPDATE [dbo].[ERP_Financial_General_Payments] SET  [PostStatus] = '0' WHERE Discriminator = '{0}'";
            sqlCommand = string.Format(sqlCommand, transactionType.ToString());
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);
            erpNodeDBContext.SaveChanges();
        }

    }
}
