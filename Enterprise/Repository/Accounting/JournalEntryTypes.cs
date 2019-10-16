
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

    public class JournalEntryTypes : ERPNodeDalRepository
    {
        public JournalEntryTypes(Organization organization) : base(organization)
        {

        }

        public List<JournalEntryType> ListAll => erpNodeDBContext.JournalEntryTypes.ToList();
        public JournalEntryType Find(Guid id) => erpNodeDBContext.JournalEntryTypes.Find(id);

        public JournalEntryType CreateNew(JournalEntryType template)
        {
            template.Id = Guid.NewGuid();
            erpNodeDBContext.JournalEntryTypes.Add(template);
            erpNodeDBContext.SaveChanges();

            return template;
        }
    }
}