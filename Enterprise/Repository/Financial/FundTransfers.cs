
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Financial.Transfer;
using ERPCore.Enterprise.Models.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Models.Accounting.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERPCore.Enterprise.Repository.Financial
{
    public class FundTransfers : ERPNodeDalRepository
    {
        public FundTransfers(Organization organization) : base(organization)
        {
            transactionType = Models.Accounting.Enums.TransactionTypes.FundTransfer;
        }

        public List<FundTransfer> ReadyForPost => erpNodeDBContext.FundTransfers
                    .Where(s => s.PostStatus == LedgerPostStatus.ReadyToPost)
                    .ToList();

        public List<FundTransfer> GetAll => erpNodeDBContext.FundTransfers.ToList();
        public IQueryable<FundTransfer> Query => erpNodeDBContext.FundTransfers;
        private int NextNumber => (erpNodeDBContext.FundTransfers.Max(e => (int?)e.No) ?? 0) + 1;
        public void ReOrder()
        {
            var transfers = erpNodeDBContext.FundTransfers
                .OrderBy(t => t.TransactionDate).ThenBy(t => t.No)
                .ToList();

            int i = 0;
            foreach (var transfer in transfers)
            {
                transfer.No = ++i;
            }

            erpNodeDBContext.SaveChanges();

        }
        public void Add(FundTransfer entity)
        {
            erpNodeDBContext.FundTransfers.Add(entity);
        }
        public void Delete(FundTransfer transfer)
        {
            if (transfer.PostStatus == LedgerPostStatus.Posted)
            {
                this.UnPostLedger(transfer);
                erpNodeDBContext.FundTransfers.Remove(transfer);
                erpNodeDBContext.SaveChanges();
            }
        }
        public FundTransfer Find(Guid transactionId)
        {
            return erpNodeDBContext.FundTransfers.Find(transactionId);
        }
        public void Save(FundTransfer transfer)
        {
            var existTransfer = erpNodeDBContext.FundTransfers.Find(transfer.Id);

            if (existTransfer == null)
            {
                transfer.FiscalYear = organization.FiscalYears.Find(transfer.TransactionDate);
                transfer.TransactionType = Models.Accounting.Enums.TransactionTypes.FundTransfer;
                transfer.No = NextNumber;
                transfer.BankFeeAccountGuid = organization.SystemAccounts.BankFee.Id;

                erpNodeDBContext.FundTransfers.Add(transfer);
            }
            else
            {
                if (existTransfer.PostStatus == LedgerPostStatus.Posted)
                    return;

                transfer.FiscalYear = organization.FiscalYears.Find(transfer.TransactionDate);
                existTransfer.Reference = transfer.Reference;
                existTransfer.TransactionDate = transfer.TransactionDate;
                existTransfer.DepositAccountGuid = transfer.DepositAccountGuid;
                existTransfer.WithDrawAccountGuid = transfer.WithDrawAccountGuid;
                existTransfer.AmountwithDraw = transfer.AmountwithDraw;
                existTransfer.AmountFee = transfer.AmountFee;
                existTransfer.Reference = transfer.Reference;
            }



            erpNodeDBContext.SaveChanges();
        }
        public bool Delete(Guid id)
        {
            var Transfer = erpNodeDBContext.FundTransfers.Find(id);

            if (Transfer.PostStatus != LedgerPostStatus.Posted)
            {
                erpNodeDBContext.FundTransfers.Remove(Transfer);
                erpNodeDBContext.SaveChanges();
                return true;
            }
            return false;
        }

        public FundTransfer Copy(FundTransfer originalFundTransfer, DateTime trDate)
        {
            var cloneFundTransfer = this.erpNodeDBContext.FundTransfers
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == originalFundTransfer.Id);

            cloneFundTransfer.Id = Guid.NewGuid();
            cloneFundTransfer.TransactionDate = trDate;
            cloneFundTransfer.Reference = "Clone-" + cloneFundTransfer.Reference;
            cloneFundTransfer.No = organization.FundTransfers.NextNumber;
            cloneFundTransfer.PostStatus = LedgerPostStatus.ReadyToPost;

            this.erpNodeDBContext.FundTransfers.Add(cloneFundTransfer);
            this.erpNodeDBContext.SaveChanges();

            return cloneFundTransfer;
        }




        public bool PostLedger(FundTransfer tr, bool SaveImmediately = true)
        {

            if (tr.PostStatus == LedgerPostStatus.Posted)
                return false;

            var trLedger = new Models.Accounting.LedgerGroup()
            {
                Id = tr.Id,
                TransactionDate = tr.TransactionDate,
                TransactionName = tr.Name,
                TransactionNo = tr.No,
                TransactionType = transactionType
            };

            trLedger.AddCredit(tr.WithDrawAccount, tr.AmountwithDraw);
            if (tr.AmountFee > 0)
                trLedger.AddDebit(tr.BankFeeAccount, tr.AmountFee);
            trLedger.AddDebit(tr.DepositAccount, tr.AmountDeposit);




            if (trLedger.FinalValidate())
            {
                erpNodeDBContext.LedgerGroups.Add(trLedger);
                tr.PostStatus = LedgerPostStatus.Posted;
            }
            else
            {
                return false;
            }

            if (SaveImmediately)
                erpNodeDBContext.SaveChanges();

            return true;
        }
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
        public void UnPostLedger(FundTransfer transfer)
        {
            Console.WriteLine("> UnPost GL," + transfer.TransactionType + " " + transfer.No);
            organization.LedgersDal.RemoveTransaction(transfer.Id);

            transfer.PostStatus = LedgerPostStatus.ReadyToPost;
            erpNodeDBContext.SaveChanges();
        }
        public void UnPostAllLedger()
        {
            Console.WriteLine("> Un Post" + this.transactionType.ToString());

            string sqlCommand = "DELETE FROM [dbo].[ERP_Ledgers] WHERE TransactionType = {0} ";
            sqlCommand = string.Format(sqlCommand, (int)this.transactionType);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);

            sqlCommand = "DELETE FROM [dbo].[ERP_Ledger_Transactions] WHERE TransactionType =  {0}";
            sqlCommand = string.Format(sqlCommand, (int)this.transactionType);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);

            erpNodeDBContext.FundTransfers.ToList().ForEach(s => s.PostStatus = LedgerPostStatus.ReadyToPost);
            erpNodeDBContext.SaveChanges();
        }
    }
}