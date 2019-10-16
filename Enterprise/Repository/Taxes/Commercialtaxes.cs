using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.ChartOfAccount.Enums;
using ERPCore.Enterprise.Models.Company;
using ERPCore.Enterprise.Models.Taxes;
using ERPCore.Enterprise.Models.Transactions;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using ERPCore.Enterprise.Models.Taxes.Enums;

namespace ERPCore.Enterprise.Repository.Taxes
{
    public class CommercialTaxes : ERPNodeDalRepository
    {
        public CommercialTaxes(Organization organization) : base(organization)
        {
            transactionType = TransactionTypes.CommercialTax;
        }

        public IQueryable<CommercialTax> Query => erpNodeDBContext.CommercialTaxes;


        public List<CommercialTax> GetAssignAbleCommercialTaxesList()
        {
            this.RemoveUnReference();
            return erpNodeDBContext.CommercialTaxes
                .Where(c => c.TaxPeriodId == null && c.TaxCode.isRecoverable)
                .ToList();
        }

        public List<CommercialTax> GetNonRecoveryCommercialTaxesList()
        {
            this.RemoveUnReference();
            return erpNodeDBContext.CommercialTaxes
                .Where(c => c.TaxPeriodId == null)
                .Where(c => c.TaxCode.isRecoverable == false)
                .ToList();
        }


        public List<CommercialTax> ListByDirection(TaxDirection? taxDirection, bool recoveryOnly = true)
        {
            IQueryable<CommercialTax> query = erpNodeDBContext.CommercialTaxes;

            if (recoveryOnly)
                query = query.Where(ct => ct.TaxCode.isRecoverable);

            if (taxDirection != null)
                query = query.Where(ct => ct.TaxDirection == taxDirection);

            return query.ToList();

        }

        public void RemoveUnReference()
        {
            var removingComercialTaxes = erpNodeDBContext.CommercialTaxes
                .Where(ct => ct.TaxCodeId == null || ct.CommercialId == null)
                .ToList();

            erpNodeDBContext.CommercialTaxes.RemoveRange(removingComercialTaxes);
            erpNodeDBContext.SaveChanges();
        }

        public CommercialTax Find(Guid id) => erpNodeDBContext.CommercialTaxes
            .Find(id);
    }
}