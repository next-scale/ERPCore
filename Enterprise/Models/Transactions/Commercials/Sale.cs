using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERPCore.Enterprise.Models.ChartOfAccount;

namespace ERPCore.Enterprise.Models.Transactions.Commercials
{
    public class Sale : Commercial
    {
        
        public Sale()
        {
            Id = Guid.NewGuid();
            this.TransactionDate = DateTime.Today;
            this.Status = CommercialStatus.Open;
            this.TransactionDate = DateTime.Today;
            this.TransactionType = Accounting.Enums.TransactionTypes.Sale;
        }
        public void UpdateAddress()
        {
            if (this.ProfileAddress == null && this.Profile.PrimaryAddress != null)
            {
                this.ProfileAddress = this.Profile.PrimaryAddress;
                this.ProfileAddressGuid = this.Profile.PrimaryAddress.AddressGuid;
            }
        }
       

    }
}
