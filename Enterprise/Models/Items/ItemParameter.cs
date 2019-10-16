
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Models.Items
{
    [Table("ERP_Item_Parameters")]
    public class ItemParameter
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? ItemId { get; set; }
        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }

        public Guid? ParameterTypeId { get; set; }
        [ForeignKey("ParameterTypeId")]
        public virtual ItemParameterType ParameterType { get; set; }
        public string Value { get; set; }
    }
}