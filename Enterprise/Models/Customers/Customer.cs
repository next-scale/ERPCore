using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPCore.Enterprise.Models.Customers
{
    [Table("ERP_Customers")]
    public class Customer
    {
        [Key, ForeignKey("Profile")]
        public Guid ProfileId { get; set; }
        public virtual Profiles.Profile Profile { get; set; }
        public string Code { get; set; }
        public Enums.CustomerStatus Status { get; set; }



        public Guid? DefaultTaxCodeId { get; set; }
        [ForeignKey("DefaultTaxCodeId")]
        public virtual Taxes.TaxCode DefaultTaxCode { get; set; }



        public Decimal CreditLimit { get; set; }
        public int CreditAgeLimit { get; set; }
        public int CountSales { get; set; }

        [Column("TotalSale")]
        public Decimal SumSaleBalance { get; set; }

        [Column("TotalBalance")]
        public Decimal TotalBalance { get; set; }
        public decimal CountBalance { get; set; }

        public void SetActive()
        {
            this.Status = Enums.CustomerStatus.Active;
        }

        public void Update(Customer customer)
        {
            this.DefaultTaxCodeId = customer.DefaultTaxCodeId;
            this.CreditAgeLimit = customer.CreditAgeLimit;
            this.CreditLimit = customer.CreditLimit;
        }
    }
}
