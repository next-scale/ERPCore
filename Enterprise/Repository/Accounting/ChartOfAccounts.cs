

using ERPCore.Enterprise.Models.Accounting.ChartOfAccount;
using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.ChartOfAccount.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace ERPCore.Enterprise.Repository.Accounting
{
    public class ChartOfAccounts : ERPNodeDalRepository
    {
        public ChartOfAccounts(Organization organization) : base(organization)
        {

        }


        public List<AccountSubTypes> GetSubType(AccountTypes AccountType)
        {
            var subTypes = Enum.GetValues(typeof(AccountSubTypes)).Cast<AccountSubTypes>();
            switch (AccountType)
            {
                case AccountTypes.Asset:
                    return subTypes.Where(subType => (int)subType >= 100 & (int)subType <= 199).ToList();

                case AccountTypes.Liability:
                    return subTypes.Where(subType => (int)subType >= 200 & (int)subType <= 299).ToList();

                case AccountTypes.Capital:
                    return subTypes.Where(subType => (int)subType >= 300 & (int)subType <= 399).ToList();

                case AccountTypes.Income:
                    return subTypes.Where(subType => (int)subType >= 400 & (int)subType <= 499).ToList();

                case AccountTypes.Expense:
                    return subTypes.Where(subType => (int)subType >= 500 & (int)subType <= 599).ToList();
            }

            return subTypes.ToList();
        }

        public FinancialBalance GetFinancial()
        {
            FinancialBalance fb = new FinancialBalance();

            fb.Node = erpNodeDBContext._dbName;
            fb.AssetBalance = this.AssetAccounts.Sum(i => i.CurrentBalance);
            fb.LiabilityBalance = this.LiabilityAccounts.Sum(i => i.CurrentBalance);
            fb.EquityBalance = fb.AssetBalance - fb.LiabilityBalance;

            fb.ExpenseBalance = this.organization.FiscalYears.CurrentPeriod.Expense;
            fb.IncomeBalance = this.organization.FiscalYears.CurrentPeriod.Income;
            return fb;
        }

        public List<Account> OpeningItemList() => this.erpNodeDBContext.Accounts
                .Where(a => a.OpeningCreditBalance != 0 || a.OpeningDebitBalance != 0)
                .ToList();


        public List<Account> ListRelatedAccounts(Account accountItem) => this.erpNodeDBContext.Accounts
             .Where(i => i.IsFolder == false)
             .Where(i => i.Type == accountItem.Type && i.SubEnumType == accountItem.SubEnumType)
             .ToList();

        public decimal ItemsBalance(AccountTypes type) => this.GetAccountByType(type).Sum(a => a.CurrentBalance);



        public void RemoveGroup(Account accountItem)
        {
            this.erpNodeDBContext.Accounts.Remove(accountItem);

            this.SaveChanges();
        }
        public void ReAssignNumber()
        {
            this.ReAssignNumber(null, AccountTypes.Asset);
            this.ReAssignNumber(null, AccountTypes.Liability);
            this.ReAssignNumber(null, AccountTypes.Capital);
            this.ReAssignNumber(null, AccountTypes.Income);
            this.ReAssignNumber(null, AccountTypes.Expense);

            this.SaveChanges();
        }
        private void ReAssignNumber(Guid? parentId, AccountTypes accountType)
        {
            var accounts = erpNodeDBContext.Accounts
                .Where(a => a.ParentId == parentId)
                .Where(a => a.Type == accountType)
                .OrderByDescending(a => a.IsFolder)
                .ThenByDescending(a => a.SubEnumType)
                .ToList();

            int index = 0;

            accounts.ForEach(account =>
            {
                account.Order = ++index;

                if (account.IsFolder)
                {
                    account.No = account.Parent?.No + account.Order.ToString();
                    this.ReAssignNumber(account.Id, account.Type);
                }
                else
                {
                    account.No = account.Parent?.No + "-" + account.Order.ToString().PadLeft(2, '0');
                }

            });
        }
        private void ClearBalanceHistory()
        {
            erpNodeDBContext.Database.ExecuteSqlRaw("TRUNCATE TABLE [ERP_Accounting_Account_History_Balance]");
            erpNodeDBContext.SaveChanges();
        }
        private void ClearBalanceHistory(Account account)
        {
            var sqlCommand = "DELETE FROM [dbo].[ERP_Accounting_Account_History_Balance] WHERE  [AccountId] = '{0}'";
            sqlCommand = string.Format(sqlCommand, account.Id);
            erpNodeDBContext.Database.ExecuteSqlRaw(sqlCommand);
            erpNodeDBContext.SaveChanges();
        }
        public void Remove(Account account)
        {
            erpNodeDBContext.Accounts.Remove(account);
            erpNodeDBContext.SaveChanges();
        }
        public Account Find(Guid AccountId) => erpNodeDBContext.Accounts.Find(AccountId);
        public Account Find(Guid? AccountId) => erpNodeDBContext.Accounts.Find(AccountId);
        public void GenerateHistoryBalance()
        {
            Console.WriteLine("> {0} Generate History Balance", DateTime.Now.ToLongTimeString());
            this.ClearBalanceHistory();
            DateTime Today = DateTime.Today.Date;

            var accounts = erpNodeDBContext.Accounts.ToList();
            using (var progress = new Helpers.ProgressBar())
            {
                var currentIndex = 0;
                accounts.ForEach(account =>
                {
                    progress.Report(currentIndex++, accounts.Count);
                    this.GenerateHistoryBalance(account);
                });
            }

        }

        public void GenerateHistoryBalance(Guid accountId)
        {
            var account = this.Find(accountId);
            this.GenerateHistoryBalance(account);
        }
        public void GenerateHistoryBalance(Account accountItem)
        {
            var Today = DateTime.Today.Date;
            this.ClearBalanceHistory(accountItem);

            var results = erpNodeDBContext.Ledgers
                .Where(GL => GL.AccountId == accountItem.Id)
                .Where(GL => GL.TransactionDate <= Today)
                .GroupBy(o => new { o.TransactionDate })
                .Select(go => new
                {
                    TransactionDate = go.Key.TransactionDate,
                    Credit = go.Sum(ii => ii.Credit) ?? 0,
                    Debit = go.Sum(ii => ii.Debit) ?? 0,
                    Count = go.Count()
                })
                .OrderBy(r => r.TransactionDate)
                .ToList();

            decimal balance = 0;
            erpNodeDBContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            List<HistoryBalanceItem> HistoryBalanceItems = new List<HistoryBalanceItem>();

            foreach (var result in results)
            {
                var historyBalanceItem = new HistoryBalanceItem()
                {
                    Id = Guid.NewGuid(),
                    TransactionDate = result.TransactionDate,
                    AccountId = accountItem.Id,
                    AccountItem = accountItem,
                    Debit = result.Debit,
                    Credit = result.Credit,
                    Count = result.Count
                };

                balance = balance + historyBalanceItem.Total;
                historyBalanceItem.Balance = balance;
                HistoryBalanceItems.Add(historyBalanceItem);
            }

            erpNodeDBContext.HistoryBalanceItems.AddRange(HistoryBalanceItems);
            accountItem.CurrentBalanceRecordDate = DateTime.Today;
            erpNodeDBContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            erpNodeDBContext.ChangeTracker.DetectChanges();
            erpNodeDBContext.SaveChanges();
        }


        public Account newFolder(AccountTypes type, AccountSubTypes subType, string codeName, string name, string nameEnglish)
        {
            Account item = new Account();

            item.Id = Guid.NewGuid();
            item.Type = type;
            item.SubEnumType = subType;
            item.IsFolder = true;
            item.CodeName = codeName;
            item.Name = name;

            erpNodeDBContext.Accounts.Add(item);
            return item;
        }
        public Account newGroup(AccountTypes accountType)
        {
            var newGroup = new Account()
            {
                Id = Guid.NewGuid(),
                Type = accountType,
                IsFolder = true
            };


            erpNodeDBContext.Accounts.Add(newGroup);
            erpNodeDBContext.SaveChanges();

            return newGroup;
        }
        public Account newItem(AccountTypes accountType, AccountSubTypes accountSubType)
        {
            var newItem = new Account()
            {
                Type = accountType,
                SubEnumType = accountSubType,
                OpeningBalance = 0,
                No = "N/A",
            };

            return newItem;
        }
        public Account newItem(AccountTypes type, AccountSubTypes subType, string codeName, string name, string englishName)
        {
            Account item = new Account();

            item.Id = Guid.NewGuid();
            item.Type = type;
            item.SubEnumType = subType;
            item.IsFolder = false;
            item.CodeName = codeName;
            item.Name = name;
            item.EnglishName = englishName;

            erpNodeDBContext.Accounts.Add(item);
            return item;
        }

        public Account Update(Account accountItem)
        {
            var exist = erpNodeDBContext.Accounts.Find(accountItem.Id);

            if (exist == null)
            {
                erpNodeDBContext.Accounts.Add(accountItem);
                exist = accountItem;
            }
            else
            {
                exist.No = accountItem.No;
                exist.Name = accountItem.Name;
                exist.IsLiquidity = accountItem.IsLiquidity;
                exist.ParentId = accountItem.ParentId;
                exist.SubEnumType = accountItem.SubEnumType;
                exist.Description = accountItem.Description;
                exist.IsPreviewDisplay = accountItem.IsPreviewDisplay;
                exist.IsCashEquivalent = accountItem.IsCashEquivalent;
                exist.OpeningDebitBalance = accountItem.OpeningDebitBalance;
                exist.OpeningCreditBalance = accountItem.OpeningCreditBalance;

            }


            if (exist.SubEnumType == AccountSubTypes.Cash || exist.SubEnumType == AccountSubTypes.Bank)
                exist.IsCashEquivalent = true;



            erpNodeDBContext.SaveChanges();

            return exist;
        }
        private void ClearBalance()
        {
            this.GetAccountsList().ToList().ForEach(Account => Account.ClearBalance());
        }
        public void UpdateBalance()
        {
            Console.WriteLine("> Update Account Balance @ {0}", DateTime.Now.ToLongTimeString());
            this.ClearBalance();
            this.erpNodeDBContext.SaveChanges();

            var firstDate = organization.DataItems.FirstDate;

            var accountBalances = erpNodeDBContext.Ledgers
                .Where(ledger => ledger.TransactionDate <= DateTime.Today)
                .GroupBy(o => o.AccountId)
                .Select(go => new
                {
                    AccountId = go.Key,
                    Account = go.Select(i => i.accountItem).FirstOrDefault(),
                    Credit = go.Sum(i => i.Credit) ?? 0,
                    Debit = go.Sum(i => i.Debit) ?? 0,
                })
                .ToList();


            accountBalances.ForEach(group =>
            {
                group.Account.Debit = group.Debit;
                group.Account.Credit = group.Credit;
                group.Account.CurrentBalanceRecordDate = DateTime.Today;

              //  Console.WriteLine(group.Account.Code + group.Account.Name + ", Dr." + group.Account.Debit + ", Cr." + group.Account.Credit);
            });

            this.SaveChanges();
        }
        public void UpdateBalance(Account accountItem)
        {
            Console.WriteLine("> Update " + accountItem.Name + " Balance.");

            accountItem.ClearBalance();
            erpNodeDBContext.SaveChanges();

            var accountBalances = erpNodeDBContext.Ledgers
                .Where(ledger => ledger.TransactionDate <= DateTime.Today)

                .Where(account => account.AccountId == accountItem.Id)
                .GroupBy(o => o.AccountId)
                .Select(go => new
                {
                    AccountId = go.Key,
                    Account = go.Select(i => i.accountItem).FirstOrDefault(),
                    Credit = go.Sum(i => i.Credit) ?? 0,
                    Debit = go.Sum(i => i.Debit) ?? 0,
                })
                .ToList();


            accountBalances.ForEach(group =>
            {
                group.Account.Credit = group.Credit;
                group.Account.Debit = group.Debit;
                group.Account.CurrentBalanceRecordDate = DateTime.Today;
            });

            this.SaveChanges();
        }


        public List<HistoryBalanceItem> GetHistoryBalance(Account accountItem, int days = 365)
        {
            var startDate = DateTime.Today.AddDays(-1 * days);

            return erpNodeDBContext.HistoryBalanceItems
                .Where(h => h.AccountId == accountItem.Id)
                .ToList();
        }
        public List<Account> GetByType(AccountTypes AccountType)
        {
            return erpNodeDBContext.Accounts
            .Where(account => account.Type == AccountType)
            .OrderByDescending(i => i.IsFolder)
            .ThenBy(i => i.No)
            .ToList();
        }
        public List<Account> GetFolderByTypes(AccountTypes AccountType) => erpNodeDBContext.Accounts.Where(account => account.Type == AccountType && account.IsFolder == true).ToList();
        public List<Account> GetItemBySubType(AccountSubTypes subType) => erpNodeDBContext.Accounts
                .Where(account => account.SubEnumType == subType)
                .Where(account => account.IsFolder == false)
                .ToList();
        public List<Account> GetAccountByType(AccountTypes AccountType) => erpNodeDBContext.Accounts
                .Where(account => account.Type == AccountType)
                .Where(account => account.IsFolder == false)
                .OrderBy(i => i.No)
                .ToList();
        public List<Account> ListPreviewAccounts(Guid profileId) => erpNodeDBContext
                    .PreviewAccounts
                    .Where(pv => (pv.Account.SubEnumType != AccountSubTypes.Cash) && (pv.Account.SubEnumType != AccountSubTypes.Bank))
                    .Where(pv => pv.OwnerProfileGuid == profileId)
                    .Select(pv => pv.Account)
                    .ToList();
        public List<Account> GetAccountsList() => erpNodeDBContext.Accounts
            .Where(i => i.IsFolder == false)
            .ToList();
        public List<Account> Folders => erpNodeDBContext.Accounts
            .Where(i => i.IsFolder)
            .ToList();
        public List<Account> AssetAccounts => this.GetAccountByType(AccountTypes.Asset);
        public List<Account> Assets => erpNodeDBContext.Accounts.Where(a => a.Type == AccountTypes.Asset).ToList();
        public List<Account> AssetAndLiability
        {
            get
            {
                return erpNodeDBContext.Accounts
              .Where(account => account.Type == AccountTypes.Asset || account.Type == AccountTypes.Liability)
              .Where(account => account.IsFolder == false)
              .Where(account => account.IsFolder == false).ToList();
            }
        }
        public List<Account> CashEquivalentAccounts => erpNodeDBContext.Accounts
              .Where(account => account.Type == AccountTypes.Asset)
               .Where(account => account.IsFolder == false)
              .Where(account => account.SubEnumType == AccountSubTypes.Bank || account.SubEnumType == AccountSubTypes.Cash || account.IsCashEquivalent)
              .ToList();
        public List<Account> CashOrBankAccounts => erpNodeDBContext.Accounts
            .Where(account => account.Type == AccountTypes.Asset)
             .Where(account => account.IsFolder == false)
            .Where(account => account.SubEnumType == AccountSubTypes.Bank || account.SubEnumType == AccountSubTypes.Cash)
            .ToList();
        public List<Account> COGSExpenseAccounts => this.GetItemBySubType(AccountSubTypes.CostOfGoodsSold);
        public List<Account> EquityAccounts => this.GetAccountByType(Models.ChartOfAccount.AccountTypes.Capital);
        public List<Account> ExpenseAccounts => this.GetAccountByType(AccountTypes.Expense);
        public List<Account> TaxRelatedAccountItems => erpNodeDBContext.Accounts
                 .Where(account => account.SubEnumType == AccountSubTypes.TaxInput || account.SubEnumType == AccountSubTypes.TaxOutput || account.SubEnumType == AccountSubTypes.TaxExpense)
                 .Where(account => account.IsFolder == false)
                 .OrderBy(i => i.No)
                 .ToList();
        public List<Account> TaxClosingAccount => erpNodeDBContext.Accounts
                 .Where(account => account.SubEnumType == AccountSubTypes.TaxPayable || account.SubEnumType == AccountSubTypes.TaxReceivable)
                 .Where(account => account.IsFolder == false)
                 .OrderBy(i => i.No)
                 .ToList();
        public List<Account> ListTaxAccounts(Models.Taxes.Enums.TaxDirection direction)
        {

            if (direction == Models.Taxes.Enums.TaxDirection.Input)
                return erpNodeDBContext.Accounts.Where(account => account.SubEnumType == AccountSubTypes.TaxInput)
                             .Where(account => account.IsFolder == false)
                             .OrderBy(i => i.No)
                             .ToList();
            else
                return erpNodeDBContext.Accounts.Where(account => account.SubEnumType == AccountSubTypes.TaxOutput)
                                .Where(account => account.IsFolder == false)
                                .OrderBy(i => i.No)
                                .ToList();


        }
        public List<Account> FixedAssets => this.GetItemBySubType(AccountSubTypes.FixedAsset);
        public List<Account> IncomeAccounts => this.GetAccountByType(AccountTypes.Income);
        public List<Account> InventoryAssetAccounts => this.GetItemBySubType(AccountSubTypes.Inventory);
        public List<Account> LiabilityAccounts => this.GetAccountByType(AccountTypes.Liability);
        public List<Account> CurrentLiabilityAccounts => this.GetItemBySubType(AccountSubTypes.CurrentLiability);
        public List<Account> AccountReceivableAccounts => this.GetItemBySubType(AccountSubTypes.AccountReceivable);
        public List<Account> AccountPayableAccounts => this.GetItemBySubType(AccountSubTypes.AccountPayable);
        public List<Account> TaxInputAccounts => this.GetItemBySubType(AccountSubTypes.TaxInput);
        public List<Account> TaxOutputAccounts => this.GetItemBySubType(AccountSubTypes.TaxOutput);
        public List<Account> TaxPayableAccounts => this.GetItemBySubType(AccountSubTypes.TaxPayable);
        public List<Account> TaxReceivableAccounts => this.GetItemBySubType(AccountSubTypes.TaxReceivable);
        public List<Account> TaxInputAndTaxExpenseAccounts => erpNodeDBContext.Accounts
                 .Where(account => account.SubEnumType == AccountSubTypes.TaxInput || account.SubEnumType == AccountSubTypes.TaxExpense)
                 .Where(account => account.IsFolder == false)
                 .OrderBy(i => i.No)
                 .ToList();

    }
}

