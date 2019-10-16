using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ERPCore.Enterprise.Helpers
{
    public enum QueryPeriod
    {
        Default = 0,
        All = 10,
        ThisMonth = 20,
        ThisYear = 30,
        LastYear = 40,
        LastMonth = 50
    }
}
