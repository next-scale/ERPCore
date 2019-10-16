using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Items.Enums;
using ERPCore.Enterprise.Models.Logistic;
using ERPCore.Enterprise.Models.Logistic.Enum;
using ERPCore.Enterprise.Models.Taxes;
using ERPCore.Enterprise.Models.Transactions.Commercials;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;


namespace ERPCore.Enterprise.Models.Transactions
{
    [Table("ERP_Transactions_Commercial")]
    public partial class Commercial
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }
        public TransactionTypes TransactionType { get; set; }

        public String QRId => Id.ToString("D");
        public String QRDisplay => Id.ToString("D").Replace("-", Environment.NewLine);
        public CommercialStatus Status { get; set; }
        public String Log { get; set; }
        public String Memo { get; set; }
        public bool IsOnline { get; set; }
        public int No { get; set; }
        public string Name =>
            string.Format("{0}/{1}/{2}", DocumentCode, DocumentGroup, No.ToString().PadLeft(3, '0'));
        public string PartialName =>
            string.Format("{0}/{1}", DocumentGroup, No.ToString().PadLeft(3, '0'));
        public string DocumentGroup => TransactionDate.ToString("yyMM");
        public string DocumentCode => TransactionHelper.TrCode(TransactionType);




        public Guid? FiscalYearId { get; set; }
        public Guid? ResponsibleProfileGuid { get; set; }

        [DefaultValue(Enums.PurchasePurposes.Use)]
        public Enums.PurchasePurposes PurchasePurposes { get; set; }

        public Guid? CompanyProfileGuid { get; set; }
        [ForeignKey("CompanyProfileGuid")]
        public virtual ERPCore.Enterprise.Models.Profiles.Profile CompanyProfile { get; set; }

        public Guid? CompanyProfileAddressGuid { get; set; }
        [ForeignKey("CompanyProfileAddressGuid")]
        public virtual ERPCore.Enterprise.Models.Profiles.ProfileAddress CompanyProfileAddress { get; set; }

        [Column("TrDate", TypeName = "Date")]
        public DateTime TransactionDate { get; set; }

        public String ThaiTransactionDate => TransactionDate.ToString("dd-MMM-yy", new CultureInfo("th-TH"));
        public String ThaiCloseDate => (CloseDate ?? DateTime.Today).ToString("dd-MMM-yy", new CultureInfo("th-TH"));
        public int Age => (int)((CloseDate ?? DateTime.Today) - TransactionDate).TotalDays;
 
        public Guid? ProfileGuid { get; set; }
        [ForeignKey("ProfileGuid")]
        public virtual Profiles.Profile Profile { get; set; }

        public string ProfileName { get; set; }

        public Guid? ProfileAddressGuid { get; set; }
        [ForeignKey("ProfileAddressGuid")]
        public virtual Profiles.ProfileAddress ProfileAddress { get; set; }

        public string CacheProfileName { get; set; }
        public string CacheProfileAddress { get; set; }

        public Guid? ProjectGuid { get; set; }
        [ForeignKey("ProjectGuid")]
        public virtual Projects.Project Project { get; set; }

        [MaxLength(200)]
        public String ReferenceCore { get; set; }

        [MaxLength(200)]
        public String Reference { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public bool DocumentComplete { get; set; }

        public DateTime? ExpiredDate { get; set; }

        public double CloseAge => ((CloseDate ?? DateTime.Today) - TransactionDate).TotalDays;

        public DateTime? DeliveryDate { get; set; }
        public Decimal LinesTotal { get; set; }
        public Decimal Tax { get; set; }
        public Decimal TaxOffset { get; set; }
        public Decimal SubTotal => LinesTotal;
        public Decimal Total { get; set; }
        public String TotalTHB { get; set; }
        public String ThaiCurrencyString => new ERPKeeper.Helpers.Currency.Thai.Baht().Process(Total);

        public Decimal? WriteOff { get; set; }

        public DateTime? EarnestDate { get; set; }

        
        public Decimal EarnestAmount { get; set; }

        public Decimal TotalBalance => Total - TotalPayment;
        public Decimal TotalPayment
        {
            get
            {
                if (Status == CommercialStatus.Paid)
                    return Total;
                else
                    return 0;
            }
        }

        public Guid? PaymentTermGuid { get; set; }
        [ForeignKey("PaymentTermGuid")]
        public virtual Commercials.PaymentTerm PaymentTerm { get; set; }

        public Guid? ShippingTermGuid { get; set; }
        [ForeignKey("ShippingTermGuid")]
        public virtual Commercials.ShippingTerm ShippingTerm { get; set; }


        public Guid? ShippingMethodGuid { get; set; }
        [ForeignKey("ShippingMethodGuid")]
        public virtual Commercials.ShippingMethod ShippingMethod { get; set; }

        public LedgerPostStatus PostStatus { get; set; }

        public String DocumentTypeName => "ใบส่งของ/ใบแจ้งหนี้/ใบกำกับภาษี";

        [Column("CommercialPaymentId")]
        public Guid? CommercialPaymentId { get; set; }
        [ForeignKey("CommercialPaymentId")]
        public virtual Financial.Payments.GeneralPayment CommercialPayment { get; set; }

        [Column(TypeName = "Date")]
        public DateTime? CloseDate { get; set; }

        [Column("PaymentAssetAccount")]
        public Guid? AssetAccountId { get; set; }
        [ForeignKey("AssetAccountId")]
        public virtual Account AssetAccount { get; set; }


        public virtual ICollection<CommercialItem> CommercialItems { get; set; }
        public virtual ICollection<CommercialTax> CommercialTaxes { get; set; }
        public virtual ICollection<Shipment> Shipments { get; set; }


        public void Update(Commercial commercial)
        {
            if (PostStatus == LedgerPostStatus.Posted)
                throw new Exception("Transaction Posted");


            Reference = commercial.Reference;
            Memo = commercial.Memo;

            ProjectGuid = commercial.ProjectGuid;
            TransactionDate = commercial.TransactionDate;
            PaymentTermGuid = commercial.PaymentTermGuid;
            ShippingTermGuid = commercial.ShippingTermGuid;
            No = commercial.No;
            ProfileName = Profile?.Name ?? ProfileName;


        }


        public void ReCalculate()
        {
            Console.WriteLine("> Calculate");
            this.ProfileName = Profile?.Name ?? this.ProfileName;

            this.ReCalculateItems();
            this.ReCalculateTax();

            this.Total = this.SubTotal + this.Tax;
            this.UpdatePayment();
        }


        public void ReCalculateItems()
        {
            this.SortItemsOrder();
            this.LinesTotal = CommercialItems?.ToList().Sum(i => i.LineTotal) ?? 0;
        }


        public void UpdatePayment()
        {
            if (CommercialPaymentId != null)
            {
                Status = CommercialStatus.Paid;
                CloseDate = CommercialPayment.TransactionDate;
            }
            else
            {
                Status = CommercialStatus.Open;
                CommercialPayment = null;
                CloseDate = null;
            }
        }

        public bool RemoveCommercialTaxs()
        {
            if (PostStatus == LedgerPostStatus.Posted)
                throw new Exception("Transaction is posted");

            var CommercialTaxs = CommercialTaxes.ToList();

            CommercialTaxs.ForEach(ct =>
            {
                if (ct.TaxPeriodId != null)
                    throw new Exception("Transaction is areardy assign to tax period");
                else
                    CommercialTaxes.Remove(ct);
            });
            ReCalculate();

            return true;
        }

        public bool RemoveCommercialTax(TaxCode taxCode)
        {
            if (taxCode == null)
                throw new NotImplementedException("TaxCode is null");


            var existCommercialTax = CommercialTaxes
                .Where(ct => ct.TaxCodeId == taxCode.Id)
                .FirstOrDefault();


            if (existCommercialTax != null && existCommercialTax.TaxPeriodId == null)
            {
                CommercialTaxes.Remove(existCommercialTax);

                ReCalculate();
                return true;
            }

            throw new NotImplementedException("Cannot Remove");
        }

        public void RemovePayment()
        {
            if (PostStatus == LedgerPostStatus.Posted)
                throw new NotImplementedException("Cannot Remove payment ,Transaction is posted");
            else
            {
                CloseDate = null;
                Status = CommercialStatus.Open;

                if (CommercialPayment != null)
                    CommercialPayment.RemoveCommercial(this);
            }
        }

        [NotMapped]
        public virtual List<CommercialItem> ExportCommercialItems => CommercialItems.ToList()
                   // .Where(c => c.UnitPrice != 0)
                      .ToList();
        public void SortItemsOrder()
        {
            var commecialItems = CommercialItems.OrderBy(item => item.Order)
                .ToList();

            int i = 1;
            foreach (var item in commecialItems)
            {
                item.Order = i;
                i++;
            }
        }
        public CommercialItem AddItem(Items.Item item, int amount)
        {
            Log = string.Format("{0}{1}{2}", Log, "> Add item " + item.PartNumber.ToString(), Environment.NewLine);

            if (CommercialItems == null)
                CommercialItems = new HashSet<CommercialItem>();

            if (!ValidateItemType(item))
                return null;
            int order = CommercialItems.Count + 1;
            var commecialItem = new CommercialItem()
            {
                Id = Guid.NewGuid(),
                ItemGuid = item.Id,
                Item = item,
                ItemPartNumber = item.PartNumber,
                ItemDescription = item.Description,
                UnitPrice = item.UnitPrice,
                Amount = amount,
                TransactionType = TransactionType,
                Order = order
            };

            CommercialItems.Add(commecialItem);

            return commecialItem;
        }
        public Shipment CreateShipment(DateTime shipDate, string trackingNo, ShippingTerm term = null)
        {
            if (term == null)
                term = ShippingTerm;

            if (Shipments == null)
                Shipments = new HashSet<Shipment>();

            var shipment = new Shipment()
            {
                Id = Guid.NewGuid(),
                ShipDate = shipDate,
                TrackingNo = trackingNo,
                Weight = 0,
                Status = ShipmentStatus.Prepare,

            };

            Shipments.Add(shipment);
            return shipment;
        }
        private bool ValidateItemType(Items.Item item)
        {
            switch (TransactionType)
            {
                case Accounting.Enums.TransactionTypes.Sale:
                case Accounting.Enums.TransactionTypes.SalesReturn:
                    if (item.ItemType == ItemTypes.Service || item.ItemType == ItemTypes.Inventory || item.ItemType == ItemTypes.NonInventory)
                        return true;
                    else
                        return false;

                case Accounting.Enums.TransactionTypes.Purchase:
                case Accounting.Enums.TransactionTypes.PurchaseReturn:
                    if (item.ItemType == ItemTypes.Expense || item.ItemType == ItemTypes.Asset || item.ItemType == ItemTypes.Inventory || item.ItemType == ItemTypes.NonInventory)
                        return true;
                    else
                        return false;
                default:
                    return false;
            }
        }
        public void RemoveItems()
        {
            foreach (var item in CommercialItems.ToList())
            {
                CommercialItems.Remove(item);
            }
        }
        public void RemoveItem(Guid itemId)
        {
            var item = CommercialItems
                .Where(Item => Item.ItemGuid == itemId)
                .FirstOrDefault();

            if (item != null)
            {
                CommercialItems.Remove(item);
            }

        }
        public void ReCalculateTax()
        {
            this.CommercialTaxes
                .ToList()
                .ForEach(commercialTax =>
                {
                    commercialTax.UpdateTaxBalance(this.TaxOffset);
                });

            this.Tax = (CommercialTaxes?.ToList().Sum(i => i.TaxBalance) ?? 0);
        }

        public bool AddCommercialTax(TaxCode taxCode)
        {
            if (taxCode == null)
                return false;

            if (CommercialTaxes == null)
                CommercialTaxes = new List<CommercialTax>();

            var existCommercialTax = CommercialTaxes
                .Where(ct => ct.TaxCodeId == taxCode.Id)
                .FirstOrDefault();

            if (existCommercialTax == null)
            {
                var newCommercialTax = new CommercialTax()
                {
                    Id = Guid.NewGuid(),
                    TaxCode = taxCode,
                    Commercial = this,
                    CommercialId = Id
                };

                CommercialTaxes.Add(newCommercialTax);
                ReCalculate();
                return true;
            }
            return false;
        }
    }
}
