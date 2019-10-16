using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPCore.Enterprise.Models.ChartOfAccount
{
    [Table("ERP_Accounting_Default_Account")]
    public class DefaultAccount
    {
        [Key]
        public Enums.SystemAccountType AccountType { get; set; }

        public Guid? AccountItemId { get; set; }
        [ForeignKey("AccountItemId")]
        public virtual Account AccountItem { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}