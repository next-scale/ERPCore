
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Models.Items
{
    public class ItemGroup : Item
    {
        public ItemGroup()
        {
            Id = Guid.NewGuid();
        }

        public void SetParent(ItemGroup parent)
        {
            this.ParentId = parent.Id;
        }

        public void UpdateGroupData(ItemGroup group)
        {
            this.PartNumber = group.PartNumber;
            this.Description = group.Description;
            this.ParentId = group.ParentId;
        }

      
    }
}