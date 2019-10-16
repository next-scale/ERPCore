
using ERPCore.Enterprise.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Reports.CompanyandFinancial;
using ERPCore.Enterprise.Models.Accounting.Enums;

namespace ERPCore.Enterprise.Repository.Accounting
{
    public class PreviewAccounts : ERPNodeDalRepository
    {
        public PreviewAccounts(Organization organization) : base(organization)
        {

        }

        public void Remove(Guid id)
        {
            var previewAccountItem = erpNodeDBContext.PreviewAccounts.Find(id);

            if (previewAccountItem != null)
            {
                erpNodeDBContext.PreviewAccounts.Remove(previewAccountItem);
                erpNodeDBContext.SaveChanges();
            }
        }

        public PreviewAccount Find(Guid accountId, Guid profileId)
        {
            return erpNodeDBContext.PreviewAccounts
                .Where(p => p.AccountGuid == accountId && p.OwnerProfileGuid == profileId)
                .FirstOrDefault();

        }

        public void Create(Guid id, Guid profileId)
        {
            PreviewAccount newPreviewAccount = new PreviewAccount(id, profileId);
            erpNodeDBContext.PreviewAccounts.Add(newPreviewAccount);
            erpNodeDBContext.SaveChanges();
        }
    }
}