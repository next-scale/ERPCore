using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ERPCore.Enterprise.Models.Items;
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Accounting.FiscalYears;
using ERPCore.Enterprise.Models.Accounting.Enums;

namespace ERPCore.Enterprise.Models.Accounting
{
    [Table("ERP_Fiscal_Years")]
    public class FiscalYear
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? PreviousFiscalId { get; set; }
        [ForeignKey("PreviousFiscalId")]
        public virtual FiscalYear PreviousFiscal { get; set; }
        public string Name => string.Format("{0}", this.EndDate.Year.ToString());

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate => this.StartDate.AddYears(1).AddDays(-1);

        public EnumFiscalYearStatus Status { get; set; }
        public String Memo { get; set; }

        public Decimal Income { get; set; }
        public Decimal Expense { get; set; }


        public Decimal AssetBalance => this.PeriodAccountBalances?
            .Where(p => p.Account.Type == AccountTypes.Asset)
            .Select(l => l.Balance)
            .DefaultIfEmpty(0)
            .Sum() ?? 0;

        public Decimal LiabilityBalance => this.PeriodAccountBalances?
            .Where(p => p.Account.Type == AccountTypes.Liability)
            .Select(l => l.Balance)
            .DefaultIfEmpty(0)
            .Sum() ?? 0;

        public Decimal EquityBalance => this.PeriodAccountBalances?
            .Where(p => p.Account.Type == AccountTypes.Capital)
            .Select(l => l.Balance)
            .DefaultIfEmpty(0)
            .Sum() ?? 0 + this.Profit;

        public Decimal RightBalance => this.EquityBalance + this.LiabilityBalance;

        public Decimal Profit => this.Income - this.Expense;

        public Decimal? ProfitPercent
        {
            get
            {
                if (this.Income == 0)
                    return null;
                else
                    return (((this.Income - this.Expense) / this.Income) * 100);
            }
        }



        public LedgerPostStatus PostStatus { get; set; }

        public Guid? ClosingAccountGuid { get; set; }
        [ForeignKey("ClosingAccountGuid")]
        public virtual Account ClosingAccount { get; set; }

        [Column("ClosingEntryCalDate")]
        public DateTime? ClosingAccountsCalculateDateTime { get; set; }
        [Column("OpeningEntryCalDate")]
        public DateTime? OpeningAccountsCalculateDateTime { get; set; }

        public virtual ICollection<FiscalYears.OpeningEntry> OpeningEntries { get; set; }
        public virtual ICollection<FiscalYears.PeriodAccountBalance> PeriodAccountBalances { get; set; }


        public virtual ICollection<FiscalYears.PeriodItemCOGS> PeriodItemsCOGS { get; set; }
        public int DayCount => (this.EndDate - this.StartDate).Days + 1;

        public bool IsPostClosingEntriesLedger { get; set; }

        public FiscalYear()
        {
            this.Id = Guid.NewGuid();
        }

        public void ClearOpeningAccountsBalance()
        {
            this.PeriodAccountBalances.ToList().ForEach(pab =>
            {
                pab.OpeningDebit = 0;
                pab.OpeningCredit = 0;
            });
        }
        public void SetOpenBalance(Account account, decimal debit, decimal credit)
        {
            var exPab = this.PeriodAccountBalances
                                    .Where(pab => pab.AccountGuid == account.Id)
                                    .FirstOrDefault();

            if (exPab == null)
                this.CreateOpenBalance(account, debit, credit);
            else
                exPab.UpdateOpenBalance(debit, credit);
        }
        private bool CreateOpenBalance(Account account, decimal debit, decimal credit)
        {
            var existPeriodAccountBalance = this.PeriodAccountBalances
                .Where(p => p.AccountGuid == account.Id);

            if (existPeriodAccountBalance != null)
                return false;

            var newPab = new FiscalYears.PeriodAccountBalance()
            {
                Id = Guid.NewGuid(),
                Account = account,
                OpeningCredit = credit,
                OpeningDebit = debit,
            };
            this.PeriodAccountBalances.Add(newPab);

            return true;
        }

        internal bool CopyAccountsBalanceFromPrevius()
        {
            this.ClearOpeningAccountsBalance();

            if (this.PreviousFiscal != null)
            {
                this.PreviousFiscal.UpdateProfit();
                this.PreviousFiscal.PeriodAccountBalances
                    .Where(p => p.Account.Type != AccountTypes.Income)
                    .Where(p => p.Account.Type != AccountTypes.Expense)
                    .ToList().ForEach(pre =>
                    {
                        this.SetOpenBalance(pre.Account, pre.TotalDebit, pre.TotalCredit);

                        if (this.PreviousFiscal.ClosingAccountGuid == pre.AccountGuid)
                        {
                            if (this.PreviousFiscal.Profit > 0)
                                this.SetOpenBalance(pre.Account, pre.TotalDebit, pre.TotalCredit + this.PreviousFiscal.Profit);
                            else
                                this.SetOpenBalance(pre.Account, pre.TotalDebit + this.PreviousFiscal.Profit, pre.TotalCredit);
                        }
                    });

                return true;
            }



            return false;
        }


        internal void PreparePeriodAccountBalance(List<Account> accounts)
        {
            accounts.ForEach(account =>
            {
                var periodAccountBalance = this.PeriodAccountBalances
                 .Where(p => p.AccountGuid == account.Id)
                 .FirstOrDefault();

                if (periodAccountBalance == null)
                    this.CreateAccountBalance(account);
            });
        }
        private bool CreateClosingBalance(Account account, decimal debit, decimal credit)
        {
            var existPeriodAccountBalance = this.PeriodAccountBalances
                .Where(p => p.AccountGuid == account.Id);

            if (existPeriodAccountBalance != null)
                return false;

            var newPab = new FiscalYears.PeriodAccountBalance()
            {
                Id = Guid.NewGuid(),
                Account = account,
                PeriodCredit = credit,
                PeriodDebit = debit,
            };
            this.PeriodAccountBalances.Add(newPab);

            return true;
        }
        public void UpdatePeriodBalance(Account account, decimal debit, decimal credit)
        {
            var exPab = this.PeriodAccountBalances
                                    .Where(pab => pab.AccountGuid == account.Id)
                                    .FirstOrDefault();

            if (exPab == null)
                this.CreateClosingBalance(account, debit, credit);
            else
                exPab.UpdateClosingBalance(debit, credit);
        }

        public void UpdateProfit()
        {
            this.Income = this.PeriodAccountBalances
               .Where(a => a.Account.Type == AccountTypes.Income)
               .Sum(c => c.Balance);

            this.Expense = this.PeriodAccountBalances
                .Where(a => a.Account.Type == AccountTypes.Expense)
                .Sum(c => c.Balance);
        }
        internal void CreateAccountBalance(Account account)
        {
            var newPeriodAccountBlance = new FiscalYears.PeriodAccountBalance()
            {
                Id = Guid.NewGuid(),
                Account = account,
                AccountGuid = account.Id,
            };
            this.PeriodAccountBalances.Add(newPeriodAccountBlance);
        }

        internal void ResetPeriodAccountsBalance()
        {
            if (this.IsPostClosingEntriesLedger)
                return;

            this.PeriodAccountBalances.ToList()
                .ForEach(pab => pab.ResetPeriodBalance());

            this.ClosingAccountsCalculateDateTime = null;
        }
    }
}