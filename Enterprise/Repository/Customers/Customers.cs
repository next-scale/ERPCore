
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Models.Profiles;
using ERPCore.Enterprise.Models.Customers;
using ERPCore.Enterprise.Models.Customers.Enums;
using ERPCore.Enterprise.Models.Profiles.Enums;

namespace ERPCore.Enterprise.Repository.Profiles
{
    public class Customers : ERPNodeDalRepository
    {
        public Customers(Organization organization) : base(organization)
        {

        }

        public IQueryable<Profile> All =>  erpNodeDBContext.Customers.Select(r => r.Profile);

        public IQueryable<Customer> GetByStatus(ProfileQueryType Type = Models.Profiles.Enums.ProfileQueryType.All)
        {
            switch (Type)
            {
                case Models.Profiles.Enums.ProfileQueryType.Active:
                    return erpNodeDBContext.Customers.Where(c => c.Status == CustomerStatus.Active);

                case ERPCore.Enterprise.Models.Profiles.Enums.ProfileQueryType.InActive:
                    return erpNodeDBContext.Customers.Where(c => c.Status == CustomerStatus.InActive);

                case ERPCore.Enterprise.Models.Profiles.Enums.ProfileQueryType.All:
                default:
                    return erpNodeDBContext.Customers;
            }
        }

        public Customer Find(Guid id)=> erpNodeDBContext.Customers.Find(id);

        public void Delete(Customer customer)
        {
            erpNodeDBContext.Customers.Remove(customer);
            erpNodeDBContext.SaveChanges();
        }

        public Customer Create(Profile newCustomerProfile)
        {
            if (newCustomerProfile.Customer == null)
            {
                newCustomerProfile.Customer = new Customer()
                {
                    Status = CustomerStatus.Active
                };
            }
            erpNodeDBContext.SaveChanges();

            return newCustomerProfile.Customer;
        }

        public void UpdateSalesBalance()
        {
            var BalanceTables = erpNodeDBContext.Sales
                .GroupBy(o => o.ProfileGuid)
                .ToList()
                .Select(go => new
                {
                    Profile = go.Select(i => i.Profile).FirstOrDefault(),
                    TotalSale = go.Sum(i => i.Total),
                    CountSale = go.Count(),
                    TotalBalance = go.Sum(i => i.TotalBalance),
                    CountBalance = go.Where(i => i.TotalBalance > 0).Count(),
                })
                .ToList();


            erpNodeDBContext.Customers.ToList().ForEach(p =>
            {
                p.SumSaleBalance = 0;
                p.TotalBalance = 0;
                p.CountSales = 0;
            });

            BalanceTables.ForEach(b =>
            {
                b.Profile.Customer.SumSaleBalance = b.TotalSale;
                b.Profile.Customer.CountSales = b.CountSale;
                b.Profile.Customer.TotalBalance = b.TotalBalance;
                b.Profile.Customer.CountBalance = b.CountBalance;
            });

            erpNodeDBContext.SaveChanges();

        }

        public List<Customer> GetTopSales(int amount = 30)
        {
            var profiles = erpNodeDBContext.Customers
                     .OrderByDescending(c => c.SumSaleBalance)
                     .Take(amount)
                     .ToList();
            return profiles;
        }
    }
}