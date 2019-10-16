
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Models.Items.Enums
{
    public enum ItemTypes
    {
        Service = 10,
        NonInventory = 20,
        Inventory = 30,
        Asset = 50,
        Expense = 100,
        Group = 999
    }
}