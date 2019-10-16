using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Estimations;
using ERPCore.Enterprise.Models.Transactions;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using ERPCore.Enterprise.Models.Estimations.Enums;
using ERPCore.Enterprise.Models.Accounting.Enums;

namespace ERPCore.Enterprise.Repository.Estimations
{
    public class SalesEstimates : ERPNodeDalRepository
    {
        public SalesEstimates(Organization organization) : base(organization)
        {

        }

        public List<SalesEstimate> ListAll => erpNodeDBContext.SalesEstimates.ToList();

        public SalesEstimate Find(Guid saleEstimateId)
        {
            return erpNodeDBContext.SalesEstimates.Find(saleEstimateId);
        }

        public void Remove(SalesEstimate entity)
        {

        }

        public List<SalesEstimate> ListByStatus(EstimateStatus? status)
        {
            if (status == null)
                return erpNodeDBContext.SalesEstimates.Where(e => e.Status != EstimateStatus.Void && e.Status != EstimateStatus.Close)
                .ToList();
            else
                return erpNodeDBContext.SalesEstimates.Where(e => e.Status == status)
              .ToList();

        }

        public int NextNumber => (erpNodeDBContext.SalesEstimates.Max(b => (int?)b.No) ?? 0) + 1;

        public IQueryable<SalesEstimate> Query => erpNodeDBContext.SalesEstimates;

        public void VoidExpired()
        {
            DateTime lastestDate = DateTime.Today.AddDays(-90);

            var estimates = erpNodeDBContext.SalesEstimates.Where(t => t.TransactionDate < lastestDate)
              .Where(t => t.Status == Models.Estimations.Enums.EstimateStatus.Quote)
              .ToList();

            foreach (var est in estimates)
            {
                est.Status = Models.Estimations.Enums.EstimateStatus.Void;
            }

            erpNodeDBContext.SaveChanges();
        }

        public SalesEstimate Create(Guid profileGuid, DateTime createDate)
        {
            var newEstimate = new SalesEstimate()
            {
                TransactionType = TransactionTypes.SalesEstimate,
                Id = Guid.NewGuid(),
                No = this.NextNumber,
                ProfileGuid = profileGuid,
                TransactionDate = createDate,
                TaxCode = organization.TaxCodes.GetDefaultOuput,
                SelfProfileGuid = organization.SelfProfile.Id,
                ExpiredInDayCount = 90,
            };

            erpNodeDBContext.SalesEstimates.Add(newEstimate);
            erpNodeDBContext.SaveChanges();

            return newEstimate;
        }

        public void ReOrder()
        {
            var transfers = erpNodeDBContext.SalesEstimates
              .OrderBy(t => t.TransactionDate).ThenBy(t => t.No)
              .ToList();

            int i = 0;
            foreach (var transfer in transfers)
            {
                transfer.No = ++i;
            }

            erpNodeDBContext.SaveChanges();
        }

        public SalesEstimate UpdateChanges(SalesEstimate estimate)
        {
            var existEstimate = erpNodeDBContext.SalesEstimates.Find(estimate.Id);
            if (existEstimate != null)
            {
                existEstimate.Reference = estimate.Reference;
                existEstimate.Memo = estimate.Memo;

                existEstimate.ProjectGuid = estimate.ProjectGuid;
                existEstimate.TransactionDate = estimate.TransactionDate;
                existEstimate.ExpiredInDayCount = estimate.ExpiredInDayCount;


                existEstimate.PaymentTermGuid = estimate.PaymentTermGuid;
                existEstimate.ShippingTermGuid = estimate.ShippingTermGuid;
     
                existEstimate.TaxCodeGuid = estimate.TaxCodeGuid;
                existEstimate.TaxCode = organization.TaxCodes.Find(estimate.TaxCodeGuid);

                existEstimate.Calculate();

                erpNodeDBContext.SaveChanges();
                return estimate;
            }

            return null;
        }

        public void Delete(Guid id)
        {
            var salesEstimate = organization.SalesEstimates.Find(id);

            foreach (var item in salesEstimate.Items.ToList())
            {
                salesEstimate.Items.Remove(item);
            }
            organization.SaveChanges();


            erpNodeDBContext.SalesEstimates.Remove(salesEstimate);
            organization.SaveChanges();
        }


        public SalesEstimate Copy(SalesEstimate originalSalesEstimate, DateTime trDate)
        {
            var cloneSalesEstimate = this.erpNodeDBContext.SalesEstimates
                    .AsNoTracking()
                    .Include(p => p.Items)
                    .FirstOrDefault(x => x.Id == originalSalesEstimate.Id);

            cloneSalesEstimate.Id = Guid.NewGuid();
            cloneSalesEstimate.TransactionDate = trDate;
            cloneSalesEstimate.Reference = "Clone-" + cloneSalesEstimate.Reference;
            cloneSalesEstimate.No = organization.SalesEstimates.NextNumber;
            cloneSalesEstimate.Status = EstimateStatus.Quote;
            cloneSalesEstimate.Items.ToList().ForEach(ci => ci.Id = Guid.NewGuid());


            this.erpNodeDBContext.SalesEstimates.Add(cloneSalesEstimate);
            this.erpNodeDBContext.SaveChanges();

            return cloneSalesEstimate;
        }
    }
}