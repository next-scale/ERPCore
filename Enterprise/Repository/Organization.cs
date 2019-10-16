using ERPCore.Enterprise.DataBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ERPCore.Enterprise.Models.Company;
using ERPCore.Enterprise.Models.Datum;
using ERPCore.Enterprise.Models.Taxes.Enums;
using ERPCore.Enterprise.Models.Transactions;

namespace ERPCore.Enterprise.Repository
{
    public class Organization : IDisposable
    {
        public EnterpriseDbContext erpNodeDBContext { get; private set; }

        private Repository.Accounting.ChartOfAccounts _chartOfAccountDal;
        private Repository.Accounting.SystemAccounts _systemAccountsDal;
        private Repository.Accounting.ChartOfAccountTemplate _ChartOfAccountTemplate;
        private Repository.Accounting.PreviewAccounts _PreviewAccounts;
        private Repository.Accounting.FiscalYears _fiscalYearsDal;
        private Repository.Accounting.Ledgers _Ledgers;
        private Repository.Security.Members _Members;
        private Repository.Company.DataItems _dataItemsDal;
        private Repository.Company.Departments _Departments;
        private Repository.Transactions.Terms.ShippingTerms _ShippingTerms;



        private Repository.Transactions.Terms.ShippingMethods _ShippingMethods;
        private Repository.Transactions.Terms.PaymentTerms _PaymentTerms;
        private Repository.Profiles.Profiles _Profiles;
        private Repository.Profiles.ProfileAddresses _ProfileAddresses;
        private Repository.Profiles.ProfileBankAccounts _ProfileBankAccounts;
        private Repository.Profiles.Customers _customers;
        private Repository.Profiles.Suppliers _suppliers;
        private Repository.Profiles.Employees _employees;
        private Repository.Profiles.Investors _investors;
        private Repository.Items.Items _Items;
        private Repository.Items.ItemGroups _ItemGroups;
        private Repository.Items.Brands _Brands;
        private Repository.Items.InventoryItems _inventoryItemsDal;

        private Repository.Items.DataSheets _DataSheets;
        private Repository.Items.ItemParameters _ItemParameters;
        private Repository.Items.ItemParameterTypes _ItemParameterTypes;
        private Repository.Items.ItemImages _ItemImages;
        private Repository.Financial.PaymentRetentions _PaymentRetentions;

        public Repository.Accounting.LedgerGroups _LedgerGroups;
        private Repository.Estimations.SalesEstimates _salesEstimateDal;
        private Repository.Estimations.PurchaseEstimates _purchaseEstimates;
        private Repository.Estimations.EstimateItems _EstimateItems;
        private Repository.Projects.Tasks _Tasks;
        private Repository.Projects.Projects _Projects;
        private Repository.Documents.Documents _Documents;
        private Repository.Transactions.CommercialItems _commercialItemsDal;
        private Repository.Transactions.Commercials _Commercials;
        private Repository.Transactions.Sales _sales;
        private Repository.Transactions.Purchases _purchases;


        private Repository.Financial.GeneralPayments _GeneralPayments;
        private Repository.Financial.ReceivePayments _ReceivePayments;
        private Repository.Financial.SupplierPayments _SupplierPayments;
        private Repository.Financial.LiabilityPayments _LiabilityPayments;



        private Repository.Financial.RetentionTypes _RetentionTypes;
        private Repository.Financial.FundTransfers _FundTransfers;
        private Repository.Financial.Loans _loans;
        private Repository.Financial.Lends _lends;
        private Repository.Investors.CapitalActivities _CapitalActivities;

        private Repository.Taxes.TaxGroups _TaxGroupsDal;
        private Repository.Taxes.TaxCodes _TaxCodesDal;
        private Repository.Taxes.TaxPeriods _TaxPeriods;
        private Repository.Taxes.IncomeTaxes _IncomeTaxes;
        private Repository.Taxes.CommercialTaxes _CommercialTaxes;


        private Repository.Employees.EmployeePayments _EmployeePayments;
        private Repository.Employees.EmployeePaymentPeriods _EmployeePaymentPeriods;
        private Repository.Employees.EmployeePaymentLines _EmployeePaymentLines;
        private Repository.Employees.EmployeePaymentTypes _EmployeePaymentTypes;
        private Repository.Employees.EmployeePaymentTemplates _EmployeePaymentTemplates;
        private Repository.Employees.EmployeePositions _EmployeePositions;

