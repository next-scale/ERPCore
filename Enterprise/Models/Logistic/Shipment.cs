using ERPCore.Enterprise.Models.Logistic.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ERPCore.Enterprise.Models.Logistic
{
   



    [Table("ERP_Logistic_Shipments")]
    public class Shipment
    {
        [Key]
        public Guid Id { get; set; }
        public String TrackingNo { get; set; }
        public DateTime ShipDate { get; set; }
        public ShipmentStatus Status { get; set; }
        public decimal Weight { get; internal set; }
        public Guid? TransactionId { get; set; }





        public void Update(Shipment term)
        {
            this.TrackingNo = term.TrackingNo;
            this.ShipDate = term.ShipDate;
        }
    }
}
