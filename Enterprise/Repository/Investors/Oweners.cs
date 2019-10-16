
using ERPCore.Enterprise.Models.Equity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPCore.Enterprise.Repository.Profiles
{
    public class Investors : ERPNodeDalRepository
    {
        public Investors(Organization organization) : base(organization)
        {

        }

        public List<Investor> ListAll => erpNodeDBContext.Investors
                  .ToList();

        public Investor Find(Guid id) => erpNodeDBContext.Investors.Find(id);

        public void UpdateStockCount()
        {
            erpNodeDBContext.Investors.ToList().ForEach(i =>
            {
                i.UpdateStockCount();
            });

            erpNodeDBContext.SaveChanges();
        }

        public Models.Equity.Investor Create(Models.Profiles.Profile newEmployeeProfile)
        {
            if (newEmployeeProfile.Investor == null)
            {
                newEmployeeProfile.Investor = new Investor()
                {
                    Status = Models.Equity.Enums.InvestorStatus.Active,
                    StockAmount = 0
                };
            }
            erpNodeDBContext.SaveChanges();

            return newEmployeeProfile.Investor;
        }

        public void Delete(Guid id)
        {
            var investor = erpNodeDBContext.Investors.Find(id);
            erpNodeDBContext.Investors.Remove(investor);
            erpNodeDBContext.SaveChanges();
        }
    }
}