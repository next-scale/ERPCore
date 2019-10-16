using ERPCore.Enterprise.DataBase;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ERPCore.Enterprise.Repository
{
    public class ERPNodeDalRepository
    {
        public Organization organization { get; private set; }
        protected EnterpriseDbContext erpNodeDBContext { get; set; }
        public Models.Accounting.Enums.TransactionTypes transactionType { get; set; }
        public string trString => this.transactionType.ToString();
        public ERPNodeDalRepository(Organization organization)
        {
            this.organization = organization;
            this.erpNodeDBContext = organization.erpNodeDBContext;
        }

        public void SaveChanges()
        {
            erpNodeDBContext.SaveChanges();
        }
    }
}