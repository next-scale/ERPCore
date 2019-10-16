using ERPCore.Enterprise.Models.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Models.Taxes;
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Accounting.Enums;

namespace ERPCore.Enterprise.Repository.Taxes
{
    public class TaxGroups : ERPNodeDalRepository
    {

        public TaxGroups(Organization organization) : base(organization)
        {

        }


        public List<TaxGroup> ListAll => erpNodeDBContext.TaxGroups.ToList();
        public IQueryable<TaxGroup> All => erpNodeDBContext.TaxGroups;

        public TaxGroup Find(Guid? TaxGroupGuid)
        {
            if (TaxGroupGuid == null)
                return null;
            else
                return erpNodeDBContext.TaxGroups.Find(TaxGroupGuid);
        }

        public TaxGroup CreateNew(string name)
        {
            if (name == null)
                name = "New TaxGroup";

            var newTaxGroup = new TaxGroup()
            {
                Id = Guid.NewGuid(),
                Name = name
            };

            erpNodeDBContext.TaxGroups.Add(newTaxGroup);
            organization.SaveChanges();
            return newTaxGroup;
        }

        internal TaxGroup GetDefault => erpNodeDBContext.TaxGroups
            .Where(t => t.isDefault)
            .FirstOrDefault();

        public void Remove(TaxGroup taxCode)
        {
            erpNodeDBContext.TaxGroups.Remove(taxCode);
            organization.SaveChanges();
        }
    }
}