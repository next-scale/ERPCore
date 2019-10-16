using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace ERPCore.Enterprise.Models.Estimations
{

    [Table("ERP_Transactions_Estimation_Taxes")]
    public class EstimationTax
    {
        [Key]
        public Guid Id { get; set; }


        [Index]
        public Guid? TaxCodeId { get; set; }
        [ForeignKey("TaxCodeId")]
        public virtual Taxes.TaxCode TaxCode { get; set; }


        public Guid? EstimateId { get; set; }
        [ForeignKey("EstimateId")]
        public virtual Estimate Estimate { get; set; }

        public virtual String Name => this.Estimate.Name;
        public virtual String ProfileName => this.Estimate.Profile.Name;
        public virtual String ProfileTaxId => this.Estimate.Profile.TaxNumber;

        public Taxes.Enums.TaxDirection TaxDirection { get; set; }

        [DisplayFormat(DataFormatString = "N2")]
        public Decimal TaxBalance { get; set; }


        [DisplayFormat(DataFormatString = "N2")]
        public Decimal EstimateBalance { get; set; }


        public EstimationTax()
        {
            Id = Guid.NewGuid();
        }

        public void UpdateTaxBalance()
        {
            if (this.TaxCode != null)
            {
                switch (this.Estimate.TransactionType)
                {
                    case Accounting.Enums.TransactionTypes.Purchase:
                    case Accounting.Enums.TransactionTypes.SalesReturn:
                        this.TaxDirection = Taxes.Enums.TaxDirection.Input;
                        break;
                    case Accounting.Enums.TransactionTypes.Sale:
                    case Accounting.Enums.TransactionTypes.PurchaseReturn:
                        this.TaxDirection = Taxes.Enums.TaxDirection.Output;
                        break;
                }

                this.EstimateBalance = this.Estimate.LinesTotal;
                this.TaxBalance = this.TaxCode?.GetExcludeTaxBalance(Estimate.TransactionDate, Estimate.LinesTotal) ?? 0;
            }
        }
    }
}
