using ERPCore.Enterprise.Models.ChartOfAccount;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ERPCore.Enterprise.Models.Items
{
    [Table("ERP_Items_DataSheets")]
    public class DataSheet
    {
        [Key]
        public Guid Id { get; set; }
        public byte[] File { get; set; }

        public Guid ItemGuid { get; set; }
       
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public int ContentLength { get; set; }
        public string Name { get; set; }
    }
}