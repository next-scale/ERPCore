using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ERPCore.Enterprise.Models.Transactions.Commercials
{
    [Table("ERP_PaymentTerms")]
    public class PaymentTerm
    {
        [Key]
        public Guid Id { get; set; }
        public String Name { get; set; }
        public int DueDayCount { get; set; }

        public Decimal DiscountPercent { get; set; }


        public int MaxDayCount { get; set; }

        public void Update(PaymentTerm term)
        {
            this.Name = term.Name;
            this.DiscountPercent = term.DiscountPercent;
            this.DueDayCount = term.DueDayCount;
        }
    }
}
