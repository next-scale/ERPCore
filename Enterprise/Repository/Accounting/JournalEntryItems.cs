
using ERPCore.Enterprise.Models.ChartOfAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Repository.Company;
using ERPCore.Enterprise.Models.AccountingEntries;
using ERPCore.Enterprise.Models.Transactions;
using Microsoft.EntityFrameworkCore;
using ERPCore.Enterprise.Models.Accounting.Enums;

namespace ERPCore.Enterprise.Repository.Accounting
{

    public class JournalEntryItems : ERPNodeDalRepository
    {
        public JournalEntryItems(Organization organization) : base(organization)
        {

        }

        public List<JournalEntryLine> GetAll => erpNodeDBContext.JournalEntryLines.ToList();
        public JournalEntryLine Find(Guid id) => erpNodeDBContext.JournalEntryLines.Find(id);
    }
}