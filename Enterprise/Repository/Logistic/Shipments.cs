
using ERPCore.Enterprise.Models.Equity;
using ERPCore.Enterprise.Models.Logistic;
using ERPCore.Enterprise.Models.Transactions.Commercials;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPCore.Enterprise.Repository.Logistic
{
    public class Shipments : ERPNodeDalRepository
    {
        public Shipments(Organization organization) : base(organization)
        {

        }

        public List<Shipment> ListAll => erpNodeDBContext.Shipments
                  .ToList();

        public Shipment Find(Guid id) => erpNodeDBContext.Shipments.Find(id);

       

        public void Delete(Guid id)
        {
            var CommercialShipment = erpNodeDBContext.Shipments.Find(id);
            erpNodeDBContext.Shipments.Remove(CommercialShipment);
            erpNodeDBContext.SaveChanges();
        }
    }
}