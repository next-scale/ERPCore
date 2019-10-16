using System;
using ERPCore.Enterprise.Models.ChartOfAccount;

namespace ERPCore.Enterprise.Models.Accounting
{
    public class TempClosingEntry
    {
        public Guid Id { get; set; }
        public Guid FiscalYearId { get; set; }
        public Guid AccountGuid { get; set; }
        public Account Account { get; set; }



        public decimal OpeningCredit { get; set; }
        public decimal OpeningDebit { get; set; }






        public decimal Credit { get; set; }
        public decimal Debit { get; set; }




    }
}