
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Financial.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPCore.Enterprise.Repository.Financial
{
    public class LiabilityAccounts : ERPNodeDalRepository
    {
        public LiabilityAccounts(Organization organization) : base(organization)
        {

        }

        public List<Models.ChartOfAccount.Account> All
        {
            get
            {
                var accounts = erpNodeDBContext.Accounts
                .Where(account => account.Type == AccountTypes.Liability)
                .ToList();
                return accounts;
            }
        }

        public List<Models.ChartOfAccount.Account> AccountItems
        {
            get
            {
                var accounts = erpNodeDBContext.Accounts
                .Where(account => account.Type == AccountTypes.Liability && account.IsFolder == false)
                .ToList();
                return accounts;
            }
        }
    }
}