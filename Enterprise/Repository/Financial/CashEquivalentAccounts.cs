
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Financial.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPCore.Enterprise.Repository.Financial
{
    public class CashEquivalentAccounts : ERPNodeDalRepository
    {


        public CashEquivalentAccounts(Organization organization) : base(organization)
        {

        }

        public List<Models.ChartOfAccount.Account> All
        {
            get
            {
                var bankAccounts = erpNodeDBContext.Accounts
                .Where(account => account.SubEnumType == AccountSubTypes.Bank || account.SubEnumType == AccountSubTypes.Cash)
                .OrderBy(account => account.BankAccountBankName)
                .ToList();

                return bankAccounts;
            }
        }
    }
}