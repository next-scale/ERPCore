using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.ChartOfAccount;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ERPCore.Enterprise.Models.Transactions;
using ERPCore.Enterprise.Models.Taxes.Enums;

namespace ERPCore.Enterprise.Models.Taxes
{
    [NotMapped]
    public class CommercialTaxGroup
    {
        public Guid? TaxCodeId { get; internal set; }
        public TaxCode TaxCode { get; internal set; }
        public decimal TaxBalance { get; internal set; }
        public int CommercialCount { get; internal set; }
    }

    [Table("ERP_Taxes_Periods")]
    public class TaxPeriod
    {
        [Key]
        public Guid Id { get; set; }
        public int? No { get; set; }
        public const TransactionTypes TransactionType = TransactionTypes.TaxPeriod;
        public Guid? FiscalYearId { get; set; }
        public DateTime PeriodStartDate =>
            new DateTime(TransactionDate.Year, TransactionDate.Month, 1);

        [Column("TrDate")]
        public DateTime TransactionDate { get; set; }

        public string Name => "TP/" + this.TransactionDate.ToString("yyMM");

        public String PeriodName =>
            TransactionDate.Year.ToString() + "-" + TransactionDate.Month.ToString().PadLeft(2, '0');

        public Guid? CloseToAccountGuid { get; set; }
        [ForeignKey("CloseToAccountGuid")]
        public virtual Account CloseToAccount { get; set; }

        public Guid? LiabilityPaymentId { get; set; }


        public String Memo { get; set; }
        public LedgerPostStatus PostStatus { get; set; }
        public virtual ICollection<CommercialTax> CommercialTaxes { get; set; }
        public virtual IEnumerable<CommercialTax> InputTaxCommercials =>
            this.CommercialTaxes.Where(c => c.TaxDirection == Enums.TaxDirection.Input)
            .OrderBy(c => c.Commercial.TransactionDate);

        public virtual IEnumerable<CommercialTax> OutputTaxCommercials =>
            this.CommercialTaxes.Where(c => c.TaxDirection == Enums.TaxDirection.Output)
            .OrderBy(c => c.Commercial.TransactionDate);

        public List<CommercialTaxGroup> GetCommercialTaxGroups()
        {


            var commercialTaxGroups = this.CommercialTaxes
                .GroupBy(c => c.TaxCodeId)
                .Select(go => new CommercialTaxGroup()
                {
                    TaxCodeId = go.Key,
                    TaxCode = go.Select(i => i.TaxCode).FirstOrDefault(),
                    TaxBalance = go.Sum(i => i.TaxBalance),
                    CommercialCount = go.Count(),
                })
                .ToList();


            //commercialTaxGroups.ForEach(tg =>
            //{
            //    Console.WriteLine($"##############################################");
            //    var commercials = this.CommercialTaxes.Where(ct => ct.TaxCodeId == tg.TaxCodeId).ToList();


            //    commercials.ForEach(c =>
            //    {
            //        Console.WriteLine($"{c.ProfileName} {c.TaxBalance}");
            //    });

            //});



            return commercialTaxGroups;
        }


        public int CommercialsCount { get; private set; }
        public decimal InputTaxBalance { get; private set; }
        public decimal OutputTaxBalance { get; private set; }
        public decimal ClosingAmount =>
            this.OutputTaxBalance - this.InputTaxBalance;
        public decimal TaxPayableBalance =>
            (ClosingAmount > 0) ? ClosingAmount : 0;
        public decimal TaxReceivableBalance =>
            (ClosingAmount < 0) ? Math.Abs(ClosingAmount) : 0;
        public List<CommercialTax> ListComercialTaxes(TaxDirection taxDirection)
        {
            return this.CommercialTaxes
                .Where(c => c.TaxDirection == taxDirection)
                .ToList();
        }
        public void RemoveInvalidCommercialTax()
        {
            this.CommercialTaxes
                 .Where(ct => ct.TaxCodeId == null || ct.CommercialId == null || !ct.TaxCode.isRecoverable)
                 .ToList()
                 .ForEach(c => this.CommercialTaxes.Remove(c));
        }
        public void ReCalculate()
        {
            this.RemoveInvalidCommercialTax();


            this.InputTaxBalance = this.CommercialTaxes
                .Where(c => c.TaxDirection == Enums.TaxDirection.Input)
                .Select(t => t.TaxBalance)
                .DefaultIfEmpty(0)
                .Sum();

            this.OutputTaxBalance = this.CommercialTaxes
                .Where(c => c.TaxDirection == Enums.TaxDirection.Output)
                .Select(t => t.TaxBalance)
                .DefaultIfEmpty(0)
                .Sum();

            this.CommercialsCount = this.CommercialTaxes.Count();

        }
        public void AddCommercialTax(CommercialTax comTax, bool reCalculate = true)
        {
            this.CommercialTaxes.Add(comTax);

            if (reCalculate)
                this.ReCalculate();
        }
    }
}
