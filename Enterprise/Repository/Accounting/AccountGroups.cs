
using ERPCore.Enterprise.Models.ChartOfAccount;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ERPCore.Enterprise.Repository.Accounting
{
    public class AccountGroups : ERPNodeDalRepository
    {
        public AccountGroups(Organization organization) : base(organization)
        {

        }

        public Account Find(Guid AccountId)
        {
            return erpNodeDBContext.Accounts
                .Where(accountItem => accountItem.IsFolder == true)
                .Where(accountItem => accountItem.Id == AccountId)
                .FirstOrDefault();
        }

        public Account Find(string accountGroupGuid)
        {
            return erpNodeDBContext.Accounts.Find(Guid.Parse(accountGroupGuid));
        }

        public Account Create(AccountTypes query)
        {
            var accountGroup = new Account()
            {
                Type = query,
                IsFolder = true
            };

            return accountGroup;
        }

        public List<Account> Get(AccountTypes AccountType)
        {
            return erpNodeDBContext.Accounts
            .Where(account => account.Type == AccountType)
            .ToList();
        }



        public Account Save(Account accountGroup)
        {
            var exist = erpNodeDBContext.Accounts.Find(accountGroup.Id);

            if (exist == null)
            {
                accountGroup.Id = Guid.NewGuid();
                accountGroup.IsFolder = true;

                erpNodeDBContext.Accounts.Add(accountGroup);
                exist = accountGroup;
            }
            else
            {
                exist.Name = accountGroup.Name;
                exist.ParentId = accountGroup.ParentId;
                exist.CodeName = accountGroup.CodeName;
            }

            erpNodeDBContext.SaveChanges();
            return exist;
        }

    }
}

