using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Models.Taxes
{
    [Table("ERP_Taxes_TaxCode")]
    public class TaxCode
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }
        public Guid? TaxGroupId { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public Enums.TaxDirection TaxDirection { get; set; }
        public bool isDefault
        { get; set; }
        public bool isRecoverable { get; set; }

        [Column("TaxRate")]
        public Decimal TaxRate { get; set; }



        public Guid? TaxAccountGuid { get; set; }
        [ForeignKey("TaxAccountGuid")]
        public virtual ChartOfAccount.Account TaxAccount { get; set; }

   
        public Guid? OutputTaxAccountGuid { get; set; }
        public Guid? InputTaxAccountGuid { get; set; }
        public Guid? AssignAccountGuid { get; set; }
        public Guid? CloseToAccountGuid { get; set; }



        public void UpdateCommertialTaxCount()
        {
            Console.Write("Update " + this.Name + " Count");
            this.CommercialCount = this.CommercialTaxes?.Count() ?? 0;
        }

 
        //Closing Account
        public Guid? TaxReceivableAccountGuid { get; set; }

        public Guid? TaxPayableAccountGuid { get; set; }
        [ForeignKey("TaxPayableAccountGuid")]
        public virtual ChartOfAccount.Account TaxPayableAccount { get; set; }

        public bool IsReady { get; set; }
        public int CommercialCount { get; set; }
        public virtual ICollection<Transactions.CommercialTax> CommercialTaxes { get; set; }

        public String Code
        { get; set; }

        public TaxCode()
        {
            this.Id = Guid.NewGuid();
        }

        public void Update(TaxCode taxCode)
        {
            this.Name = taxCode.Name;
            this.Code = taxCode.Code;
            this.TaxDirection = taxCode.TaxDirection;

            this.Description = taxCode.Description;
            this.TaxGroupId = taxCode.TaxGroupId;
            this.TaxRate = taxCode.TaxRate;
            this.isDefault = taxCode.isDefault;
            this.isRecoverable = taxCode.isRecoverable;
            this.TaxAccountGuid = taxCode.TaxAccountGuid;
            
        }

        public Decimal GetTaxRate(DateTime TransactionDate)
        {
            return TaxRate;
        }

        public Decimal GetExcludeTaxBalance(DateTime TransactionDate, Decimal amount)
        {
            var taxRate = this.GetTaxRate(TransactionDate);
            return decimal.Round(amount * taxRate / (decimal)100, 2, MidpointRounding.ToEven);
        }

        public Decimal GetIncludeAmount(DateTime TransactionDate, Decimal amount)
        {
            var taxRate = this.GetTaxRate(TransactionDate);
            return decimal.Round(amount * taxRate / ((decimal)100 + taxRate), 2, MidpointRounding.ToEven);
        }
    }
}
