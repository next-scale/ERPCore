using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPCore.Enterprise.Models.Accounting.Enums
{
    public enum LedgerPostStatus
    {
        ReadyToPost = 0,
        Posted = 9,
        PreOpening = 10
    }
}
