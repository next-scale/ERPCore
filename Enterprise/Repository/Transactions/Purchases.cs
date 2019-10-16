using ERPCore.Enterprise.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Models.Transactions.Commercials;
using Microsoft.EntityFrameworkCore;
using ERPCore.Enterprise.Models.Transactions.Enums;
using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.Financial.Payments;
using ERPCore.Enterprise.Models.Financial.Payments.Enums;
using ERPCore.Enterprise.Models.Logging;

namespace ERPCore.Enterprise.Repository.Transactions
{
    public class Purchases : Commercials
    {
        public Purchases(Organization organization) : base(organization)
        {
            this.transactionType = TransactionTypes.Purchase;
        }
        public new IQueryable<Purchase> Query => erpNodeDBContext.Purchases;
        public IQueryable<Purchase> GetByStatus(CommercialStatus status) => this.Query
            .Where(t => t.Status == status)
            .Include(t => t.Profile);


        public new IQueryable<Purchase> QueryOpen => this.erpNodeDBContext
            .Purchases
            .Where(t => t.Status == CommercialStatus.Open);

        public List<Purchase> ReadyForPost => this.erpNodeDBContext
            .Purchases
            .Where(s => s.PostStatus == LedgerPostStatus.ReadyToPost)
            .Include(s => s.CommercialItems)
            .ToList();

        public int OpenCount => this.erpNodeDBContext
            .Purchases
            .Where(b => b.Status == CommercialStatus.Open)
            .Count();
        public int PaidCount => this.erpNodeDBContext
            .Purchases
            .Where(b => b.Status == CommercialStatus.Paid)
            .Count();

        public int OverDueCount
        {
            get
            {
                var expDate = DateTime.Today.AddDays(-30);
                var overdueCount = erpNodeDBContext.Purchases
                    .Where(b => b.Status == CommercialStatus.Open)
                    .Where(b => b.TransactionDate < expDate)
                    .Count();
                return overdueCount;
            }
        }
        public Decimal OpenBalance => erpNodeDBContext.Purchases
            .Where(b => b.Status == CommercialStatus.Open)
            .Sum(b => (Decimal?)b.Total) ?? 0;
        public Decimal PaidBalance => erpNodeDBContext.Purchases
    .Where(b => b.Status == CommercialStatus.Paid)
    .Sum(b => (Decimal?)b.Total) ?? 0;
        public Decimal AllBalance => erpNodeDBContext.Purchases
    .Sum(b => (Decimal?)b.Total) ?? 0;

        public IQueryable<Purchase> GetByViewStatus(CommercialViewStatus status)
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

        public IQueryable<Purchase> LastFiscal
        {
            get
            {
                var currentFiscal = organization.FiscalYears.CurrentPeriod;

                var burchaseBeforeDate = currentFiscal.EndDate;
                var PurchasesList = erpNodeDBContext.Purchases
                    .Where(b => b.TransactionDate < currentFiscal.StartDate)
                    .Where(b => b.CloseDate >= currentFiscal.StartDate);
                return PurchasesList;
            }
        }

        public IQueryable<Purchase> GetOverDueList
        {
            get
            {
                var expDate = DateTime.Today.AddDays(-30);
                var overdueList = erpNodeDBContext.Purchases
                    .Where(b => b.Status == CommercialStatus.Open)
                    .Where(b => b.TransactionDate < expDate);
                return overdueList;
            }
        }


