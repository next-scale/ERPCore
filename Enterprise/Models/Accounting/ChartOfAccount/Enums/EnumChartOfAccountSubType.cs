using System.ComponentModel.DataAnnotations;

namespace ERPCore.Enterprise.Models.ChartOfAccount
{
    public enum AccountSubTypes
    {
        None = 0,

        /// <summary>
        /// Asset Types
        /// </summary>
        /// 
        Cash = 100,
        Bank = 120,
        ShotTermLending = 125,
        LongTermLending = 126,
        ShotTermInvestment = 130,
        OtherCurrentAsset = 140,
        Inventory = 160,
        OtherAsset = 180,
        AccountReceivable = 170,
        FixedAsset = 190,
        AccDepreciation = 198,
        EarnestPayment = 191,
        TaxInput = 181,
        TaxReceivable = 183,


        /// <summary>
        /// Liability Type
        /// </summary>
        PayrollLiability = 220,
        CurrentLiability = 240,
        LongTermLiability = 260,
        AccountPayable = 270,
        TaxOutput = 281,
        TaxPayable = 282,


        EarnestReceive = 291,
        OverReceivePayment = 292,
        UnpaidCheck = 295,
        PendingItemReceipts = 298,

        /// <summary>
        /// Equity Type
        /// </summary>
        Equity = 300,
        Stock = 301,
        RetainEarning = 302,
        OverStockValue = 303,
        OpeningBalance = 304,
        Dividend = 305,


        /// <summary>
        /// Income
        /// </summary>
        Income = 400,
        Interest = 402,
        OtherIncome = 450,
        DiscountTaken = 452,



        /// <summary>
        /// Expense Type
        /// </summary>
        Expense = 500,
        OtherExpense = 550,
        CostOfGoodsSold = 570,
        PayrollExpense = 580,
        DiscountGiven = 555,
        BankFee = 556,
        TaxExpense = 560,
        IncomeTax = 563,

        IncomeTaxExpense = 562,
        AccumulateExpense = 561,


        AmortizeExpense = 582,
        AwaitingDepreciation = 583,
        InvestmentRevenue = 584,
    }
}
