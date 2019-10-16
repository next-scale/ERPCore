using ERPCore.Enterprise.Models.Profiles;
using ERPCore.Enterprise.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace ERPCore.Enterprise.Repository.Profiles
{
    public class Profiles : ERPNodeDalRepository
    {
        public Profiles(Organization organization) : base(organization)
        {

        }

        public IQueryable<Profile> Query => erpNodeDBContext.Profiles;
        public Profile GetSelectRandom => erpNodeDBContext.Profiles.OrderBy(r => Guid.NewGuid()).Take(1).FirstOrDefault();
        public List<Profile> GetCustomers => erpNodeDBContext.Customers.Select(r => r.Profile).ToList();
        public List<Profile> GetSuppliers => erpNodeDBContext.Suppliers.Select(r => r.Profile).ToList();
        public List<Profile> GetPermissionProfileList => erpNodeDBContext.Profiles.Where(p => p.AccessLevel != Models.Security.Enums.AccessLevel.None).ToList();
        public Profile GetSelfOrganization => erpNodeDBContext.Profiles.Where(r => r.isSelfOrganization == true).First();




        public Profile Authen(LogInModel loginModel)
        {
            return this.Query
            .Where(p => p.Email.ToLower() == loginModel.Email || p.TaxNumber.ToLower() == loginModel.Email)
            .Where(p => p.Pin == loginModel.Pin)
            .FirstOrDefault();
        }



        public object ListHistoryEvent(Guid id)
        {
            var profileHistories = erpNodeDBContext.HistoryItems
                .Where(historyItem => historyItem.ProfileGuid == id)
                .GroupBy(historyItem => historyItem.KeyId)
                .Select(hi => hi.OrderByDescending(h => h.RecordDate).FirstOrDefault())
                .OrderByDescending(historyItem => historyItem.RecordDate)
                .Take(10)
                .ToList();

            return profileHistories;
        }


        public Profile Find(Guid id) => erpNodeDBContext.Profiles.Find(id);


        public void Remove(Profile profile)
        {
            Console.WriteLine("DELETE > " + profile.Name);

            if (profile.Supplier != null)
                erpNodeDBContext.Suppliers.Remove(profile.Supplier);
            if (profile.Customer != null)
                erpNodeDBContext.Customers.Remove(profile.Customer);
            if (profile.Employee != null)
                erpNodeDBContext.Employees.Remove(profile.Employee);
            if (profile.Investor != null)
                erpNodeDBContext.Investors.Remove(profile.Investor);

            erpNodeDBContext.ProfileBankAccounts.RemoveRange(profile.BankAccounts);
            erpNodeDBContext.ProfileAddresses.RemoveRange(profile.Addresses);
            erpNodeDBContext.HistoryItems.RemoveRange(profile.Histories);
            erpNodeDBContext.Profiles.Remove(profile);
            erpNodeDBContext.SaveChanges();
        }

        public IQueryable<Profile> GetProfiles(ProfileType? type = null)
        {
            switch (type)
            {
                case ProfileType.People:
                    return this.GetPeople();
                case ProfileType.Organization:
                    return this.GetOrganization();
                default:
                    return erpNodeDBContext.Profiles;
            }
        }

        public IQueryable<Profile> GetOrganization() => erpNodeDBContext.Profiles.Where(p => p.ProfileType == ProfileType.Organization);
        public IQueryable<Profile> GetPeople() => erpNodeDBContext.Profiles.Where(p => p.ProfileType == ProfileType.People);

        public IList<Profile> ListAll => erpNodeDBContext.Profiles.ToList();
        public Profile Save(Profile profile)
        {
            var existingProfile = erpNodeDBContext.Profiles.Find(profile.Id);
            existingProfile.Update(profile);
            erpNodeDBContext.SaveChanges();

            return existingProfile;

        }
        public Profile ChangePin(Profile profile)
        {
            var existingProfile = erpNodeDBContext.Profiles.Find(profile.Id);
            erpNodeDBContext.SaveChanges();

            return existingProfile;
        }
        public Profile CreateNew(Models.Profiles.ProfileType type, string name, string email, string taxId)
        {

            var profile = new ERPCore.Enterprise.Models.Profiles.Profile()
            {
                Id = Guid.NewGuid(),
                ProfileType = type,
                Name = name,
                TaxNumber = taxId,
                Email = email,
                CreatedDate = DateTime.Today,
                localizedLanguage = ERPCore.Enterprise.Models.Profiles.EnumLanguage.en
            };

            erpNodeDBContext.Profiles.Add(profile);
            erpNodeDBContext.SaveChanges();

            return profile;
        }
        public Profile CreateNew(ProfileType Type)
        {
            var newProfile = new Profile()
            {
                ProfileType = Type,
                localizedLanguage = EnumLanguage.en,
                Id = Guid.NewGuid(),
                Name = "New Profile",
            };
            erpNodeDBContext.Profiles.Add(newProfile);
            erpNodeDBContext.SaveChanges();

            return newProfile;
        }

        public void RemoveUnReferenceProfile()
        {
            var profiles = erpNodeDBContext.Profiles.ToList();

            profiles.ForEach(p =>
            {
                int commercialCount = erpNodeDBContext.Commercials.Where(c => c.ProfileGuid == p.Id).Count();
                int estimateCount = erpNodeDBContext.Estimates.Where(c => c.ProfileGuid == p.Id).Count();
                int employeeCount = erpNodeDBContext.EmployeePayments.Where(c => c.EmployeeId == p.Id).Count();
                int capitalCount = erpNodeDBContext.CapitalActivities.Where(c => c.InvestorId == p.Id).Count();

                var total = commercialCount + estimateCount + employeeCount + capitalCount;


                if (total == 0 && !p.isSelfOrganization)
                {
                    this.Remove(p);
                }
            });
            erpNodeDBContext.SaveChanges();
        }
    }
}