        private Repository.Accounting.JournalEntries _journalEntries;
        private Repository.Accounting.JournalEntryTypes _JournalEntryTypes;
        private Repository.Accounting.JournalEntryItems _JournalEntryItems;

        private Repository.Items.ItemsCOSG _PeriodItemsCOSG;
        private Repository.Assets.FixedAssetTypes _FixedAssetTypes;
        private Repository.Assets.FixedAssets _FixedAssets;

        private Repository.Assets.DeprecateSchedules _DeprecatedSchedules;
        private Repository.Accounting.OpeningEntries _OpeningEntries;


        private Repository.Logistic.Shipments _Shipments;



        private Logging.EventLogs _EventLogs { get; set; }
        public Logging.EventLogs EventLogs
        {
            get
            {
                if (this._EventLogs == null)
                    this._EventLogs = new Logging.EventLogs(this);
                return _EventLogs;
            }
        }

        private Accounting.AccountGroups _AccountGroups { get; set; }
        public Accounting.AccountGroups AccountGroups
        {
            get
            {
                if (this._AccountGroups == null)
                    this._AccountGroups = new Accounting.AccountGroups(this);
                return _AccountGroups;
            }
        }



        public Repository.Accounting.ChartOfAccounts ChartOfAccount
        {
            get
            {
                if (this._chartOfAccountDal == null)
                    this._chartOfAccountDal = new Accounting.ChartOfAccounts(this);
                return _chartOfAccountDal;
            }
        }
        public Repository.Accounting.SystemAccounts SystemAccounts
        {
            get
            {
                if (this._systemAccountsDal == null)
                    this._systemAccountsDal = new Accounting.SystemAccounts(this);
                return _systemAccountsDal;
            }
        }
        public Repository.Accounting.ChartOfAccountTemplate ChartOfAccountTemplate
        {
            get
            {
                if (this._ChartOfAccountTemplate == null)
                    this._ChartOfAccountTemplate = new Accounting.ChartOfAccountTemplate(this);
                return _ChartOfAccountTemplate;
            }
        }
        public Repository.Accounting.PreviewAccounts PreviewAccounts
        {
            get
            {
                if (this._PreviewAccounts == null)
                    this._PreviewAccounts = new Accounting.PreviewAccounts(this);
                return _PreviewAccounts;
            }
        }





        public Repository.Company.DataItems DataItems
        {
            get
            {
                if (this._dataItemsDal == null)
                    this._dataItemsDal = new Company.DataItems(this);
                return _dataItemsDal;
            }
        }
        public Repository.Company.Departments Departments
        {
            get
            {
                if (this._Departments == null)
                    this._Departments = new Company.Departments(this);
                return _Departments;
            }
        }

        public Repository.Security.Members Members
        {
            get
            {
                if (this._Members == null)
                    this._Members = new Repository.Security.Members(this);
                return _Members;
            }
        }

        public Repository.Transactions.Terms.ShippingTerms ShippingTerms
        {
            get
            {
                if (this._ShippingTerms == null)
                    this._ShippingTerms = new Transactions.Terms.ShippingTerms(this);
                return _ShippingTerms;
            }
        }
        public Repository.Transactions.Terms.ShippingMethods ShippingMethods
        {
            get
            {
                if (this._ShippingMethods == null)
                    this._ShippingMethods = new Transactions.Terms.ShippingMethods(this);
                return _ShippingMethods;
            }
        }
        public Repository.Transactions.Terms.PaymentTerms PaymentTerms
        {
            get
            {
                if (this._PaymentTerms == null)
                    this._PaymentTerms = new Transactions.Terms.PaymentTerms(this);
                return _PaymentTerms;
            }
        }



        public Repository.Profiles.Profiles Profiles
        {
            get
            {
                if (this._Profiles == null)
                    this._Profiles = new Repository.Profiles.Profiles(this);
                return _Profiles;
            }
        }

        public Repository.Profiles.ProfileAddresses ProfileAddresses
        {
            get
            {
                if (this._ProfileAddresses == null)
                    this._ProfileAddresses = new Repository.Profiles.ProfileAddresses(this);
                return _ProfileAddresses;
            }
        }

