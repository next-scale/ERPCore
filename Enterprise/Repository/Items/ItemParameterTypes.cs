
using ERPCore.Enterprise.Models.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPCore.Enterprise.Repository.Items
{
    public class ItemParameterTypes : ERPNodeDalRepository
    {
        public ItemParameterTypes(Organization organization) : base(organization)
        {

        }

        public IQueryable<ItemParameterType> Query => erpNodeDBContext.ItemParameterTypes;
        public List<ItemParameterType> ListAll => erpNodeDBContext.ItemParameterTypes.ToList();
        public ItemParameterType Find(Guid id) => erpNodeDBContext.ItemParameterTypes.Find(id);

        public void Delete(Guid id)
        {
            var itemParameterType = erpNodeDBContext.ItemParameterTypes.Find(id);

            erpNodeDBContext.ItemParameterTypes.Remove(itemParameterType);
            organization.SaveChanges();
        }

        public ItemParameterType CreateNew(ItemParameterType itemParameterType)
        {
            itemParameterType.Id = Guid.NewGuid();
            erpNodeDBContext.ItemParameterTypes.Add(itemParameterType);
            return itemParameterType;
        }
        public ItemParameterType CreateNew(string name)
        {
            var newParameterType = new ItemParameterType()
            {
                Id = Guid.NewGuid(),
                Name = name?.Trim()
            };

            erpNodeDBContext.ItemParameterTypes.Add(newParameterType);
            erpNodeDBContext.SaveChanges();
            return newParameterType;
        }
    }
}
