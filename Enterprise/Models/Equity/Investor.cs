using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPCore.Enterprise.Models.Equity
{
    [Table("ERP_Investors")]
    public class Investor
    {
        [Key, ForeignKey("Profile")]
        public Guid ProfileId { get; set; }

        public virtual Profiles.Profile Profile { get; set; }

        public int StockAmount { get; set; }

        public Enums.InvestorStatus Status { get; set; }

        public virtual ICollection<CapitalActivity> CapitalActivities { get; set; }



        public void UpdateStockCount()
        {
            if (CapitalActivities == null)
                this.StockAmount = 0;
            else
                this.StockAmount = this.CapitalActivities.ToList().Sum(c => c.StockAmount);


            if (StockAmount == 0)
                this.Status = Enums.InvestorStatus.InActive;
            else
                this.Status = Enums.InvestorStatus.Active;

        }
    }
}
