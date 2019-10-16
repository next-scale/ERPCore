using ERPCore.Enterprise.Models.ChartOfAccount;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Models.Items
{
    [Table("ERP_Items")]
    public partial class Item
    {
        [Key]
        public Guid Id { get; set; }
        public ItemStatus Status { get; set; }

        [Index]
        public Enums.ItemTypes ItemType { get; set; }
        public int Level { get; set; }


       

        [MaxLength(30)]
        public String PartNumber { get; set; }

        [Column("UPC")]
        public String UPC { get; set; }

        public String Description { get; set; }
        public bool OnlineSale { get; set; }
        public int CommercialAmount { get; set; }



        [Column("SalesPrice", TypeName = "Money")]
        public Decimal UnitPrice { get; set; }

        public Decimal PurchasePrice { get; set; }

        public Guid? PriceRangeId { get; set; }
        [ForeignKey("PriceRangeId")]
        public virtual PriceRange PriceRange { get; set; }

        public virtual string HarmonizedCode { get; set; }



        public virtual ICollection<Transactions.CommercialItem> CommercialItems { get; set; }
        public virtual ICollection<Estimations.EstimateItem> EstimateItems { get; set; }
        public virtual ICollection<ItemImage> Images { get; set; }
        public virtual ICollection<DataSheet> DataSheets { get; set; }
        public virtual ICollection<ItemParameter> ItemParameters { get; set; }


        public List<ItemParameter> GetParametersList()
        {
            List<ItemParameter> parameters = new List<ItemParameter>();

            if (this.ItemParameters != null)
                parameters.AddRange(this.ItemParameters.ToList());

            if (this.ParentId != null && this.Parent.ItemParameters != null)
                parameters.AddRange(this.Parent.GetParametersList());

            return parameters;
        }

        public Guid? itemGroupId { get; set; }
        public virtual ItemImage DefaultImage => Images.FirstOrDefault();
        public string Specification { get; set; }


        public Guid? BrandGuid { get; set; }
        [ForeignKey("BrandGuid")]
        public virtual Brand Brand { get; set; }

        public String BrandName => Brand?.Name;

        public Guid? DefaultSupplyierId { get; set; }
        [ForeignKey("DefaultSupplyierId")]
        public virtual Profiles.Profile DefaultSupplyier { get; set; }

        [DisplayName("Purchase Account")]
        [Column("PurchaseAccountId")]
        public Guid? PurchaseAccountId { get; set; }
        [ForeignKey("PurchaseAccountId")]
        public virtual Account PurchaseAccount { get; set; }


        [DisplayName("Income Account")]
        [Column("IncomeAccountId")]
        public Guid? IncomeAccountId { get; set; }
        [ForeignKey("IncomeAccountId")]
        public virtual Account IncomeAccount { get; set; }


        public virtual Account GetPurchaseAccount
        {
            get
            {
                if (this.ItemType == Enums.ItemTypes.Asset)
                    return this.FixedAssetType.AssetAccount;
                else
                    return this.PurchaseAccount;
            }
        }

        public DateTime? StockCalculateDate { get; set; }

        [Column("stockValue")]
        public decimal StockValue { get; set; }
        public int StockAmount { get; set; }


        public decimal OpeningPurchaseCost { get; set; }
        public int OpeningPurchaseAmount { get; set; }


        public string QuickFinderKey { get; set; }
        public string QuickFinderValue { get; set; }

        public string GetQuickFinderValue => QuickFinderValue ?? this.Brand?.Name;
        public DateTime? CreatedDate { get; set; }
        public void AddImage(byte[] toPutInDb)
        {
            var itemImage = new ERPCore.Enterprise.Models.Items.ItemImage()
            {
                Id = Guid.NewGuid(),
                ItemGuid = Id,
                Image = toPutInDb
            };

            this.Images.Add(itemImage);
        }
        public void AddItemParameter(ItemParameterType type, string value)
        {
            if (this.ItemParameters == null)
                return;

            var existParameter = this.ItemParameters
                .Where(ip => ip.ParameterTypeId == type.Id)
                .FirstOrDefault();

            if (existParameter != null)
                throw new Exception("Exist Parameter");

            var parameter = new ItemParameter()
            {
                Id = Guid.NewGuid(),
                ParameterTypeId = type.Id,
                ParameterType = type,
                Value = value
            };

            this.ItemParameters.Add(parameter);

        }
        public void AssignLevel(int CurrentLevel)
        {
            CurrentLevel++;
            this.Level = CurrentLevel;

            this.Child.Where(c => c.ItemType == Enums.ItemTypes.Group)
                .ToList()
                .ForEach(g =>
                {
                    if (g.Level == 0)
                    {
                 //       Console.WriteLine("Dig inside " + CurrentLevel);
                        g.AssignLevel(CurrentLevel);
                    }
                    else
                    {
                        Console.WriteLine("Loop Found");
                        g.ParentId = null;
                        g.Parent = null;
                    }
                });

        }
    }
}