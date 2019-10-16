using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERPCore.Enterprise.Models.ChartOfAccount;
using System.ComponentModel;
using ERPCore.Enterprise.Models.Accounting;
using ERPCore.Enterprise.Models.Accounting.Enums;

namespace ERPCore.Enterprise.Models.Assets
{
    [Table("ERP_FixedAssets")]
    public class FixedAsset
    {
        [Key]
        public Guid Id { get; set; }
        public int No { get; set; }

        [Column("AssetName")]
        public String Name { get; set; }
        public String Code { get; set; }
        public String AssetCode => this.TransactionDate.ToString("yy") + "-" + this.FixedAssetType?.CodePrefix + this.No.ToString().PadLeft(3, '0');

        public string CodeName => string.Format("{0}/{1}", this.TransactionDate.ToString("yy"), this.No.ToString().PadLeft(3, '0'));

        public Guid? FiscalYearId { get; set; }
        [ForeignKey("FiscalYearId")]
        public FiscalYear FiscalYear { get; set; }


        public Enums.FixedAssetStatus Status { get; set; }
        public DateTime StartDeprecationDate { get; set; }
        public DateTime EndDeprecationDate => StartDeprecationDate.AddDays(-1).AddYears(this.FixedAssetType?.UseFulLifeYear ?? 1);
        public double RemainAgeDay
        {
            get
            {
                if (EndDeprecationDate >= DateTime.Today)
                    return (EndDeprecationDate - DateTime.Today).TotalDays;
                else
                    return 0;
            }
        }

        public DateTime TransactionDate => StartDeprecationDate;


        [Column("AssetValue", TypeName = "Money")]
        public Decimal AssetValue { get; set; }

        [Column(TypeName = "Money")]
        public Decimal SavageValue { get; set; }

        [Column(TypeName = "Money")]
        public Decimal PreDepreciationValue { get; set; }

        public Decimal TotalDepreciationValue => AssetValue - SavageValue;
        public String Memo { get; set; }
        public String Reference { get; set; }

        [Column("FixedAssetTypeId")]
        public Guid? FixedAssetTypeId { get; set; }
        [ForeignKey("FixedAssetTypeId")]
        public virtual FixedAssetType FixedAssetType { get; set; }

        public LedgerPostStatus PostStatus { get; set; }

        public decimal DeprecatePerYear => decimal.Round(this.TotalDepreciationValue / (decimal)(this.FixedAssetType?.UseFulLifeYear ?? 1), 2, MidpointRounding.AwayFromZero);

        public virtual ICollection<DeprecateSchedule> DepreciationSchedules { get; set; }

        public FixedAsset()
        {
            Id = Guid.NewGuid();
        }

        public virtual DeprecateSchedule GetNearestSchedules(FiscalYear FiscalYear)
        {
            return DepreciationSchedules?
                         .Where(d => d.EndDate <= FiscalYear.EndDate)
                         .OrderByDescending(x => x.EndDate).FirstOrDefault();
        }

        public virtual DeprecateSchedule GetNearestSchedules(DateTime date)
        {
            return DepreciationSchedules?
                         .Where(d => d.EndDate <= date)
                         .OrderByDescending(x => x.EndDate).FirstOrDefault();
        }

        public virtual decimal CurrentAssetValue => DepreciationSchedules?
                         .Where(d => d.FiscalYear.EndDate > DateTime.Today)
                         .OrderBy(x => x.EndDate)
                         .FirstOrDefault()?.OpenValue ?? this.SavageValue;


        public void UpdateStatus()
        {
            if (this.CurrentAssetValue == this.SavageValue)
            {
                this.Status = Enums.FixedAssetStatus.Closed;
            }
        }

    }
}
