using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace ERPCore.Enterprise.Models.Financial.Payments
{
    [Table("ERP_Finance_RetentionTypes")]
    public class RetentionType
    {
        [Key, Column("Id")]
        public Guid Id { get; set; }

        public String Name { get; set; }

        public String Description { get; set; }

        public bool IsActive { get; set; }

        public Enums.RetentionDirection RetentionDirection { get; set; }
        public Enums.RetentionStatus Status { get; set; }


        public Decimal? Rate { get; set; }

        public Guid? RetentionToAccountGuid { get; set; }
        [ForeignKey("RetentionToAccountGuid")]
        public virtual Models.ChartOfAccount.Account RetentionToAccount { get; set; }

        public void Update(RetentionType type)
        {
            this.Name = type.Name;
            this.Description = type.Description;
            this.RetentionToAccountGuid = type.RetentionToAccountGuid;
            this.IsActive = type.IsActive;
            this.Id = type.Id;
            this.Rate = type.Rate;
            this.RetentionDirection = type.RetentionDirection;
        }
    }
}
