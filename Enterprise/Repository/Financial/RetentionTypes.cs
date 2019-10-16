
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Financial;
using ERPCore.Enterprise.Models.Financial.Transfer;
using ERPCore.Enterprise.Models.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Models.Employees;
using ERPCore.Enterprise.Models.Financial.Payments;
using ERPCore.Enterprise.Models.Financial.Payments.Enums;

namespace ERPCore.Enterprise.Repository.Financial
{
    public class RetentionTypes : ERPNodeDalRepository
    {
        public RetentionTypes(Organization organization) : base(organization)
        {

        }


        public List<RetentionType> ListAll => erpNodeDBContext.RetentionTypes.ToList();
        public List<RetentionType> ReceiveRetentionsTypeList =>
               erpNodeDBContext.RetentionTypes
            .Where(r => r.RetentionDirection == RetentionDirection.Receive).ToList();

        public List<RetentionType> PayRetentionsTypeList =>
              erpNodeDBContext.RetentionTypes
           .Where(r => r.RetentionDirection == RetentionDirection.Pay).ToList();


        public RetentionType Find(Guid id) => erpNodeDBContext.RetentionTypes.Find(id);

        public RetentionType CreateNew(RetentionType type)
        {
            erpNodeDBContext.RetentionTypes.Add(type);
            return type;
        }

        public RetentionType CreateNew(RetentionDirection id)
        {
            var newRetentionType = new RetentionType()
            {
                Id = Guid.NewGuid(),
                Rate = 0,
                RetentionDirection = id,
                Status = RetentionStatus.InActive
            };

            erpNodeDBContext.RetentionTypes.Add(newRetentionType);
            organization.SaveChanges();
            return newRetentionType;
        }
    }
}