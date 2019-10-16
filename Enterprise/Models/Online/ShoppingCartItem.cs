using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace ERPCore.Enterprise.Models.Online
{

    [Table("ERP_ShoppingCart_Items")]
    public class ShoppingCartItem
    {
        [Key, Column("Id")]
        public Guid Id { get; set; }

        public string SessionId { get; set; }

        public DateTime CreateDate { get; set; }

        public Guid? ProfileId { get; set; }
        [ForeignKey("ProfileId")]
        public virtual ERPCore.Enterprise.Models.Profiles.Profile Profile { get; set; }

        public Guid ItemGuid { get; set; }
        [ForeignKey("ItemGuid")]
        public virtual ERPCore.Enterprise.Models.Items.Item Item { get; set; }



        [Column("UnitPrice")]
        public Decimal UnitPrice { get; set; }

        
        public int Amount { get; set; }


        public string Memo { get; set; }

        [DisplayFormat(DataFormatString = "N2")]
        [NotMapped]
        public Decimal LineTotal => Amount * UnitPrice;

    }
}