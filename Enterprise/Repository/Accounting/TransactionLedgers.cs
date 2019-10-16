
using ERPCore.Enterprise.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Reports.CompanyandFinancial;
using ERPCore.Enterprise.Models.Accounting.Enums;

namespace ERPCore.Enterprise.Repository.Accounting
{
    public class LedgerGroups : ERPNodeDalRepository
    {

        public LedgerGroups(Organization organization) : base(organization)
        {

        }

        public IQueryable<Models.Accounting.LedgerGroup> All => erpNodeDBContext.LedgerGroups;

        public object Find(Guid id) => erpNodeDBContext.LedgerGroups.Find(id);
    }
}