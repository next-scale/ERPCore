using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.ChartOfAccount;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace ERPCore.Enterprise.Models.Accounting.FiscalYears
{
    [Table("ERP_FiscalYear_ClosingAccounts")]
    public class PeriodAccountBalance
    {
        public PeriodAccountBalance()
        {

        }

        [Key]
        public Guid Id { get; set; }

        public Guid? AccountGuid { get; set; }
        [ForeignKey("AccountGuid")]
        public virtual Account Account { get; set; }


        public LedgerPostStatus PostStatus { get; set; }
        public Guid? FiscalYearId { get; set; }
        [ForeignKey("FiscalYearId")]
        public virtual ERPCore.Enterprise.Models.Accounting.FiscalYear FiscalYear { get; set; }
        public decimal OpeningCredit { get; set; }
        public decimal OpeningDebit { get; set; }


        [Column("Debit")]
        public Decimal PeriodDebit { get; set; }
        [Column("Credit")]
        public Decimal PeriodCredit { get; set; }


        public Decimal TotalPeriodDebit
        {
            get
            {
                var totalPeriodDebit = PeriodDebit - PeriodCredit;
                if (totalPeriodDebit < 0)
                    totalPeriodDebit = 0;

                return totalPeriodDebit;
            }
        }
        public Decimal TotalPeriodCredit
        {
            get
            {
             
               var totalPeriodCredit = PeriodCredit - PeriodDebit;
                if (totalPeriodCredit < 0)
                    totalPeriodCredit = 0;
                return totalPeriodCredit;
            }
        }



        public virtual Decimal PeriodBalance
        {
            get
            {
                switch (this.Account.Type)
                {
                    case AccountTypes.Asset:
                    case AccountTypes.Expense:
                        return (this.TotalPeriodDebit) - (this.TotalPeriodCredit);

                    case AccountTypes.Liability:
                    case AccountTypes.Capital:
                    case AccountTypes.Income:
                        return (this.TotalPeriodCredit) - (this.TotalPeriodDebit);
                    default:
                        return 0;
                }
            }
        }









        public Decimal TotalDebit
        {
            get
            {
                var totalDebit = this.OpeningDebit + this.PeriodDebit;
                var totalCredit = this.OpeningCredit + this.PeriodCredit;
                totalDebit = totalDebit - totalCredit;
                if (totalDebit < 0)
                    totalDebit = 0;

                return totalDebit;
            }
        }
        public Decimal TotalCredit
        {
            get
            {
                var totalDebit = this.OpeningDebit + this.PeriodDebit;
                var totalCredit = this.OpeningCredit + this.PeriodCredit;
                totalCredit = totalCredit - totalDebit;
                if (totalCredit < 0)
                    totalCredit = 0;
                return totalCredit;
            }
        }


        public Decimal? Multiplier { get; set; }


        [Column(TypeName = "Money")]
        [DisplayFormat(DataFormatString = "N2")]
        public virtual Decimal Balance
        {
            get
            {
                switch (this.Account.Type)
                {
                    case AccountTypes.Asset:
                    case AccountTypes.Expense:
                        return (this.TotalDebit) - (this.TotalCredit);

                    case AccountTypes.Liability:
                    case AccountTypes.Capital:
                    case AccountTypes.Income:
                        return (this.TotalCredit) - (this.TotalDebit);
                    default:
                        return 0;
                }
            }
        }



        public PeriodAccountBalance(TempClosingEntry closingAccount, FiscalYear fiscalYear)
        {
            Id = closingAccount.Id;
            AccountGuid = closingAccount.AccountGuid;
            Account = closingAccount.Account;

            OpeningCredit = closingAccount.OpeningCredit;
            OpeningDebit = closingAccount.OpeningDebit;

            PeriodCredit = closingAccount.Credit;
            PeriodDebit = closingAccount.Debit;

            FiscalYearId = closingAccount.FiscalYearId;
            FiscalYear = fiscalYear;
        }

        internal void UpdateOpenBalance(decimal debit, decimal credit)
        {
            this.OpeningDebit = debit;
            this.OpeningCredit = credit;

            this.CalculateBalance();
        }

        internal void UpdateClosingBalance(decimal debit, decimal credit)
        {
            this.PeriodDebit = debit;
            this.PeriodCredit = credit;

            this.CalculateBalance();
        }

        internal void ResetPeriodBalance()
        {
            this.PeriodDebit = 0;
            this.PeriodCredit = 0;
        }

        internal void CalculateBalance()
        {
            this.OpeningDebit = this.OpeningDebit - this.OpeningCredit;
            this.OpeningCredit = 0;
            if (this.OpeningDebit < 0)
            {
                this.OpeningCredit = Math.Abs(this.OpeningDebit);
                this.OpeningDebit = 0;
            }

            this.PeriodDebit = this.PeriodDebit - this.PeriodCredit;
            this.PeriodCredit = 0;

            if (this.PeriodDebit < 0)
            {
                this.PeriodCredit = Math.Abs(this.PeriodDebit);
                this.PeriodDebit = 0;
            }


        }
    }
}
