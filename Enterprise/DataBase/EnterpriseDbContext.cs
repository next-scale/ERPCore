
using ERPCore.Enterprise.Models.Logging;
using Microsoft.EntityFrameworkCore;
using System;


namespace ERPCore.Enterprise.DataBase
{

    public class EnterpriseDbContext : DbContext
    {
        public String _dbName { get; private set; }
        private String _connectionString { get; set; }
        private string _dbUser { get; set; }
        private string _dbPassword { get; set; }
        private string _dbHost { get; set; }

        public EnterpriseDbContext() : base()
        {

        }

        public EnterpriseDbContext(string siteName) : base()
        {
            _dbName = siteName;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            this._dbName = "ERP.E1".ToUpper().Trim();
            this._dbUser = "sa";
            this._dbPassword = "B0nu3dv4";
            this._dbHost = "172.17.101.97";

            _connectionString = $"Server={_dbHost};Database={_dbName};User Id={_dbUser};Password={_dbPassword};";
            optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        public DbSet<Models.Datum.DataItem> DataItems { get; set; }
        public DbSet<Models.Online.ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<Models.Equity.CapitalActivity> CapitalActivities { get; set; }
        public DbSet<Models.Security.Member> Members { get; set; }


        public DbSet<Models.Items.Item> Items { get; set; }
        public DbSet<Models.Items.ItemGroup> ItemGroups { get; set; }
        public DbSet<Models.Items.Brand> Brands { get; set; }
        public DbSet<Models.Items.ItemImage> ItemImages { get; set; }
        public DbSet<Models.Items.DataSheet> DataSheets { get; set; }
        public DbSet<Models.Items.ItemParameterType> ItemParameterTypes { get; set; }
        public DbSet<Models.Items.ItemParameter> ItemParameters { get; set; }
        public DbSet<Models.Items.PriceRange> PriceRanges { get; set; }
        public DbSet<Models.Items.PriceRangeItem> PriceRangeItems { get; set; }


        public DbSet<Models.Accounting.FiscalYear> FiscalYears { get; set; }
        public DbSet<Models.Accounting.FiscalYears.PeriodAccountBalance> PeriodAccountsBalance { get; set; }
        public DbSet<Models.Accounting.FiscalYears.OpeningEntry> OpeningEntries { get; set; }
        public DbSet<Models.Accounting.FiscalYears.PeriodItemCOGS> PeriodItemsCOGS { get; set; }
        public DbSet<Models.ChartOfAccount.Account> Accounts { get; set; }
        public DbSet<Models.ChartOfAccount.DefaultAccount> DefaultAccounts { get; set; }
        public DbSet<Models.ChartOfAccount.PreviewAccount> PreviewAccounts { get; set; }
        public DbSet<Models.ChartOfAccount.HistoryBalanceItem> HistoryBalanceItems { get; set; }
        public DbSet<Models.Accounting.LedgerGroup> LedgerGroups { get; set; }
        public DbSet<Models.Accounting.LedgerLine> Ledgers { get; set; }
        public DbSet<Models.AccountingEntries.JournalEntryType> JournalEntryTypes { get; set; }
        public DbSet<Models.AccountingEntries.JournalEntry> JournalEntries { get; set; }
        public DbSet<Models.AccountingEntries.JournalEntryLine> JournalEntryLines { get; set; }




        #region Estimations
        public DbSet<Models.Estimations.Estimate> Estimates { get; set; }
        public DbSet<Models.Estimations.SalesEstimate> SalesEstimates { get; set; }
        public DbSet<Models.Estimations.PurchaseEstimate> PurchaseEstimates { get; set; }
        public DbSet<Models.Estimations.EstimateItem> EstimateItems { get; set; }
        public DbSet<Models.Estimations.EstimationTax> EstimateTaxes { get; set; }
        #endregion



        #region Role
        public DbSet<Models.Equity.Investor> Investors { get; set; }
        public DbSet<Models.Customers.Customer> Customers { get; set; }
        public DbSet<Models.Suppliers.Supplier> Suppliers { get; set; }
        #endregion




        #region Profiles
        public DbSet<Models.Profiles.Profile> Profiles { get; set; }
        public DbSet<Models.Profiles.ProfileGroup> ProfileGroups { get; set; }
        public DbSet<Models.Profiles.ProfileBankAccount> ProfileBankAccounts { get; set; }
        public DbSet<Models.Profiles.ProfileAddress> ProfileAddresses { get; set; }
        public DbSet<Models.Profiles.HistoryItem> HistoryItems { get; set; }
        #endregion





        #region Commercial
        public DbSet<Models.Transactions.Commercial> Commercials { get; set; }
        public DbSet<Models.Transactions.Commercials.Sale> Sales { get; set; }
        public DbSet<Models.Transactions.Commercials.Purchase> Purchases { get; set; }
        public DbSet<Models.Transactions.Commercials.PaymentTerm> PaymentTerms { get; set; }
        public DbSet<Models.Transactions.Commercials.ShippingTerm> ShippingTerms { get; set; }
        public DbSet<Models.Transactions.Commercials.ShippingMethod> ShippingMethods { get; set; }
        public DbSet<Models.Transactions.CommercialItem> CommercialItems { get; set; }
        public DbSet<Models.Transactions.CommercialTax> CommercialTaxes { get; set; }
        public DbSet<Models.Logistic.Shipment> Shipments { get; set; }
        public DbSet<Models.Transactions.CommercialDailyBalance> CommercialDailyBalances { get; set; }

        #endregion





        public DbSet<Models.Departments.Department> Departments { get; set; }
        public DbSet<Models.Projects.Project> Projects { get; set; }
        public DbSet<Models.Tasks.Task> Tasks { get; set; }
        public DbSet<Models.Projects.Material> Materials { get; set; }


        public DbSet<Models.Assets.FixedAsset> FixedAssets { get; set; }
        public DbSet<Models.Assets.FixedAssetType> FixedAssetTypes { get; set; }
        public DbSet<Models.Assets.DeprecateSchedule> DeprecateSchedules { get; set; }





        #region Financial
        public DbSet<Models.Financial.Transfer.FundTransfer> FundTransfers { get; set; }
        public DbSet<Models.Financial.Payments.RetentionType> RetentionTypes { get; set; }
        public DbSet<Models.Financial.Loans.Loan> Loans { get; set; }
        public DbSet<Models.Financial.Loans.LoanPayment> LoanPayments { get; set; }
        public DbSet<Models.Financial.Lends.Lend> Lends { get; set; }
        public DbSet<Models.Financial.Lends.LendPayment> LendPayments { get; set; }
        public DbSet<Models.Financial.Payments.GeneralPayment> GeneralPayments { get; set; }
        public DbSet<Models.Financial.Payments.LiabilityPayment> LiabilityPayments { get; set; }
        public DbSet<Models.Financial.Payments.ReceivePayment> ReceivePayments { get; set; }
        public DbSet<Models.Financial.Payments.SupplierPayment> SupplierPayments { get; set; }
        public DbSet<Models.Financial.Payments.PaymentRetention> PaymentRetentions { get; set; }
        public DbSet<Models.Financial.Payments.PaymentFromAccount> PaymentFromAccounts { get; set; }


        #endregion






        #region Taxes
        public DbSet<Models.Taxes.TaxGroup> TaxGroups { get; set; }
        public DbSet<Models.Taxes.TaxCode> TaxCodes { get; set; }
        public DbSet<Models.Taxes.TaxRate> TaxRate { get; set; }
        public DbSet<Models.Taxes.IncomeTax> IncomeTaxs { get; set; }
        public DbSet<Models.Taxes.TaxPeriod> TaxPeriods { get; set; }
        #endregion




        #region Employees
        public DbSet<Models.Employees.Employee> Employees { get; set; }
        public DbSet<Models.Employees.EmployeePosition> EmployeePositions { get; set; }
        public DbSet<Models.Employees.EmployeePaymentPeriod> EmployeePaymentPeriods { get; set; }
        public DbSet<Models.Employees.EmployeePaymentType> EmployeePaymentTypes { get; set; }
        public DbSet<Models.Employees.EmployeePaymentItem> EmployeePaymentLines { get; set; }
        public DbSet<Models.Employees.EmployeePayment> EmployeePayments { get; set; }
        public DbSet<Models.Employees.EmployeeLeave> EmployeeLeaves { get; set; }
        public DbSet<Models.Employees.EmployeePaymentTemplate> EmployeePaymentTemplates { get; set; }
        public DbSet<Models.Employees.EmployeePaymentTemplateItem> EmployeePaymentTemplateItems { get; set; }
        #endregion


        public DbSet<Models.Documents.Document> Documents { get; set; }
        public DbSet<EventLog> EventLogs { get; set; }
    }


}