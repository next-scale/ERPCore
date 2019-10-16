using ERPCore.Enterprise.Models.Accounting.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPCore.Enterprise.Models.Transactions
{
    public static class TransactionHelper
    {
        public static string TrCode(TransactionTypes type)
        {
            switch (type)
            {
                case TransactionTypes.Sale:
                    return "SL";
                case TransactionTypes.SalesReturn:
                    return "SR";
                case TransactionTypes.SalesEstimate:
                    return "SE";
                case TransactionTypes.Purchase:
                    return "PU";
                case TransactionTypes.PurchaseReturn:
                    return "PR";
                case TransactionTypes.PurchaseEstimate:
                    return "PE";
                case TransactionTypes.EmployeePayment:
                    return "EMP";
                case TransactionTypes.FundTransfer:
                    return "FT";
                case TransactionTypes.SupplierPayment:
                    return "BP";
                case TransactionTypes.ReceivePayment:
                    return "RP";
                case TransactionTypes.CommercialPayment:
                    return "CP";
                case TransactionTypes.LiabilityPayment:
                    return "LP";
                case TransactionTypes.TaxPayment:
                    return "TP";
            }

            return string.Empty;
        }
    }
}
