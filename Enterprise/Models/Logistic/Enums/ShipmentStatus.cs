using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ERPCore.Enterprise.Models.Logistic.Enum
{
    public enum ShipmentStatus
    {
        Prepare = 0,
        Shiped = 1,
        Delivered = 2
    }

}
