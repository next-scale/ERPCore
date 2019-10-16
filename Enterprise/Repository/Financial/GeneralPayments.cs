
using ERPCore.Enterprise.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ERPCore.Enterprise.Models.Transactions.Commercials;
using ERPCore.Enterprise.Models.Financial;
using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.Financial.Payments;
using ERPCore.Enterprise.Models.Financial.Payments.Enums;
using ERPCore.Enterprise.Models.Profiles;

namespace ERPCore.Enterprise.Repository.Financial
{
    public class GeneralPayments : ERPNodeDalRepository
    {
        public GeneralPayments(Organization organization) : base(organization)
        {
            transactionType = TransactionTypes.GeneralPayment;
        }

        public GeneralPayment Find(Guid id) => erpNodeDBContext.GeneralPayments.Find(id);
        public IQueryable<GeneralPayment> Query => erpNodeDBContext.GeneralPayments;

        public void UpdateBalance()
        {
            erpNodeDBContext.GeneralPayments.ToList().ForEach(q => q.UpdateBalance());
            organization.SaveChanges();
        }
    }
}
