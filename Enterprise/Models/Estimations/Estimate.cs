using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPKeeper;
using ERPCore.Enterprise.Models.Transactions.Commercials;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;


namespace ERPCore.Enterprise.Models.Estimations
{
    [Table("ERP_Estimations")]
    public partial class Estimate
    {
        [Key]
        public Guid Id { get; set; }
        public TransactionTypes TransactionType { get; set; }
        public string DocumentCode => Transactions.TransactionHelper.TrCode(this.TransactionType);
        public int No { get; set; }
        public string DocumentGroup => this.TransactionDate.ToString("yyMM");
        public string Name =>
            string.Format("{0}/{1}/{2}", DocumentCode, DocumentGroup, No.ToString().PadLeft(3, '0'));

        [Column(TypeName = "Date")]
        public DateTime TransactionDate { get; set; }
        public int ExpiredInDayCount { get; set; }

        public DateTime? CloseDate { get; set; }

        public int Age => (int)((CloseDate ?? DateTime.Today) - TransactionDate).TotalDays;
      


        public Enums.EstimateStatus Status { get; set; }
        public String Reference { get; set; }
        public String Memo { get; set; }

        public Enums.EstimationType Type { get; set; }

        public Guid? ProjectGuid { get; set; }
        [ForeignKey("ProjectGuid")]
        public virtual Models.Projects.Project Project { get; set; }

        public Guid? ProfileGuid { get; set; }
        [ForeignKey("ProfileGuid")]
        public virtual ERPCore.Enterprise.Models.Profiles.Profile Profile { get; set; }
        public string ProfileName { get; set; }


        public Guid? ProfileAddressGuid { get; set; }
        [ForeignKey("ProfileAddressGuid")]
        public virtual ERPCore.Enterprise.Models.Profiles.ProfileAddress ProfileAddress { get; set; }


        /// <summary>
        /// Profile of Self Organization
        /// </summary>
        public Guid? SelfProfileGuid { get; set; }
        [ForeignKey("SelfProfileGuid")]
        public virtual ERPCore.Enterprise.Models.Profiles.Profile SelfProfile { get; set; }


        public Guid? TaxCodeGuid { get; set; }
        [ForeignKey("TaxCodeGuid")]
        public virtual Models.Taxes.TaxCode TaxCode { get; set; }

        public Guid? PaymentTermGuid { get; set; }
        [ForeignKey("PaymentTermGuid")]
        public virtual PaymentTerm PaymentTerm { get; set; }


        public Guid? ShippingTermGuid { get; set; }
        [ForeignKey("ShippingTermGuid")]
        public virtual ShippingTerm ShippingTerm { get; set; }

        public Guid? ShippingMethodGuid { get; set; }
        [ForeignKey("ShippingMethodGuid")]
        public virtual ShippingMethod ShippingMethod { get; set; }

        public virtual ICollection<EstimateItem> Items { get; set; }


        [NotMapped]
        public List<EstimateItem> ExportItems => this.Items.ToList();
            //.Where(c => c.UnitPrice != 0)
            //.ToList();

        public String ItemsString
        {
            get
            {
                string items = "";

                foreach (var tri in this.Items.ToList())
                {
                    items = items + String.Format("{0} x {1}" + Environment.NewLine, tri.Amount, tri.ItemPartNumber);
                }
                return items;
            }
        }

        public void RemoveItem(EstimateItem existEstItem) => this.Items.Remove(existEstItem);

        public decimal LinesTotal { get; set; }



        public decimal Tax { get; set; }
        public decimal Total { get; set; }

        public String TotalThaiCurrencyString => new ERPKeeper.Helpers.Currency.Thai.Baht().Process(Total);


        [DefaultValue(false)]
        public bool IsPaymentComplete { get; set; }

        public string PaymentMemo { get; set; }
        public DateTime? PaymentDate { get; set; }


        [Column("PaymentAssetAccountId")]
        public Guid? AssetAccountId { get; set; }
        [ForeignKey("AssetAccountId")]
        public virtual ERPCore.Enterprise.Models.ChartOfAccount.Account AssetAccount { get; set; }



        public void Calculate()
        {

            this.ProfileName = this.Profile?.Name ?? this.ProfileName;
            this.LinesTotal = Items?.Sum(estimateItem => estimateItem.LineTotal) ?? 0;
            this.Tax = this.TaxCode?.GetExcludeTaxBalance(this.TransactionDate, this.LinesTotal) ?? 0;
            this.Total = this.LinesTotal + this.Tax;
        }


        public void ReOrder()
        {
            int i = 0;
            Items.OrderBy(item => item.Order).ToList().ForEach(item =>
              {
                  item.Order = ++i;
              });
        }

        public void AddItem(Items.Item item, int amount)
        {
            if (Items == null)
                Items = new HashSet<EstimateItem>();
            int order = Items.Count + 1;
            var estimateItem = new EstimateItem()
            {
                Id = Guid.NewGuid(),
                ItemGuid = item.Id,
                Amount = amount,
                ItemPartNumber = item.PartNumber,
                ItemDescription = item.Description,
                UnitPrice = item.UnitPrice,
                Order = order,
            };

            Items.Add(estimateItem);
            this.Calculate();
        }
    }
}
