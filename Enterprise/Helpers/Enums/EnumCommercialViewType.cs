using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ERPKeeper.Helpers.Enums
{
    public enum EnumCommercialViewType
    {
        Default = 0,
        Open = 10,
        Void = 20,
        Close = 22,
        PartialPaid = 30,
        Quote = 40,
        Request = 45,
        All = 50,
        OverDue = 60,
        ThisMonth = 70,
        ThisYear = 80,
        LastYear = 90,
        LastMonth = 100,
        Ordered = 110,
        Complete = 120
    }
}
