using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Financial.Payments;
using ERPCore.Enterprise.Models.Financial.Payments.Enums;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ERPCore.Enterprise.Repository.Financial
{
    public class PaymentRetentions : ERPNodeDalRepository
    {



        public PaymentRetentions(Organization organization) : base(organization)
        {
        }

        public PaymentRetention Find(Guid uid) => erpNodeDBContext.PaymentRetentions.Find(uid);

        public List<PaymentRetention> AllItems => erpNodeDBContext.PaymentRetentions.ToList();




    }

}
