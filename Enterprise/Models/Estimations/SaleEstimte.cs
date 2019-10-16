using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Taxes;
using ERPCore.Enterprise.Models.Transactions.Commercials;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using ERPCore.Enterprise.Models.Items;

namespace ERPCore.Enterprise.Models.Estimations
{
    public partial class SalesEstimate : Estimate
    {
        public string OmiseChargeId { get; set; }
    }
}
