
using ERPCore.Enterprise.Models.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPCore.Enterprise.Repository.Items
{
    public class ItemParameters : ERPNodeDalRepository
    {
        public ItemParameters(Organization organization) : base(organization)
        {

        }

        public IQueryable<ItemParameter> Query => erpNodeDBContext.ItemParameters;
        public List<ItemParameter> ListAll => erpNodeDBContext.ItemParameters.ToList();
        public ItemParameter Find(Guid id) => erpNodeDBContext.ItemParameters.Find(id);

        public void Delete(Guid id)
        {
            var itemParameter = erpNodeDBContext.ItemParameters.Find(id);

            erpNodeDBContext.ItemParameters.Remove(itemParameter);
            organization.SaveChanges();
        }

        public ItemParameter CreateNew(ItemParameter itemParameter)
        {
            itemParameter.Id = Guid.NewGuid();
            erpNodeDBContext.ItemParameters.Add(itemParameter);
            return itemParameter;
        }
        public ItemParameter CreateNew(string name)
        {
            var itemParameter = new ItemParameter();
            itemParameter.Id = Guid.NewGuid();

            erpNodeDBContext.ItemParameters.Add(itemParameter);
            erpNodeDBContext.SaveChanges();
            return itemParameter;
        }
    }
}
