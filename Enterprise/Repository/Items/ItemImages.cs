

using ERPCore.Enterprise.Models.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPCore.Enterprise.Repository.Items
{
    public class ItemImages : ERPNodeDalRepository
    {
        public ItemImages(Organization organization) : base(organization)
        {

        }

        public IQueryable<ItemImage> Query => erpNodeDBContext.ItemImages;
        public List<ItemImage> ListAll => erpNodeDBContext.ItemImages.ToList();
        public ItemImage Find(Guid id) => erpNodeDBContext.ItemImages.Find(id);

        public void Delete(Guid id)
        {
            var itemImage = erpNodeDBContext.ItemImages.Find(id);

            erpNodeDBContext.ItemImages.Remove(itemImage);
            organization.SaveChanges();
        }

        public ItemImage CreateNew(ItemImage itemImage)
        {
            itemImage.Id = Guid.NewGuid();
            erpNodeDBContext.ItemImages.Add(itemImage);
            return itemImage;
        }
        public ItemImage CreateNew(string name)
        {
            var itemImage = new ItemImage()
            {
                Id = Guid.NewGuid(),
                Name = name
            };

            erpNodeDBContext.ItemImages.Add(itemImage);
            erpNodeDBContext.SaveChanges();
            return itemImage;
        }
    }
}
