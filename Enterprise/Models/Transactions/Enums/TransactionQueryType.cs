using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Models.Transactions
{
    public enum TransactionQueryType
    {
        All = 0,
        Lastest100 = 20,
        Status = 30,
        Time = 40,
        Overdue = 50
    }
}

