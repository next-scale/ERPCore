using ERPCore.Enterprise.Models.ChartOfAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPCore.Enterprise.Models.Accounting.Reports
{
    public class BalanceSheet
    {
        public DateTime Date { get; set; }


        public List<Account> AssetAccounts { get; set; }
        public List<Account> LiabilityAccounts { get; set; }
        public List<Account> EquityAccounts { get; set; }

    }
}
