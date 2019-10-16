using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Models.Datum
{
    [Table("ERP_Datum")]
    public class DataItem
    {
        [Key]
        public Guid Id { get; set; }
        public DataItemKey Key { get; set; }



        public String Value { get; set; }
        public DataType DataType { get; set; }
        

        public String ValueString { get; set; }
        public String ValueDecimal { get; set; }
        public String VlaueInt { get; set; }
        public String VlaueDateTime { get; set; }

    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeRush", "Type can be moved to separate file")]
    public enum DataType
    {
        String = 0,
        Decimal = 1,
        Int = 2,
        DateTime = 3

    }
}