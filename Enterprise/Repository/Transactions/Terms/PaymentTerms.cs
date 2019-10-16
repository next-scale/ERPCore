
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Financial;
using ERPCore.Enterprise.Models.Financial.Transfer;
using ERPCore.Enterprise.Models.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Models.Employees;
using ERPCore.Enterprise.Models.Transactions.Commercials;

namespace ERPCore.Enterprise.Repository.Transactions.Terms
{
    public class PaymentTerms : ERPNodeDalRepository
    {
        public PaymentTerms(Organization organization) : base(organization)
        {

        }


        public List<PaymentTerm> ListAll => erpNodeDBContext.PaymentTerms.ToList();
        public PaymentTerm Find(Guid id) => erpNodeDBContext.PaymentTerms.Find(id);
        public IQueryable<PaymentTerm> Query => erpNodeDBContext.PaymentTerms;

        public PaymentTerm CreateNew(PaymentTerm paymentTerm)
        {
            paymentTerm.Id = Guid.NewGuid();
            erpNodeDBContext.PaymentTerms.Add(paymentTerm);
            erpNodeDBContext.SaveChanges();

            return paymentTerm;
        }
    }
}