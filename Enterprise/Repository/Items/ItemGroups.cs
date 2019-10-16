using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.Items;
using ERPCore.Enterprise.Models.Items.Enums;
using ERPCore.Enterprise.Models.Transactions.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPCore.Enterprise.Repository.Items
{
    public class ItemGroups : ERPNodeDalRepository
    {
        public ItemGroups(Organization organization) : base(organization)
        {

        }

        public IQueryable<ItemGroup> GetAll => erpNodeDBContext.ItemGroups;
        public List<ItemGroup> ListAll => GetAll.ToList();
        public List<ItemGroup> ListRoot()
        {
            return erpNodeDBContext.ItemGroups.Where(ig => ig.ParentId == null).ToList();
        }
        public ItemGroup Find(Guid id) => erpNodeDBContext.ItemGroups.Find(id);

        public ItemGroup ItemGroup(ItemGroup group)
        {
            var exItemGroup = erpNodeDBContext.ItemGroups.Find(group.Id);

            if (exItemGroup != null)
            {
                exItemGroup.PartNumber = group.PartNumber.Trim();
                exItemGroup.Description = group.Description.Trim();
                erpNodeDBContext.SaveChanges();
            }

            return exItemGroup;
        }

        public ItemGroup Create(string name, Guid? parentId)
        {
            var group = new ItemGroup()
            {
                Id = Guid.NewGuid(),
                PartNumber = name ?? "NA-Group",
                ParentId = parentId,
                ItemType = ItemTypes.Group,
                CreatedDate = DateTime.Now,

            };
            erpNodeDBContext.ItemGroups.Add(group);

            this.SaveChanges();
            return group;
        }



        public void Delete(Guid id)
        {
            var itemGroup = erpNodeDBContext.ItemGroups.Find(id);
            erpNodeDBContext.ItemGroups.Remove(itemGroup);

            this.SaveChanges();
        }

        public void UpdateLevel()
        {
            organization.ItemGroups.ListAll
                .ForEach(i => i.Level = 0);
            organization.SaveChanges();

            var root = organization.ItemGroups.ListRoot();
            Console.WriteLine("Update Item group Level " + root.Count + "  Group Count");
            root.ForEach(g => g.AssignLevel(0));

            this.SaveChanges();
        }
    }
}
