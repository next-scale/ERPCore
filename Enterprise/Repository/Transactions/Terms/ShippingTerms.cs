
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Financial;
using ERPCore.Enterprise.Models.Financial.Transfer;
using ERPCore.Enterprise.Models.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Models.Employees;
using ERPCore.Enterprise.Models.Transactions.Commercials;

namespace ERPCore.Enterprise.Repository.Transactions.Terms
{
    public class ShippingTerms : ERPNodeDalRepository
    {
        public ShippingTerms(Organization organization) : base(organization)
        {

        }


        public List<ShippingTerm> ListAll => erpNodeDBContext.ShippingTerms.ToList();
        public ShippingTerm Find(Guid id) => erpNodeDBContext.ShippingTerms.Find(id);
        public IQueryable<ShippingTerm> Query => erpNodeDBContext.ShippingTerms;

        public ShippingTerm CreateNew(ShippingTerm term)
        {
            term.Id = Guid.NewGuid();
            erpNodeDBContext.ShippingTerms.Add(term);
            return term;
        }
    }
}