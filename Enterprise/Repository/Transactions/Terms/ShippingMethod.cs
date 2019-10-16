
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
    public class ShippingMethods : ERPNodeDalRepository
    {
        public ShippingMethods(Organization organization) : base(organization)
        {

        }


        public List<ShippingMethod> ListAll => erpNodeDBContext.ShippingMethods.ToList();
        public ShippingMethod Find(Guid id) => erpNodeDBContext.ShippingMethods.Find(id);
        public IQueryable<ShippingMethod> Query => erpNodeDBContext.ShippingMethods;

        public ShippingMethod CreateNew(ShippingMethod term)
        {
            term.Id = Guid.NewGuid();
            erpNodeDBContext.ShippingMethods.Add(term);

            return term;
        }
    }
}