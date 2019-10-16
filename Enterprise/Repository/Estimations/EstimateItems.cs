
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

namespace ERPCore.Enterprise.Repository.Estimations
{
    public class EstimateItems : ERPNodeDalRepository
    {
        public EstimateItems(Organization organization) : base(organization)
        {

        }

        public List<EstimateItem> ListAll => erpNodeDBContext.EstimateItems.ToList();

        public EstimateItem Find(Guid saleEstimateId)
        {
            return erpNodeDBContext.EstimateItems.Find(saleEstimateId);
        }

        public IQueryable<EstimateItem> Query => erpNodeDBContext.EstimateItems;

        public void Delete(Guid id)
        {
            var salesEstimate = organization.EstimateItems.Find(id);
            erpNodeDBContext.EstimateItems.Remove(salesEstimate);
            organization.SaveChanges();
        }

        public void Remove(EstimateItem estimateItem)
        {
            erpNodeDBContext.EstimateItems.Remove(estimateItem);
            organization.SaveChanges();
        }
    }
}