
using ERPCore.Enterprise.Models.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPCore.Enterprise.Repository.Items
{
    public class Brands : ERPNodeDalRepository
    {
        public Brands(Organization organization) : base(organization)
        {

        }

        public IQueryable<Brand> Query => erpNodeDBContext.Brands;
        public List<Brand> ListAll => erpNodeDBContext.Brands.ToList();
        public Brand Find(Guid id) => erpNodeDBContext.Brands.Find(id);

        public void Delete(Guid id)
        {
            var brand = erpNodeDBContext.Brands.Find(id);

            erpNodeDBContext.Brands.Remove(brand);
            organization.SaveChanges();
        }

        public Brand CreateNew(Brand brand)
        {
            brand.Id = Guid.NewGuid();
            erpNodeDBContext.Brands.Add(brand);
            return brand;
        }
        public Brand CreateNew(string name)
        {
            var brand = new Brand();
            brand.Id = Guid.NewGuid();
            brand.Name = name;

            erpNodeDBContext.Brands.Add(brand);
            erpNodeDBContext.SaveChanges();
            return brand;
        }
    }
}
