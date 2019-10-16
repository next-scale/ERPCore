using ERPCore.Enterprise.Models.ChartOfAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPCore.Enterprise.Models.Accounting.Reports
{
    public class IncomeStatement
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Account> Incomes { get; set; }
        public List<Account> Expenses { get; set; }
        public Decimal Profit { get; set; }
    }
}