        public Repository.Profiles.ProfileBankAccounts ProfileBankAccounts
        {
            get
            {
                if (this._ProfileBankAccounts == null)
                    this._ProfileBankAccounts = new Repository.Profiles.ProfileBankAccounts(this);
                return _ProfileBankAccounts;
            }
        }


        public Repository.Profiles.Customers Customers
        {
            get
            {
                if (this._customers == null)
                    this._customers = new Repository.Profiles.Customers(this);
                return _customers;
            }
        }
        public Repository.Profiles.Suppliers Suppliers
        {
            get
            {
                if (this._suppliers == null)
                    this._suppliers = new Repository.Profiles.Suppliers(this);
                return _suppliers;
            }
        }
        public Repository.Profiles.Employees Employees
        {
            get
            {
                if (this._employees == null)
                    this._employees = new Repository.Profiles.Employees(this);
                return _employees;
            }
        }
        public Repository.Profiles.Investors Investors
        {
            get
            {
                if (this._investors == null)
                    this._investors = new Repository.Profiles.Investors(this);
                return _investors;
            }
        }


        public Repository.Items.Items Items
        {
            get
            {
                if (this._Items == null)
                    this._Items = new Items.Items(this);
                return _Items;
            }
        }
        public Repository.Items.Brands Brands
        {
            get
            {
                if (this._Brands == null)
                    this._Brands = new Items.Brands(this);
                return _Brands;
            }
        }
        public Repository.Items.DataSheets DataSheets
        {
            get
            {
                if (this._DataSheets == null)
                    this._DataSheets = new Items.DataSheets(this);
                return _DataSheets;
            }
        }
        public Repository.Items.ItemGroups ItemGroups
        {
            get
            {
                if (this._ItemGroups == null)
                    this._ItemGroups = new Items.ItemGroups(this);
                return _ItemGroups;
            }
        }
        public Repository.Items.ItemParameters ItemParameters
        {
            get
            {
                if (this._ItemParameters == null)
                    this._ItemParameters = new Items.ItemParameters(this);
                return _ItemParameters;
            }
        }
        public Repository.Items.ItemParameterTypes ItemParameterTypes
        {
            get
            {
                if (this._ItemParameterTypes == null)
                    this._ItemParameterTypes = new Items.ItemParameterTypes(this);
                return _ItemParameterTypes;
            }
        }
        public Repository.Items.ItemImages ItemImages
        {
            get
            {
                if (this._ItemImages == null)
                    this._ItemImages = new Items.ItemImages(this);
                return _ItemImages;
            }
        }



        public Repository.Items.InventoryItems InventoryItemsDal
        {
            get
            {
                if (this._inventoryItemsDal == null)
                    this._inventoryItemsDal = new Items.InventoryItems(this);
                return _inventoryItemsDal;
            }
        }
      
        public Repository.Financial.PaymentRetentions PaymentRetentions
        {
            get
            {
                if (this._PaymentRetentions == null)
                    this._PaymentRetentions = new Financial.PaymentRetentions(this);
                return _PaymentRetentions;
            }
        }



        public Repository.Taxes.TaxGroups TaxGroups
        {
            get
            {
                if (this._TaxGroupsDal == null)
                    this._TaxGroupsDal = new Taxes.TaxGroups(this);
                return _TaxGroupsDal;
            }
        }

        public Repository.Taxes.TaxCodes TaxCodes
        {
            get
            {
                if (this._TaxCodesDal == null)
                    this._TaxCodesDal = new Taxes.TaxCodes(this);
                return _TaxCodesDal;
            }
        }
        public Repository.Taxes.IncomeTaxes IncomeTaxes
        {
            get
            {
                if (this._IncomeTaxes == null)
                    this._IncomeTaxes = new Taxes.IncomeTaxes(this);
                return _IncomeTaxes;
            }
        }

        public Repository.Taxes.CommercialTaxes CommercialTaxes
        {
            get
            {
                if (this._CommercialTaxes == null)
                    this._CommercialTaxes = new Taxes.CommercialTaxes(this);
                return _CommercialTaxes;
            }
        }

        public Repository.Accounting.FiscalYears FiscalYears
        {
            get
            {
                if (this._fiscalYearsDal == null)
                    this._fiscalYearsDal = new Accounting.FiscalYears(this);
                return _fiscalYearsDal;
            }
        }

