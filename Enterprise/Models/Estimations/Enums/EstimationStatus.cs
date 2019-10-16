using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPCore.Enterprise.Models.Estimations.Enums
{

    public enum EstimateStatus
    {
        Quote = 0,
        Ordered = 1,
        Close = 2,
        Paid = 3,
        Void = 99

    }
}
