using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPCore.Enterprise.Models.Accounting.ChartOfAccount
{
    public class FinancialBalance
    {
        public string Node { get; set; }
        public decimal AssetBalance { get; set; }
        public decimal LiabilityBalance { get; set; }
        public decimal EquityBalance { get; set; }

        public decimal IncomeBalance { get; set; }
        public decimal ExpenseBalance { get; set; }
    }
}
