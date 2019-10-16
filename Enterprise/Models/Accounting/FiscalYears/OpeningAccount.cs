using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ERPCore.Enterprise.Models.ChartOfAccount;

namespace ERPCore.Enterprise.Models.Accounting.FiscalYears
{
    [Table("ERP_FiscalYear_OpeningAccounts")]
    public class OpeningEntry
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? AccountGuid { get; set; }
        [ForeignKey("AccountGuid")]
        public virtual Account Account { get; set; }

        public LedgerPostStatus PostStatus { get; set; }
        public Guid? FiscalYearId { get; set; }
        [ForeignKey("FiscalYearId")]
        public virtual ERPCore.Enterprise.Models.Accounting.FiscalYear FiscalYear { get; set; }

        public Decimal? Debit { get; set; }
        public Decimal? Credit { get; set; }
        public Decimal? Multiplier { get; set; }


      


        [NotMapped]
        public Decimal Balance
        {
            get
            {
                if (Account == null)
                    return 0;

                switch (Account.Type)
                {
                    case AccountTypes.Asset:
                        return (Debit ?? 0) - (Credit ?? 0);
                    case AccountTypes.Capital:
                    case AccountTypes.Liability:
                        return (Credit ?? 0) - (Debit ?? 0);
                }

                return 0;
            }
        }

    }
}
