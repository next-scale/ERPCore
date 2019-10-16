using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Models.Transactions
{

    [Table("ERP_Transactions_Commercial_Taxes")]
    public class CommercialTax
    {
        [Key]
        public Guid Id { get; set; }


        [Index]
        public Guid? TaxCodeId { get; set; }
        [ForeignKey("TaxCodeId")]
        public virtual Taxes.TaxCode TaxCode { get; set; }




        [Column("CommercialId")]
        public Guid? CommercialId { get; set; }
        [ForeignKey("CommercialId")]
        public virtual Commercial Commercial { get; set; }

        public virtual String Name => this.Commercial.Name;
        public virtual String ProfileName => this.Commercial.Profile.Name;
        public virtual String ProfileTaxId => this.Commercial.Profile.TaxNumber;



        [Column("TaxPeriodId")]
        public Guid? TaxPeriodId { get; set; }
        [ForeignKey("TaxPeriodId")]
        public virtual Models.Taxes.TaxPeriod TaxPeriod { get; set; }


        public Taxes.Enums.TaxDirection TaxDirection { get; set; }

        [DisplayFormat(DataFormatString = "N2")]
        [Column("TaxBalance")]
        public Decimal TaxBalance { get; set; }


        [DisplayFormat(DataFormatString = "N2")]
        [Column("CommercialBalance")]
        public Decimal CommercialBalance { get; set; }



        public CommercialTax()
        {
            Id = Guid.NewGuid();
        }

        public void UpdateTaxBalance(decimal offset = 0)
        {
            if (this.TaxCode != null)
            {
                switch (this.Commercial.TransactionType)
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

                this.CommercialBalance = this.Commercial.LinesTotal;

                this.TaxBalance = offset + (this.TaxCode?.GetExcludeTaxBalance(Commercial.TransactionDate, Commercial.LinesTotal) ?? 0);
            }
        }
    }
}