        public Repository.Accounting.LedgerGroups LedgerGroups
        {
            get
            {
                if (this._LedgerGroups == null)
                    this._LedgerGroups = new Accounting.LedgerGroups(this);
                return _LedgerGroups;
            }
        }


        public Repository.Accounting.Ledgers LedgersDal
        {
            get
            {
                if (this._Ledgers == null)
                    this._Ledgers = new Accounting.Ledgers(this);
                return _Ledgers;
            }
        }
        public Repository.Transactions.CommercialItems CommercialItems
        {
            get
            {
                if (this._commercialItemsDal == null)
                    this._commercialItemsDal = new Transactions.CommercialItems(this);
                return _commercialItemsDal;
            }
        }


        public Repository.Transactions.Commercials Commercials
        {
            get
            {
                if (this._Commercials == null)
                    this._Commercials = new Transactions.Commercials(this);
                return _Commercials;
            }
        }
        public Repository.Transactions.Sales Sales
        {
            get
            {
                if (this._sales == null)
                    this._sales = new Transactions.Sales(this);
                return _sales;
            }
        }
        public Repository.Transactions.Purchases Purchases
        {
            get
            {
                if (this._purchases == null)
                    this._purchases = new Transactions.Purchases(this);
                return _purchases;
            }
        }

        public Repository.Employees.EmployeePayments EmployeePayments
        {
            get
            {
                if (this._EmployeePayments == null)
                    this._EmployeePayments = new Employees.EmployeePayments(this);
                return _EmployeePayments;
            }
        }

        public Repository.Employees.EmployeePaymentPeriods EmployeePaymentPeriods
        {
            get
            {
                if (this._EmployeePaymentPeriods == null)
                    this._EmployeePaymentPeriods = new Employees.EmployeePaymentPeriods(this);
                return _EmployeePaymentPeriods;
            }
        }
        public Repository.Employees.EmployeePaymentTypes EmployeePaymentTypes
        {
            get
            {
                if (this._EmployeePaymentTypes == null)
                    this._EmployeePaymentTypes = new Employees.EmployeePaymentTypes(this);
                return _EmployeePaymentTypes;
            }
        }
        public Repository.Employees.EmployeePaymentTemplates EmployeePaymentTemplates
        {
            get
            {
                if (this._EmployeePaymentTemplates == null)
                    this._EmployeePaymentTemplates = new Employees.EmployeePaymentTemplates(this);
                return _EmployeePaymentTemplates;
            }
        }
        public Repository.Employees.EmployeePositions EmployeePositions
        {
            get
            {
                if (this._EmployeePositions == null)
                    this._EmployeePositions = new Employees.EmployeePositions(this);
                return _EmployeePositions;
            }
        }
        public Repository.Employees.EmployeePaymentLines EmployeePaymentLines
        {
            get
            {
                if (this._EmployeePaymentLines == null)
                    this._EmployeePaymentLines = new Employees.EmployeePaymentLines(this);
                return _EmployeePaymentLines;
            }
        }

        public Repository.Financial.GeneralPayments GeneralPayments
        {
            get
            {
                if (this._GeneralPayments == null)
                    this._GeneralPayments = new Financial.GeneralPayments(this);
                return _GeneralPayments;
            }
        }


        public Repository.Financial.ReceivePayments ReceivePayments
        {
            get
            {
                if (this._ReceivePayments == null)
                    this._ReceivePayments = new Financial.ReceivePayments(this);
                return _ReceivePayments;
            }
        }








        public Repository.Financial.SupplierPayments SupplierPayments
        {
            get
            {
                if (this._SupplierPayments == null)
                    this._SupplierPayments = new Financial.SupplierPayments(this);
                return _SupplierPayments;
            }
        }
        public Repository.Financial.LiabilityPayments LiabilityPayments
        {
            get
            {
                if (this._LiabilityPayments == null)
                    this._LiabilityPayments = new Financial.LiabilityPayments(this);
                return _LiabilityPayments;
            }
        }
        public Repository.Financial.FundTransfers FundTransfers
        {
            get
            {
                if (this._FundTransfers == null)
                    this._FundTransfers = new Financial.FundTransfers(this);
                return _FundTransfers;
            }
        }


