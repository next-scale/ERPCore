using ERPCore.Enterprise.Models.Projects;
using ERPCore.Enterprise.Models.ChartOfAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Models.ChartOfAccount.Enums;

namespace ERPCore.Enterprise.Repository.Accounting
{
    public class SystemAccounts : ERPNodeDalRepository
    {
        public SystemAccounts(Organization organization) : base(organization)
        {

        }

        public List<DefaultAccount> All => erpNodeDBContext.DefaultAccounts.ToList();

        public Account Cash => this.GetAccount(SystemAccountType.Cash);
        public Account BankFee => this.GetAccount(SystemAccountType.BankFee);
        public Account COSG => this.GetAccount(SystemAccountType.CostOfGoodSold);
        public Account OverPayment => this.GetAccount(SystemAccountType.OverRecivePayment);
        public Account RetainedEarning => this.GetAccount(SystemAccountType.RetainedEarning);
        public Account AccountPayable => this.GetAccount(SystemAccountType.AccountPayable);
        public Account AccountReceivable => this.GetAccount(SystemAccountType.AccountReceivable);
        public Account OpeningBalanceEquity => this.GetAccount(SystemAccountType.OpeningBalanceEquity);

        public void Add(DefaultAccount defaultAccountItem)
        {
            erpNodeDBContext.DefaultAccounts.Add(defaultAccountItem);
            erpNodeDBContext.SaveChanges();
        }

        public Account EquityStock => this.GetAccount(SystemAccountType.EquityStock);
        public Account DiscountGiven => this.GetAccount(SystemAccountType.DiscountGiven);
        public Account DiscountTaken => this.GetAccount(SystemAccountType.DiscountTaken);
        public Account Income => this.GetAccount(SystemAccountType.Income);
        public Account Expense => this.GetAccount(SystemAccountType.Expense);

        public Account GetAccount(SystemAccountType type)
        {
            var defaultAccountItem = erpNodeDBContext.DefaultAccounts.Find(type);
            if (defaultAccountItem != null)
                return defaultAccountItem.AccountItem;
            else
                return null;
        }

        public DefaultAccount Find(SystemAccountType type) => erpNodeDBContext.DefaultAccounts.Find(type);



        public void SetIfUnAssign(SystemAccountType defaultAccountType, Account accountItem)
        {
            if (this.GetAccount(defaultAccountType) == null)
            {
                this.Set(defaultAccountType, accountItem);
            }
        }

        public void Set(SystemAccountType defaultAccountType, Account accountItem)
        {
            if (accountItem != null)
            {
                var defaultAccountItem = erpNodeDBContext.DefaultAccounts.Find(defaultAccountType);

                if (defaultAccountItem != null)
                {
                    defaultAccountItem.AccountItemId = accountItem.Id;
                    defaultAccountItem.AccountItem = accountItem;
                    defaultAccountItem.LastUpdate = DateTime.Today;
                }
                else
                {
                    defaultAccountItem = new DefaultAccount()
                    {
                        AccountType = defaultAccountType,
                        AccountItem = accountItem,
                        AccountItemId = accountItem.Id,
                        LastUpdate = DateTime.Today
                    };
                    erpNodeDBContext.DefaultAccounts.Add(defaultAccountItem);
                }
            }
            erpNodeDBContext.SaveChanges();

        }

        public void Create(SystemAccountType accountType)
        {
            var defaultAccountItem = erpNodeDBContext.DefaultAccounts.Find(accountType);

            if (defaultAccountItem == null)
            {
                defaultAccountItem = new DefaultAccount()
                {
                    AccountType = accountType,
                    LastUpdate = DateTime.Today
                };

                erpNodeDBContext.DefaultAccounts.Add(defaultAccountItem);
                erpNodeDBContext.SaveChanges();
            }
        }



        public void AutoAssignSystemAccount()
        {
            this.AutoCreateSystemAccount();

            Console.WriteLine("=> Auto Assign System Account");
            var accounts = erpNodeDBContext.Accounts.Where(i => i.IsFolder == false).ToList();

            this.SetIfUnAssign(SystemAccountType.Income, accounts.Where(i => i.Type == AccountTypes.Income).FirstOrDefault());
            this.SetIfUnAssign(SystemAccountType.Cash, accounts.Where(i => i.SubEnumType == AccountSubTypes.Cash).FirstOrDefault());
            this.SetIfUnAssign(SystemAccountType.AccountPayable, accounts.Where(i => i.SubEnumType == AccountSubTypes.AccountPayable).FirstOrDefault());
            this.SetIfUnAssign(SystemAccountType.Expense, accounts.Where(i => i.SubEnumType == AccountSubTypes.Expense).FirstOrDefault());
            this.SetIfUnAssign(SystemAccountType.RetainedEarning, accounts.Where(i => i.SubEnumType == AccountSubTypes.RetainEarning).FirstOrDefault());
            this.SetIfUnAssign(SystemAccountType.EquityStock, accounts.Where(i => i.SubEnumType == AccountSubTypes.Stock).FirstOrDefault());
            this.SetIfUnAssign(SystemAccountType.OpeningBalanceEquity, accounts.Where(i => i.SubEnumType == AccountSubTypes.OpeningBalance).FirstOrDefault());
            this.SetIfUnAssign(SystemAccountType.DiscountGiven, accounts.Where(i => i.SubEnumType == AccountSubTypes.DiscountGiven).FirstOrDefault());
            this.SetIfUnAssign(SystemAccountType.DiscountTaken, accounts.Where(i => i.Type == AccountTypes.Income && i.SubEnumType == AccountSubTypes.DiscountTaken).FirstOrDefault());
            this.SetIfUnAssign(SystemAccountType.Inventory, accounts.Where(i => i.SubEnumType == AccountSubTypes.Inventory).FirstOrDefault());
            this.SetIfUnAssign(SystemAccountType.AccountReceivable, accounts.Where(i => i.SubEnumType == AccountSubTypes.AccountReceivable).FirstOrDefault());
            this.SetIfUnAssign(SystemAccountType.EarnestAsset, accounts.Where(i => i.SubEnumType == AccountSubTypes.EarnestPayment).FirstOrDefault());
            this.SetIfUnAssign(SystemAccountType.EarnestLiability, accounts.Where(i => i.SubEnumType == AccountSubTypes.EarnestReceive).FirstOrDefault());
            this.SetIfUnAssign(SystemAccountType.BankFee, accounts.Where(i => i.SubEnumType == AccountSubTypes.BankFee).FirstOrDefault());
            this.SetIfUnAssign(SystemAccountType.EquityPremiumStock, accounts.Where(i => i.SubEnumType == AccountSubTypes.OverStockValue).FirstOrDefault());
            this.SetIfUnAssign(SystemAccountType.CostOfGoodSold, accounts.Where(i => i.SubEnumType == AccountSubTypes.CostOfGoodsSold).FirstOrDefault());
            this.SetIfUnAssign(SystemAccountType.OverRecivePayment, accounts.Where(i => i.SubEnumType == AccountSubTypes.OverReceivePayment).FirstOrDefault());



            this.erpNodeDBContext.SaveChanges();
            Console.WriteLine("  => Complete");
        }

        public void AutoCreateSystemAccount()
        {
            var systemAccountTypeList = Enum.GetValues(typeof(SystemAccountType)).Cast<SystemAccountType>();

            systemAccountTypeList.ToList().ForEach(t =>
            {
                this.Create(t);
            });
        }
    }
}