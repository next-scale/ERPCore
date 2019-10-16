
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Financial;
using ERPCore.Enterprise.Models.Financial.Transfer;
using ERPCore.Enterprise.Models.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Models.Employees;
using ERPCore.Enterprise.Models.Transactions.Commercials;
using ERPCore.Enterprise.Models.Security;

namespace ERPCore.Enterprise.Repository.Security
{
    public class Members : ERPNodeDalRepository
    {
        public Members(Organization organization) : base(organization)
        {

        }

        public List<Member> ListAll => erpNodeDBContext.Members.ToList();
        public Member Find(Guid id) => erpNodeDBContext.Members.Find(id);
        public IQueryable<Member> Query => erpNodeDBContext.Members;


        public Member Crate(Guid coreProfileId, String name)
        {
            var existMember = erpNodeDBContext.Members.Find(coreProfileId);

            if (existMember != null)
                return existMember;

            var newMember = new Member()
            {
                Id = coreProfileId,
                Name = name,
                AccessLevel = Models.Security.Enums.AccessLevel.Viewer
            };
            erpNodeDBContext.Members.Add(newMember);
            erpNodeDBContext.SaveChanges();
            return newMember;
        }

    }
}