        public Repository.Financial.RetentionTypes RetentionTypes
        {
            get
            {
                if (this._RetentionTypes == null)
                    this._RetentionTypes = new Financial.RetentionTypes(this);
                return _RetentionTypes;
            }
        }


        public Repository.Investors.CapitalActivities CapitalActivities
        {
            get
            {
                if (this._CapitalActivities == null)
                    this._CapitalActivities = new Investors.CapitalActivities(this);
                return _CapitalActivities;
            }
        }
        public Repository.Taxes.TaxPeriods TaxPeriods
        {
            get
            {
                if (this._TaxPeriods == null)
                    this._TaxPeriods = new Taxes.TaxPeriods(this);
                return _TaxPeriods;
            }
        }
        public Repository.Financial.Loans Loans
        {
            get
            {
                if (this._loans == null)
                    this._loans = new Financial.Loans(this);
                return _loans;
            }
        }
        public Repository.Financial.Lends Lends
        {
            get
            {
                if (this._lends == null)
                    this._lends = new Financial.Lends(this);
                return _lends;
            }
        }

        public Repository.Accounting.JournalEntries JournalEntries
        {
            get
            {
                if (this._journalEntries == null)
                    this._journalEntries = new Accounting.JournalEntries(this);
                return _journalEntries;
            }
        }
        public Repository.Accounting.JournalEntryTypes JournalEntryTemplates
        {
            get
            {
                if (this._JournalEntryTypes == null)
                    this._JournalEntryTypes = new Accounting.JournalEntryTypes(this);
                return _JournalEntryTypes;
            }
        }
        public Repository.Accounting.JournalEntryItems JournalEntryItems
        {
            get
            {
                if (this._JournalEntryItems == null)
                    this._JournalEntryItems = new Accounting.JournalEntryItems(this);
                return _JournalEntryItems;
            }
        }
        public Repository.Items.ItemsCOSG ItemsCOGS
        {
            get
            {
                if (this._PeriodItemsCOSG == null)
                    this._PeriodItemsCOSG = new Items.ItemsCOSG(this);
                return _PeriodItemsCOSG;
            }
        }
        public Repository.Assets.FixedAssets FixedAssets
        {
            get
            {
                if (this._FixedAssets == null)
                    this._FixedAssets = new Assets.FixedAssets(this);
                return _FixedAssets;
            }
        }
        public Repository.Assets.FixedAssetTypes FixedAssetTypes
        {
            get
            {
                if (this._FixedAssetTypes == null)
                    this._FixedAssetTypes = new Assets.FixedAssetTypes(this);
                return _FixedAssetTypes;
            }
        }





        public Repository.Assets.DeprecateSchedules FixedAssetDreprecateSchedules
        {
            get
            {
                if (this._DeprecatedSchedules == null)
                    this._DeprecatedSchedules = new Assets.DeprecateSchedules(this);
                return _DeprecatedSchedules;
            }
        }
        public Repository.Accounting.OpeningEntries OpeningEntries
        {
            get
            {
                if (this._OpeningEntries == null)
                    this._OpeningEntries = new Accounting.OpeningEntries(this);
                return _OpeningEntries;
            }
        }

        public Estimations.SalesEstimates SalesEstimates
        {
            get
            {
                if (this._salesEstimateDal == null)
                    this._salesEstimateDal = new Estimations.SalesEstimates(this);
                return _salesEstimateDal;
            }
        }
        public Estimations.PurchaseEstimates PurchaseEstimates
        {
            get
            {
                if (this._purchaseEstimates == null)
                    this._purchaseEstimates = new Estimations.PurchaseEstimates(this);
                return _purchaseEstimates;
            }
        }
        public Estimations.EstimateItems EstimateItems
        {
            get
            {
                if (this._EstimateItems == null)
                    this._EstimateItems = new Estimations.EstimateItems(this);
                return _EstimateItems;
            }
        }
        public Projects.Tasks Tasks
        {
            get
            {
                if (this._Tasks == null)
                    this._Tasks = new Projects.Tasks(this);
                return _Tasks;
            }
        }
        public Projects.Projects Projects
        {
            get
            {
                if (this._Projects == null)
                    this._Projects = new Projects.Projects(this);
                return _Projects;
            }
        }
        public Documents.Documents Documents
        {
            get
            {
                if (this._Documents == null)
                    this._Documents = new Documents.Documents(this);
                return _Documents;
            }
        }

