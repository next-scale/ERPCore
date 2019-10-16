using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Models.Accounting.Enums
{
    public enum TransactionTypes
    {
        NotSet = 0,
        Sale = 30,

        Purchase = 60,

        FixedAsset = 95,
        EmployeePayment = 110,
        CapitalActivity = 120,
        OpeningEntry = 125,
        FiscalYearClosing = 130,
        FundTransfer = 131,


        PaySalesTax = 135,
        OpeningInventoryItem = 145,
        OpeningFixedAsset = 150,
        Loan = 160,
        LoanPayment = 161,
        Lend = 170,
        LendPayment = 162,
        JournalEntry = 190,
        TaxPeriod = 200,
        IncomeTax = 210,
        EarnestPayment = 220,
        EarnestReceive = 230,
        FixedAssetUsage = 232,
        InputTax = 233,
        OutputTax = 243,
        Expense = 244,



        GeneralPayment = 240,
        SupplierPayment = 245,
        LiabilityPayment = 246,
        ReceivePayment = 248,
        CommercialPayment = 249,


        SalesReturn = 250,
        RetentionPayment = 251,
        Asset = 252,
        PurchaseReturn = 254,
        ItemCOGS = 255,
        DeprecateSchedule = 256,
        FIFOCost = 257,
        CommercialTax = 258,
        SalesEstimate = 259,
        PurchaseEstimate = 260,
        TaxPayment = 261
    }
}