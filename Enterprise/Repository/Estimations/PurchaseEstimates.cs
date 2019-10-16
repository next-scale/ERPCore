using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Estimations;
using ERPCore.Enterprise.Models.Estimations.Enums;
using ERPCore.Enterprise.Models.Transactions;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ERPCore.Enterprise.Repository.Estimations
{
    public class PurchaseEstimates : ERPNodeDalRepository
    {
        public PurchaseEstimates(Organization organization) : base(organization)
        {

        }

        public List<PurchaseEstimate> ListAll => erpNodeDBContext.PurchaseEstimates.ToList();
        public IQueryable<PurchaseEstimate> Query => erpNodeDBContext.PurchaseEstimates;
        public PurchaseEstimate Find(Guid saleEstimateId) => erpNodeDBContext.PurchaseEstimates.Find(saleEstimateId);

        public void Remove(PurchaseEstimate entity) => erpNodeDBContext.PurchaseEstimates.Remove(entity);
        public int NextNumber => (erpNodeDBContext.PurchaseEstimates.Max(e => (int?)e.No) ?? 0) + 1;

        public List<PurchaseEstimate> ListByStatus(EstimateStatus status) => erpNodeDBContext.PurchaseEstimates
                    .Where(e => e.Status == status)
            .ToList();



        public Estimate Create(Guid profileGuid, DateTime createDate, ERPCore.Enterprise.Models.Transactions.Enums.PurchasePurposes purpose)
        {
            var newPurchaseEstimate = new PurchaseEstimate()
            {
                TransactionType = TransactionTypes.PurchaseEstimate,
                Id = Guid.NewGuid(),
                No = this.NextNumber,
                ProfileGuid = profileGuid,
                TransactionDate = createDate,
                PurchasePurposes = purpose,
                TaxCode = organization.TaxCodes.GetDefaultInput,
                SelfProfileGuid = organization.SelfProfile.Id,
                ExpiredInDayCount = 90,
            };

            erpNodeDBContext.PurchaseEstimates.Add(newPurchaseEstimate);
            erpNodeDBContext.SaveChanges();

            return newPurchaseEstimate;
        }

        public void ReOrder()
        {
            var transfers = erpNodeDBContext.PurchaseEstimates
              .OrderBy(t => t.TransactionDate)
              .ThenBy(t => t.No)
              .ToList();

            int i = 0;
            transfers.ForEach(t => t.No = i++);

            erpNodeDBContext.SaveChanges();
        }

        public PurchaseEstimate Save(PurchaseEstimate est)
        {
            var existEst = erpNodeDBContext.PurchaseEstimates.Find(est.Id);

            if (existEst != null)
            {
                existEst.Reference = est.Reference;
                existEst.Memo = est.Memo;

                existEst.ProjectGuid = est.ProjectGuid;
                existEst.TransactionDate = est.TransactionDate;
                existEst.PaymentTermGuid = est.PaymentTermGuid;
                existEst.ShippingTermGuid = est.ShippingTermGuid;

                existEst.TaxCodeGuid = est.TaxCodeGuid;
                existEst.TaxCode = organization.TaxCodes.Find(est.TaxCodeGuid);

                existEst.Calculate();
                erpNodeDBContext.SaveChanges();
                return est;
            }

            return null;
        }

        public void Delete(Guid id)
        {
            var PurchasesEstimate = erpNodeDBContext.PurchaseEstimates.Find(id);

            foreach (var item in PurchasesEstimate.Items.ToList())
            {
                PurchasesEstimate.Items.Remove(item);
            }
            organization.SaveChanges();
            erpNodeDBContext.PurchaseEstimates.Remove(PurchasesEstimate);
            organization.SaveChanges();
        }


        public PurchaseEstimate Copy(PurchaseEstimate originalPurchaseEstimate, DateTime trDate)
        {
            var clonePurchaseEstimate = this.erpNodeDBContext.PurchaseEstimates
                    .AsNoTracking()
                    .Include(p => p.Items)
                    .FirstOrDefault(x => x.Id == originalPurchaseEstimate.Id);

            clonePurchaseEstimate.Id = Guid.NewGuid();
            clonePurchaseEstimate.TransactionDate = trDate;
            clonePurchaseEstimate.Reference = "Clone-" + clonePurchaseEstimate.Reference;
            clonePurchaseEstimate.No = organization.PurchaseEstimates.NextNumber;
            clonePurchaseEstimate.Status = EstimateStatus.Quote;
            clonePurchaseEstimate.Items.ToList().ForEach(ci => ci.Id = Guid.NewGuid());


            this.erpNodeDBContext.PurchaseEstimates.Add(clonePurchaseEstimate);
            this.erpNodeDBContext.SaveChanges();

            return clonePurchaseEstimate;
        }
    }
}