        private Repository.Logistic.Shipments Shipments
        {
            get
            {
                if (this._Shipments == null)
                    this._Shipments = new Logistic.Shipments(this);
                return _Shipments;
            }
        }


        public string DatabaseName { get; private set; }
        public void Dispose()
        {
            this.erpNodeDBContext.Dispose();
        }
        public void SaveChanges()
        {
            Console.WriteLine("> Final save change.");
            erpNodeDBContext.SaveChanges();
        }
        public Organization(string dbName)
        {
            this.DatabaseName = dbName;
            this.erpNodeDBContext = new EnterpriseDbContext(dbName);
        }

        public Models.Profiles.Profile SelfProfile => erpNodeDBContext.Profiles
            .Where(p => p.isSelfOrganization == true)
            .FirstOrDefault();



        public void UpdateProfileName() => this.DataItems.Set(DataItemKey.OrganizationHeader, this.SelfProfile.DisplayHeader.Trim());


        public Company.ExternalData GetExternalData()
        {
            var data = new Company.ExternalData()
            {
                Name = DataItems.OrganizationName,
                TaxId = DataItems.TaxID,
                MemberCount = erpNodeDBContext.Profiles.Where(p => p.AccessLevel != Models.Security.Enums.AccessLevel.None).Count(),
                OpenTaskCount = erpNodeDBContext.Tasks.Count()
            };
            return data;
        }

        public void CreateCompanyDB(Models.Company.NewCompanyModel newCompany)
        {
            this.SetFirstDate(newCompany.FirstDate);
            this.CreateSelfProfile(newCompany);

            var defaultAccountsDal = new Repository.Accounting.ChartOfAccountTemplate(this);
            this.ChartOfAccountTemplate.CreateAccounts();
            this.CreateSystemAccounts();
            this.CreateFiscalYear(newCompany);
        }
        private void CreateFiscalYear(NewCompanyModel newCompany)
        {
            var fiscalYearDal = new Repository.Accounting.FiscalYears(this);
            fiscalYearDal.Create(newCompany.FirstDate);
            fiscalYearDal.Create(DateTime.Today);
        }
        private void CreateSystemAccounts()
        {
            Console.WriteLine("> Create System Accounts");

            if (this.DataItems.Get(Models.Datum.DataItemKey.DefaultSystemAccountAssign) == null)
            {
                this.SystemAccounts.AutoAssignSystemAccount();
                this.DataItems.Set(Models.Datum.DataItemKey.DefaultSystemAccountAssign, true.ToString());
            }
        }

        public DateTime FirstDate => this.DataItems.FirstDate;



        private void SetFirstDate(DateTime firstDate)
        {
            Console.WriteLine("> Set FirstDate");
            DateTime verifyFirstDate = new DateTime(DateTime.Today.Year, firstDate.Month, firstDate.Day);


            if (this.DataItems.Get(Models.Datum.DataItemKey.FirstDate) == null)
                this.DataItems.FirstDate = verifyFirstDate;

            this.erpNodeDBContext.SaveChanges();
        }
        private void CreateSelfProfile(Models.Company.NewCompanyModel newCompany)
        {
            Console.WriteLine("> Create Self Organization Profile");

            if (this.DataItems.Get(Models.Datum.DataItemKey.OrganizationName) == null)
            {
                this.DataItems.Set(Models.Datum.DataItemKey.OrganizationName, newCompany.Name);
                this.DataItems.Set(Models.Datum.DataItemKey.TaxId, newCompany.TaxID);

                var profilesDal = new Repository.Profiles.Profiles(this);

                var erpProfile = new Models.Profiles.Profile()
                {
                    Id = Guid.NewGuid(),
                    ProfileType = Models.Profiles.ProfileType.Organization,
                    Name = newCompany.Name,
                    CreatedDate = DateTime.Today,
                    localizedLanguage = Models.Profiles.EnumLanguage.en,
                    TaxNumber = newCompany.TaxID,
                    isSelfOrganization = true
                };

                this.erpNodeDBContext.Profiles.Add(erpProfile);
                this.erpNodeDBContext.SaveChanges();
            }
        }
    }
}
