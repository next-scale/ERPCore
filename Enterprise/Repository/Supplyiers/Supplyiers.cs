
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Models.Profiles;
using ERPCore.Enterprise.Models.Profiles.Enums;
using ERPCore.Enterprise.Models.Suppliers;
using ERPCore.Enterprise.Models.Suppliers.Enums;

namespace ERPCore.Enterprise.Repository.Profiles
{
    public class Suppliers : ERPNodeDalRepository
    {
        public Suppliers(Organization organization) : base(organization)
        {

        }

        public IQueryable<Models.Profiles.Profile> All
        {
            get
            {
                return erpNodeDBContext.Suppliers
                   .Select(r => r.Profile);
            }
        }

        public IQueryable<Models.Suppliers.Supplier> GetByType(ProfileQueryType Type = ProfileQueryType.Active)
        {
            switch (Type)
            {
                case ProfileQueryType.Active:
                    return erpNodeDBContext.Suppliers
                   .Where(s => s.Status == SupplierStatus.Active);

                case ProfileQueryType.InActive:
                    return erpNodeDBContext.Suppliers
                   .Where(s => s.Status == Models.Suppliers.Enums.SupplierStatus.InActive);

                case ProfileQueryType.All:
                default:
                    return erpNodeDBContext.Suppliers;
            }
        }

        public Supplier Find(Guid id) => erpNodeDBContext.Suppliers.Find(id);

        public Supplier Create(Models.Profiles.Profile newSupplierProfile)
        {
            if (newSupplierProfile.Supplier == null)
            {
                newSupplierProfile.Supplier = new Supplier()
                {
                    Status = SupplierStatus.Active
                };
            }
            erpNodeDBContext.SaveChanges();
            return newSupplierProfile.Supplier;
        }
    }
}