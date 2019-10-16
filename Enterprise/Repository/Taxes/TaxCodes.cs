using ERPCore.Enterprise.Models.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Models.Taxes;
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Accounting.Enums;

namespace ERPCore.Enterprise.Repository.Taxes
{
    public class TaxCodes : ERPNodeDalRepository
    {

        public TaxCodes(Organization organization) : base(organization)
        {

        }


        public List<TaxCode> ListAll => erpNodeDBContext.TaxCodes.ToList();
        public IQueryable<TaxCode> All => erpNodeDBContext.TaxCodes;

        public List<TaxCode> GetList(TransactionTypes trType)
        {
            switch (trType)
            {
                case TransactionTypes.Sale:
                case TransactionTypes.PurchaseReturn:
                    return this.ListOutputTax;

                case TransactionTypes.Purchase:
                case TransactionTypes.SalesReturn:
                    return this.ListInputTax;
            }

            throw new NotImplementedException("Transaction Type Not Define");
        }


        public List<TaxCode> ListInputTax => erpNodeDBContext
            .TaxCodes.Where(t => t.TaxDirection == Models.Taxes.Enums.TaxDirection.Input)
            .ToList();

        public List<TaxCode> ListOutputTax => erpNodeDBContext.TaxCodes
            .Where(t => t.TaxDirection == Models.Taxes.Enums.TaxDirection.Output)
            .ToList();

        public TaxCode Find(Guid? TaxCodeGuid)
        {
            if (TaxCodeGuid == null)
                return null;
            else
                return erpNodeDBContext.TaxCodes.Find(TaxCodeGuid);
        }

        public TaxCode CreateNew(string name)
        {
            if (name == null)
                name = "New TaxCode";

            var newTaxCode = new TaxCode()
            {
                Id = Guid.NewGuid(),
                Name = name
            };

            erpNodeDBContext.TaxCodes.Add(newTaxCode);
            organization.SaveChanges();
            return newTaxCode;
        }

        public TaxCode GetDefault => erpNodeDBContext.TaxCodes.Where(t => t.isDefault).FirstOrDefault();

        public TaxCode GetDefaultInput => erpNodeDBContext.TaxCodes.Where(t => t.TaxDirection == Models.Taxes.Enums.TaxDirection.Input).FirstOrDefault();
        public TaxCode GetDefaultOuput => erpNodeDBContext.TaxCodes.Where(t => t.TaxDirection == Models.Taxes.Enums.TaxDirection.Output).FirstOrDefault();

        public void Remove(TaxCode taxCode)
        {
            erpNodeDBContext.TaxCodes.Remove(taxCode);
            organization.SaveChanges();
        }
    }
}