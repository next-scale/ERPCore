using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ERPCore.Enterprise.Models.ChartOfAccount;

namespace ERPCore.Enterprise.Models.Financial.Transfer
{
    public enum FundTransferStatus
    {
        Open = 0,
        Void = 2
    }
}