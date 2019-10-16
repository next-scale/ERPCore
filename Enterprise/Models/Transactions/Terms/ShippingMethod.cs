using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ERPCore.Enterprise.Models.Transactions.Commercials
{

    [Table("ERP_ShippingMethods")]
    public class ShippingMethod
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }

        public String Name { get; set; }
        public String Detail { get; set; }

        public void Update(ShippingMethod term)
        {
            this.Name = term.Name ?? this.Name;
            this.Detail = term.Detail;
        }
    }



}
