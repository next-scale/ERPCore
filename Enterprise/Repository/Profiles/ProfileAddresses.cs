
using ERPCore.Enterprise.Models.Items;
using ERPCore.Enterprise.Models.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPCore.Enterprise.Repository.Profiles
{
    public class ProfileAddresses : ERPNodeDalRepository
    {
        public ProfileAddresses(Organization organization) : base(organization)
        {

        }

        public IQueryable<ProfileAddress> Query => erpNodeDBContext.ProfileAddresses;
        public List<ProfileAddress> ListAll => erpNodeDBContext.ProfileAddresses.ToList();
        public ProfileAddress Find(Guid id) => erpNodeDBContext.ProfileAddresses.Find(id);

        public void Delete(Guid id)
        {
            var profileAddress = erpNodeDBContext.ProfileAddresses.Find(id);

            erpNodeDBContext.ProfileAddresses.Remove(profileAddress);
            organization.SaveChanges();
        }
        public ProfileAddress CreateNew(Guid profileId)
        {
            var addreses = new ProfileAddress()
            {
                ProfileGuid = profileId,
                AddressGuid = Guid.NewGuid()
            };

            erpNodeDBContext.ProfileAddresses.Add(addreses);
            erpNodeDBContext.SaveChanges();

            return addreses;
        }
        public ProfileAddress CreateNew(ProfileAddress profileAddress)
        {
            profileAddress.AddressGuid = Guid.NewGuid();
            erpNodeDBContext.ProfileAddresses.Add(profileAddress);
            return profileAddress;
        }
        public ProfileAddress CreateNew(string name)
        {
            var profileAddress = new ProfileAddress();
            profileAddress.AddressGuid = Guid.NewGuid();

            erpNodeDBContext.ProfileAddresses.Add(profileAddress);
            erpNodeDBContext.SaveChanges();
            return profileAddress;
        }
    }
}
