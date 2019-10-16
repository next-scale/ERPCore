using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using ERPCore.Enterprise.Models.Accounting.Enums;

namespace ERPCore.Enterprise.Models.Transactions
{

    [Table("ERP_Transactions_Commercial_Histories")]
    public class CommercialDailyBalance
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionTypes Type { get; set; }
        public decimal Balance { get; set; }


        public CommercialDailyBalance()
        {
            Id = Guid.NewGuid();
        }
    }
}
