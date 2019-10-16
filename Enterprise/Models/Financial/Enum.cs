using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Models.Financial
{
    public enum AccountType
    {
        Saving = 0,
        Current = 1,
        Other = 2
    }
}