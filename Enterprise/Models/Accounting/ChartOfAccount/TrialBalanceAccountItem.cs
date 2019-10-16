using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPCore.Enterprise.Models.ChartOfAccount
{
    [NotMapped]
    public class TrialBalanceAccountItem
    {
        public Guid Id { get; set; }
     //   public Guid AccountId { get; set; }
        public Account accountItem { get; set; }
        public Decimal OpeningBalance { get; set; }
        public Decimal Credit { get; set; }
        public Decimal Debit { get; set; }
        public AccountTypes Type { get; set; }
        public AccountSubTypes? SubType { get; set; }


        public Decimal Balance
        {
            get
            {
                switch (accountItem.Type)
                {
                    case AccountTypes.Asset:
                    case AccountTypes.Expense:
                        return this.Debit - this.Credit;
                    default:
                        return this.Credit - this.Debit;
                }
            }
        }
    }
}
