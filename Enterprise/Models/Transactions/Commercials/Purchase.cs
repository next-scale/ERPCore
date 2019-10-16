using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERPCore.Enterprise.Models.ChartOfAccount;
using System.ComponentModel;

namespace ERPCore.Enterprise.Models.Transactions.Commercials
{
    public class Purchase : Commercial
    {
  

        public Purchase()
        {
            this.Id = Guid.NewGuid();
            this.TransactionDate = DateTime.Today;
            this.Status = CommercialStatus.Open;
            this.TransactionType = Accounting.Enums.TransactionTypes.Purchase;
        }
    

    
    }
}
