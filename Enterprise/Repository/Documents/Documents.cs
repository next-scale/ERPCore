
using ERPCore.Enterprise.Models.ChartOfAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Repository.Company;
using ERPCore.Enterprise.Models.AccountingEntries;
using ERPCore.Enterprise.Models.Transactions;
using Microsoft.EntityFrameworkCore;
using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.Documents;
using System.IO;

namespace ERPCore.Enterprise.Repository.Documents
{

    public class Documents : ERPNodeDalRepository
    {
        public Documents(Organization organization) : base(organization)
        {

        }

        public List<Document> ListAll => erpNodeDBContext.Documents.ToList();

        public IQueryable<Document> Query => erpNodeDBContext.Documents;

        public Document Find(Guid id) => erpNodeDBContext.Documents.Find(id);

        public Document CreateNew(Document template)
        {
            template.Id = Guid.NewGuid();
            erpNodeDBContext.Documents.Add(template);
            this.SaveChanges();

            return template;
        }

        public void Delete(Guid id)
        {
            var ExistFile = this.erpNodeDBContext.Documents.Find(id);
            this.erpNodeDBContext.Documents.Remove(ExistFile);
            this.SaveChanges();
        }


        public void ReOrder()
        {
            int i = 0;
            erpNodeDBContext.Documents.OrderBy(d => d.DocumentDate).ToList().ForEach(d =>
            {
                d.No = ++i;
            });
            organization.SaveChanges();

        }
        public void Add(Document document)
        {
            erpNodeDBContext.Documents.Add(document);
        }
    }
}