        public Decimal OverDueBalance
        {
            get
            {
                var expDate = DateTime.Today.AddDays(-30);
                var overdueBalance = erpNodeDBContext.Purchases
                    .Where(b => b.Status == CommercialStatus.Open)
                    .Where(b => b.TransactionDate < expDate)
                    .Sum(b => (Decimal?)b.Total) ?? 0;
                return overdueBalance;
            }
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

        public void UpdatePurchasingBalance()
        {
            var BalanceTables = erpNodeDBContext.Purchases
                .GroupBy(o => o.ProfileGuid)
                .ToList()
                .Select(go => new
                {
                    Profile = go.Select(i => i.Profile).FirstOrDefault(),
                    TotalPurchase = go.Sum(i => i.Total),
                    CountPurchase = go.Count(),
                    TotalBalance = go.Sum(i => i.TotalBalance),
                    CountBalance = go.Where(i => i.TotalBalance > 0).Count(),
                })
                .ToList();


            erpNodeDBContext.Suppliers.ToList().ForEach(p =>
            {
                p.SumPurchaseBalance = 0;
                p.TotalBalance = 0;
                p.CountPurchases = 0;
            });

            BalanceTables.ForEach(b =>
            {
                if (b.Profile != null && b.Profile.Supplier != null)
                {
                    b.Profile.Supplier.SumPurchaseBalance = b.TotalPurchase;
                    b.Profile.Supplier.CountPurchases = b.CountPurchase;
                    b.Profile.Supplier.TotalBalance = b.TotalBalance;
                    b.Profile.Supplier.CountBalance = b.CountBalance;
                }
                else
                {
                    organization.EventLogs.NewEventLog(EventLogLevel.Error, "1111", "Unknow b.profile", b.Profile.Name, "");
                }
            });

            erpNodeDBContext.SaveChanges();
        }
        public void AddPayment(Guid id, DateTime payDate)
        {
            var purchase = erpNodeDBContext.Purchases.Find(id);

            var payment = new SupplierPayment()
            {
                Id = Guid.NewGuid(),
                TransactionDate = payDate,
                AssetAccount = organization.SystemAccounts.Cash,
                LiabilityAccount = organization.SystemAccounts.AccountPayable,
            };


            purchase.UpdatePayment();
            erpNodeDBContext.SaveChanges();

        }
        public object Create(Guid profileGuid, object purchasePurposes)
        {
            throw new NotImplementedException();
        }
        public new Purchase Find(Guid transactionGuid) => erpNodeDBContext.Purchases.Find(transactionGuid);
        private new int NextNumber
        {
            get
            {
                try
                {
                    return erpNodeDBContext.Purchases.Max(e => e.No) + 1;
                }
                catch (Exception)
                {
                    return 1;
                }
            }
        }
        public void ReOrder()
        {
            var Purchases = erpNodeDBContext.Purchases
                .OrderBy(tr => tr.TransactionDate)
                .GroupBy(o => new { o.TransactionDate.Year, o.TransactionDate.Month })

                .Select(go => new
                {
                    year = go.Key.Year,
                    month = go.Key.Month,
                    purchases = go.ToList()
                })
                .ToList();




            foreach (var purchaseMonth in Purchases)
            {
                int i = 0;
                purchaseMonth.purchases.OrderBy(p => p.TransactionDate).ToList().ForEach(p =>
                  {
                      p.No = ++i;
                  });
            }
            this.SaveChanges();
        }
        public Purchase Update(Purchase purchase)
        {
            var existPurchase = erpNodeDBContext.Purchases.Find(purchase.Id);

            if (existPurchase == null)
                throw new Exception("Cannot update, Transaction not found");
            else if (existPurchase.PostStatus == LedgerPostStatus.Posted)
                throw new Exception("Cannot update, Transaction is Posted");
            else
            {
                existPurchase.ProjectGuid = purchase.ProjectGuid;
                existPurchase.TransactionDate = purchase.TransactionDate;
                existPurchase.Reference = purchase.Reference;
                existPurchase.Memo = purchase.Memo;
                existPurchase.PaymentTermGuid = purchase.PaymentTermGuid;
                existPurchase.ShippingTermGuid = purchase.ShippingTermGuid;
                existPurchase.TaxOffset = purchase.TaxOffset;

                existPurchase.ReCalculate();
                erpNodeDBContext.SaveChanges();
            }

            return existPurchase;
        }

        public void UpdateProfilesCache()
        {
            var commercials = erpNodeDBContext.Commercials.ToList();

            commercials.Where(c => c.ProfileGuid != null).ToList().ForEach(c =>
            {
                c.CacheProfileName = c.Profile.DisplayName;
            });
            erpNodeDBContext.SaveChanges();
        }
        public Purchase Create(Guid profileGuid, PurchasePurposes purpose)
        {
            return Create(profileGuid, purpose, DateTime.Today);
        }
        public Purchase Create(Guid profileGuid, PurchasePurposes purpose, DateTime createDate)
        {

            var newPurchase = new Purchase()
            {
                ProfileGuid = profileGuid,
                No = this.NextNumber,
                PurchasePurposes = purpose,
                TransactionDate = createDate,
                CompanyProfile = erpNodeDBContext.Profiles.Where(p => p.isSelfOrganization).FirstOrDefault(),
                TransactionType = this.transactionType
            };

            erpNodeDBContext.Commercials.Add(newPurchase);
            erpNodeDBContext.SaveChanges();

            return newPurchase;
        }

        public bool Delete(Purchase purchase)
        {
            if (purchase == null)
                throw new Exception("Delete fail, Transaction not found");
            else if (purchase.PostStatus == LedgerPostStatus.Posted)
                throw new Exception("Delete fail, Transaaction aleardy posted");
            else
            {
                purchase.RemoveItems();
                purchase.RemoveCommercialTaxs();
                erpNodeDBContext.Commercials.Remove(purchase);
                organization.SaveChanges();
            }
            return true;
        }

        public Purchase Copy(Purchase originalPurchase, DateTime trDate)
        {
            var clonePurchase = this.erpNodeDBContext.Purchases
                    .AsNoTracking()
                    .Include(p => p.CommercialItems)
                    .FirstOrDefault(x => x.Id == originalPurchase.Id);

            if (clonePurchase == null)
                throw new Exception("Copy Fail, Transaction not found");
            else
            {
                clonePurchase.Id = Guid.NewGuid();
                clonePurchase.TransactionDate = trDate;
                clonePurchase.Reference = "Clone-" + clonePurchase.Reference;
                clonePurchase.No = organization.Purchases.NextNumber;
                clonePurchase.PostStatus = LedgerPostStatus.ReadyToPost;
                clonePurchase.CommercialPayment = null;
                clonePurchase.CommercialPaymentId = null;
                clonePurchase.CommercialItems.ToList().ForEach(ci => ci.Id = Guid.NewGuid());

                this.erpNodeDBContext.Purchases.Add(clonePurchase);
                this.erpNodeDBContext.SaveChanges();

                return clonePurchase;
            }

        }
        public void UpdateDailyBalance()
        {
            this.ClearDailyBalance();
            var dailyPurchases = this.Query.GroupBy(t => new { t.TransactionDate })
                                           .Select(go => new
                                           {
                                               IncomeAmount = go.Sum(i => i.Total),
                                               TransactionDate = go.Key.TransactionDate
                                           }).ToList();

            Console.WriteLine("> {0} Update Daily Purchases Balance {2} [{1}]", DateTime.Now.ToLongTimeString(), dailyPurchases.Count(), this.transactionType.ToString());

            erpNodeDBContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            dailyPurchases.ForEach(ds =>
            {
                var dailyPurchase = new CommercialDailyBalance()
                {
                    TransactionDate = ds.TransactionDate,
                    Balance = ds.IncomeAmount,
                    Type = this.transactionType,
                    Id = Guid.NewGuid()
                };
                this.erpNodeDBContext.CommercialDailyBalances.Add(dailyPurchase);
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
        public bool PostLedger(Purchase tr, bool SaveImmediately = true)
        {
            if (tr.PostStatus == LedgerPostStatus.Posted)
                throw new Exception("Post fail, Transaaction aleardy posted");
            if (tr.Total == 0)
                return false;

            var trLedger = new Models.Accounting.LedgerGroup()
            {
                Id = tr.Id,
                TransactionDate = tr.TransactionDate,
                TransactionName = tr.Name,
                TransactionNo = tr.No,
                TransactionType = transactionType,
                Reference = tr.Reference,
                ProfileName = tr.Profile?.Name,
            };

            this.PostLedger_Items(trLedger, tr);
            this.PostLedger_Tax(trLedger, tr);
            trLedger.AddCredit(organization.SystemAccounts.AccountPayable, tr.Total);

            if (trLedger.FinalValidate())
            {
                tr.PostStatus = LedgerPostStatus.Posted;
                erpNodeDBContext.LedgerGroups.Add(trLedger);
            }
            else
            {
                trLedger.RemoveAllLedgerLines();
                organization.EventLogs.NewEventLog(EventLogLevel.Error,
                    "1011", "Error Posting", trLedger.TransactionName, "");
                return false;
            }

            if (SaveImmediately)
                erpNodeDBContext.SaveChanges();

            return true;
        }

        private void PostLedger_Items(Models.Accounting.LedgerGroup trLedger, Purchase purchase)
        {
            foreach (var transactionItem in purchase.CommercialItems)
            {
                string memo = transactionItem.Amount.ToString() + " x " + transactionItem.ItemPartNumber;

                if (transactionItem.Item.GetPurchaseAccount == null)
                    transactionItem.Item.PurchaseAccount = organization.SystemAccounts.COSG;

                trLedger.AddDebit(transactionItem.Item.GetPurchaseAccount, transactionItem.LineTotal, memo);

            }
        }
        private bool PostLedger_Tax(Models.Accounting.LedgerGroup trLedger, Purchase purchase)
        {
            purchase.CommercialTaxes
                 .ToList()
                 .ForEach(commercialTax =>
                 {

                     trLedger.AddDebit(commercialTax.TaxCode.TaxAccount, commercialTax.TaxBalance);
                 });

            return true;
        }

        public void UnPostLedger(Purchase purchase)
        {
            Console.WriteLine("> Un Posting ,PO" + purchase.No + "," + purchase.Status);
            organization.LedgersDal.RemoveTransaction(purchase.Id);

            purchase.PostStatus = LedgerPostStatus.ReadyToPost;
            erpNodeDBContext.SaveChanges();
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