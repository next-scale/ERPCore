
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ERPCore.Enterprise.Models.Transactions.Commercials;
using ERPCore.Enterprise.Models.Transactions;
using ERPCore.Enterprise.Models.Accounting.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using ERPCore.Enterprise.Models.Logging;

namespace ERPCore.Enterprise.Repository.Transactions
{
    public class QueryDateRange
    {
        public QueryDateRange(DateTime starTime, DateTime endDate)
        {
            StarTime = starTime;
            EndDate = endDate;
        }

        public DateTime StarTime { get; private set; }
        public DateTime EndDate { get; private set; }
    }

    public class Sales : Commercials
    {
        public Sales(Organization organization) : base(organization)
        {
            transactionType = TransactionTypes.Sale;
        }

        public new IQueryable<Sale> Query => erpNodeDBContext.Sales;
        public new Sale Find(Guid transactionId) => erpNodeDBContext.Sales.Find(transactionId);
        public IQueryable<Sale> GetByStatus(CommercialStatus status)
             => this.Query
                    .Where(t => t.Status == status)
                    .Include(t => t.Profile);

        public IQueryable<Sale> GetByViewStatus(CommercialViewStatus status)
        {
            switch (status)
            {
                case CommercialViewStatus.Open:
                    return this.Query.Where(t => t.Status == CommercialStatus.Open).Include(t => t.Profile);
                case CommercialViewStatus.Paid:
                    return this.Query.Where(t => t.Status == CommercialStatus.Paid).Include(t => t.Profile);
                case CommercialViewStatus.OverDue:
                    return this.GetOverDueList;
                case CommercialViewStatus.LastFiscal:
                    return this.LastFiscal;
                case CommercialViewStatus.All:
                default:
                    return this.Query;
            }

        }

        public IQueryable<Sale> LastFiscal
        {
            get
            {
                var currentFiscal = organization.FiscalYears.CurrentPeriod;

                var burchaseBeforeDate = currentFiscal.EndDate;
                var salesList = erpNodeDBContext.Sales
                    .Where(b => b.TransactionDate < currentFiscal.StartDate)
                    .Where(b => b.CloseDate >= currentFiscal.StartDate);
                return salesList;
            }
        }

        public IQueryable<Sale> GetOverDueList
        {
            get
            {
                var expDate = DateTime.Today.AddDays(-30);
                var overdueList = erpNodeDBContext.Sales
                    .Where(b => b.Status == CommercialStatus.Open)
                    .Where(b => b.TransactionDate < expDate);
                return overdueList;
            }
        }

        public new IQueryable<Sale> QueryOpen => erpNodeDBContext.Sales
            .Where(t => t.Status == CommercialStatus.Open);

        public IQueryable<Sale> QuerySales(QueryDateRange dateRange)
        {
            return this.Query
                .Where(t => t.TransactionDate >= dateRange.StarTime &&
                            t.TransactionDate <= dateRange.EndDate);
        }


        public List<CommercialDailyBalance> ListDailyBalances(int duration)
        {
            DateTime StartDate = DateTime.Now.AddDays(duration * -1);

            var dailyBalances = this.erpNodeDBContext.CommercialDailyBalances
                 .Where(db => db.TransactionDate > StartDate && db.Type == this.transactionType)
                 .ToList();

            var firstDateBalance = dailyBalances.Where(b => b.TransactionDate == StartDate).FirstOrDefault();

            if (firstDateBalance == null)
            {
                firstDateBalance = new CommercialDailyBalance()
                {
                    TransactionDate = StartDate,
                    Balance = 0,
                    Type = this.transactionType,
                    Id = Guid.NewGuid()
                };

                dailyBalances.Add(firstDateBalance);
            }

            return dailyBalances;
        }

        public new Sale Find(int transactionNo) => erpNodeDBContext.Sales.Where(t => t.No == transactionNo).FirstOrDefault();

        public int OpenCount => erpNodeDBContext.Sales
    .Where(b => b.Status == CommercialStatus.Open)
    .Count();
        public int OverDueCount
        {
            get
            {
                var expDate = DateTime.Today.AddDays(-30);
                var overdueCount = erpNodeDBContext.Sales
                    .Where(b => b.Status == CommercialStatus.Open)
                    .Where(b => b.TransactionDate < expDate)
                    .Count();
                return overdueCount;
            }
        }
        public Decimal OpenBalance => erpNodeDBContext.Sales
            .Where(b => b.Status == CommercialStatus.Open)
            .Sum(b => (Decimal?)b.Total) ?? 0;
        public Decimal OverDueBalance
        {
            get
            {
                var expDate = DateTime.Today.AddDays(-30);
                var overdueBalance = erpNodeDBContext.Sales
                    .Where(b => b.Status == CommercialStatus.Open)
                    .Where(b => b.TransactionDate < expDate)
                    .Sum(b => (Decimal?)b.Total) ?? 0;
                return overdueBalance;
            }
        }


