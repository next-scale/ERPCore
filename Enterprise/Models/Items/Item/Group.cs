using ERPCore.Enterprise.Models.ChartOfAccount;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ERPCore.Enterprise.Models.Items
{
    public partial class Item
    {

        public Guid? ParentId { get; set; }
        [ForeignKey("ParentId")]

        public virtual Item Parent { get; set; }


        public virtual ICollection<Item> Child { get; set; }
        public virtual int ChildCount => this.Child?.Count ?? 0;

        public virtual int ChildItemsCount => this.Child?.Where(c => c.ItemType != Enums.ItemTypes.Group).Count() ?? 0;
        public virtual int ChildGroupsCount => this.Child?.Where(c => c.ItemType == Enums.ItemTypes.Group).Count() ?? 0;
        public bool IsChild(ItemGroup item)
        {
            var isChild = false;
            foreach (var child in this.Child.ToList())
            {
                if (child.Id == item.Id)
                    return true;
                else
                    isChild = child.IsChild(item);
            };
            return isChild;
        }
    }
}