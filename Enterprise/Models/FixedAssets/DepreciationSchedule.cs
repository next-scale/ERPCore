using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ERPCore.Enterprise.Models.ChartOfAccount;
using System.ComponentModel;
using ERPCore.Enterprise.Models.Accounting.Enums;

namespace ERPCore.Enterprise.Models.Assets
{
    [Table("ERP_FixedAssets_DeprecateSchedules")]
    public class DeprecateSchedule
    {
        [Key]
        public Guid Id { get; set; }
        public int No { get; set; }

        [Column("FixedAssetId")]
        public Guid? FixedAssetId { get; set; }
        [ForeignKey("FixedAssetId")]
        public virtual FixedAsset FixedAsset { get; set; }

        public virtual ERPCore.Enterprise.Models.Accounting.FiscalYear FiscalYear { get; set; }

        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime TransactionDate => EndDate;

        public string Name => string.Format("{0}/{1}", this.TransactionDate.ToString("yyMM"), this.No.ToString().PadLeft(3, '0'));
        public int DayCount => (FiscalYear.EndDate - BeginDate.Date).Days + 1;
        public Decimal DepreciationValue { get; set; }
        public decimal DepreciateAccumulation { get; set; }
        public Decimal RemainValue => this.FixedAsset.AssetValue - this.DepreciateAccumulation;
        public Decimal OpenValue => this.FixedAsset.AssetValue - this.DepreciateAccumulation + this.DepreciationValue;





        public LedgerPostStatus PostStatus { get; set; }


        public DeprecateSchedule()
        {
            this.Id = Guid.NewGuid();
        }

    }
}