        public List<Sale> ReadyForPost => erpNodeDBContext.Sales
                    .Where(s => s.PostStatus == LedgerPostStatus.ReadyToPost)
                    .Include(s => s.CommercialItems).ToList();
        public List<Sale> GetReceiveAble(DateTime viewDate)
        {
            var openSales = erpNodeDBContext.Sales
                .Where(i => i.TransactionDate <= viewDate)
                .ToList();

            return openSales.Where(i => i.CloseDate == null || i.CloseDate > viewDate).ToList();

        }

        private new int NextNumber => (erpNodeDBContext.Sales.Max(e => (int?)e.No) ?? 0) + 1;

        [NotMapped]
        public IQueryable<Sale> PaidTransactions => erpNodeDBContext.Sales.Where(s => s.Status == CommercialStatus.Paid);

        public Sale Create(Models.Profiles.Profile profile, DateTime trDate)
        {
            var newSale = new Sale()
            {
                No = this.NextNumber,
                ProfileGuid = profile.Id,
                Profile = profile,
                TransactionDate = trDate,
                CompanyProfile = organization.SelfProfile,
                Status = CommercialStatus.Open,
                CommercialItems = new HashSet<CommercialItem>(),
                TransactionType = this.transactionType,
            };
            newSale.AddCommercialTax(organization.TaxCodes.GetDefaultOuput);

            erpNodeDBContext.Sales.Add(newSale);
            erpNodeDBContext.SaveChanges();

            return newSale;
        }

        public List<Commercial> FindList(Guid id) => this.erpNodeDBContext.Commercials.Where(tr => tr.Id == id).Take(1).ToList();

        public Sale Update(Sale sale)
        {
            var existSale = erpNodeDBContext.Sales.Find(sale.Id);

            if (existSale == null)
                throw new Exception("Update fail, transaction not found");
            else if (existSale.PostStatus == LedgerPostStatus.Posted)
                throw new Exception("Update fail, transaction aleardy posted");
            else
            {
                existSale.Reference = sale.Reference;
                existSale.Memo = sale.Memo;
                existSale.ProjectGuid = sale.ProjectGuid;
                existSale.TransactionDate = sale.TransactionDate;
                existSale.PaymentTermGuid = sale.PaymentTermGuid;
                existSale.ShippingTermGuid = sale.ShippingTermGuid;
                existSale.CompanyProfile = erpNodeDBContext.Profiles.Where(p => p.isSelfOrganization).FirstOrDefault();
                existSale.CompanyProfileAddress = existSale.CompanyProfile.PrimaryAddress;
                erpNodeDBContext.SaveChanges();

                existSale.ReCalculate();
                erpNodeDBContext.SaveChanges();
                return sale;
            }
        }
        public List<Sale> ListByProfile(Guid id) => organization.Sales.Query
                .Where(Transaction => Transaction.ProfileGuid == id)
                .ToList();


        public void UpdateDailyBalance()
        {
            this.ClearDailyBalance();
            var dailySales = this.Query.GroupBy(t => new { t.TransactionDate })
                                           .Select(go => new
                                           {
                                               IncomeAmount = go.Sum(i => i.Total),
                                               TransactionDate = go.Key.TransactionDate
                                           }).ToList();

            Console.WriteLine("> {0} Update Daily Sales Balance {2} [{1}]", DateTime.Now.ToLongTimeString(), dailySales.Count(), this.transactionType.ToString());

            erpNodeDBContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            dailySales.ForEach(ds =>
            {
                var dailySale = new CommercialDailyBalance()
                {
                    TransactionDate = ds.TransactionDate,
                    Balance = ds.IncomeAmount,
                    Type = this.transactionType,
                    Id = Guid.NewGuid()
                };
                this.erpNodeDBContext.CommercialDailyBalances.Add(dailySale);
            });

            erpNodeDBContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            erpNodeDBContext.ChangeTracker.DetectChanges();
            erpNodeDBContext.SaveChanges();
        }
        public void ClearDailyBalance()
        {
            var removeList = this.erpNodeDBContext.CommercialDailyBalances.Where(t => t.Type == this.transactionType).ToList();
            this.erpNodeDBContext.CommercialDailyBalances.RemoveRange(removeList);
            this.SaveChanges();
        }

