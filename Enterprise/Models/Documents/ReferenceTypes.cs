using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPCore.Enterprise.Models.Documents
{
    public enum ReferenceTypes
    {
        NotSet = 0,
        Quotation = 10,
        Sale = 30,
        CashSale = 35,
        PurchaseOrder = 40,
        ReceivedItems = 50,
        Purchase = 60,
        CashPurchase = 65,
        SalesReturn = 70,
        PurchaseReturn = 80,
        FixedAsset = 95,
        VendorQuotation = 100,


        EmployeePayment = 110,
        CapitalActivity = 120,
        ClosingYear = 130,
        FundTransfer = 131,
        CashDeposit = 132,
        CashWithDraw = 133,

        PaySalesTax = 135,
        OpeningAccount = 140,
        OpeningInventoryItem = 145,
        OpeningFixedAsset = 150,

        Loan = 160,
        Lend = 170,

        CustomerPayment = 182,
        VendorPayment = 183,


        JournalEntry = 190,

        SalesTax = 200,
        IncomeTax = 210,

        EarnestPayment = 220,
        EarnestReceive = 230
    }
}