        public void PostLedger()
        {
            var unPostTransactions = this.ReadyForPost;

            string logTitle = string.Format("> Post {0} [{1}]", this.trString, unPostTransactions.Count());
            organization.EventLogs.NewEventLog(EventLogLevel.Information, "00", logTitle, null, "");

            erpNodeDBContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            using (var progress = new Helpers.ProgressBar())
            {
                var currentIndex = 0;
                unPostTransactions.ForEach(s =>
                {
                    progress.Report(currentIndex++, unPostTransactions.Count);
                    this.PostLedger(s, false);
                });
            }

            erpNodeDBContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            erpNodeDBContext.ChangeTracker.DetectChanges();
            erpNodeDBContext.SaveChanges();
        }
        public bool PostLedger(Guid id)
        {
            var sale = this.Find(id);
            return this.PostLedger(sale, true);
        }
        public bool PostLedger(Sale sale, bool saveChange = true)
        {
            if (sale.PostStatus == LedgerPostStatus.Posted)
                throw new Exception("Post fail, Transaction aleardy posted");
            if (sale.Total == 0)
                return false;

            Console.WriteLine(sale.Profile.Name);


            var trLedger = new Models.Accounting.LedgerGroup()
            {
                Id = sale.Id,
                TransactionDate = sale.TransactionDate,
                TransactionName = sale.Name,
                ProfileName = sale.Profile.Name,
                TransactionNo = sale.No,
                TransactionType = transactionType,
                Reference = sale.Reference,
            };

            this.PostLedger_Items(trLedger, sale);
            this.PostLedger_Tax(trLedger, sale);

            trLedger.AddDebit(organization.SystemAccounts.AccountReceivable, sale.Total);

            if (trLedger.FinalValidate())
            {
                sale.PostStatus = LedgerPostStatus.Posted;
                erpNodeDBContext.LedgerGroups.Add(trLedger);
            }
            else
            {
                trLedger.RemoveAllLedgerLines();
                organization.EventLogs.NewEventLog(EventLogLevel.Error,
                    "1011", "Error Posting", trLedger.TransactionName, "");
                return false;
            }

            if (saveChange)
                erpNodeDBContext.SaveChanges();

            return true;
        }
        private void PostLedger_Items(Models.Accounting.LedgerGroup trLedger, Sale sale)
        {
            foreach (var transactionItem in sale.CommercialItems)
            {
                string memo = transactionItem.Amount.ToString() + " x " + transactionItem.ItemPartNumber;
                trLedger.AddCredit(transactionItem.Item.IncomeAccount, transactionItem.LineTotal, memo);
            }
        }
        public bool Delete(Sale sale)
        {
            if (sale == null)
                throw new Exception("Delete fail, Transaction not found");
            else if (sale.PostStatus == LedgerPostStatus.Posted)
                throw new Exception("Delete fail, Transaaction aleardy posted");
            else
            {
                sale.RemoveItems();
                sale.RemoveCommercialTaxs();
                erpNodeDBContext.Commercials.Remove(sale);
                organization.SaveChanges();
                return true;
            }
        }
        public Sale Copy(Sale originalSale, DateTime trDate)
        {
            var cloneSale = this.erpNodeDBContext.Sales
                    .AsNoTracking()
                    .Include(p => p.CommercialItems)
                    .FirstOrDefault(x => x.Id == originalSale.Id);

            cloneSale.Id = Guid.NewGuid();
            cloneSale.TransactionDate = trDate;
            cloneSale.Reference = "Clone-" + cloneSale.Reference;
            cloneSale.No = organization.Sales.NextNumber;
            cloneSale.PostStatus = LedgerPostStatus.ReadyToPost;

            cloneSale.RemovePayment();
            cloneSale.CommercialItems.ToList().ForEach(ci => ci.Id = Guid.NewGuid());


            this.erpNodeDBContext.Sales.Add(cloneSale);
            this.erpNodeDBContext.SaveChanges();

            return cloneSale;
        }
        private bool PostLedger_Tax(Models.Accounting.LedgerGroup trLedger, Sale sale)
        {

            sale.CommercialTaxes
                 .ToList()
                 .ForEach(commercialTax =>
                 {
                     trLedger.AddCredit(commercialTax.TaxCode.TaxAccount, commercialTax.TaxBalance);
                 });

            return true;


        }
        public void UnPostLedger(Sale sale)
        {
            if (sale.PostStatus == LedgerPostStatus.ReadyToPost)
                throw new Exception("Post fail, Transaction is not posted");
            else if (sale.PostStatus == LedgerPostStatus.PreOpening)
                throw new Exception("Post fail, Transaction is pre fiscal year.");
            else
            {
                Console.WriteLine("> Un Posting, " + sale.Name + "," + sale.Status);
                organization.LedgersDal.RemoveTransaction(sale.Id);
                sale.PostStatus = LedgerPostStatus.ReadyToPost;
                this.SaveChanges();
            }
        }
        public void UnPostAllLedger()
        {
            organization.LedgersDal.UnPostAllLedgers(this.transactionType);

            var sqlCommand = "UPDATE [dbo].[ERP_Transactions_Commercial] SET  [PostStatus] = '0' WHERE  [Discriminator] ='{0}' ";
            sqlCommand = string.Format(sqlCommand, this.transactionType.ToString());
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);

        }
    }
}
