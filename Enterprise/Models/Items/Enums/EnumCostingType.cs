using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPCore.Enterprise.Models.Items.Enums
{
    public enum CostingMethods
    {
        None = 0,
        FIFO = 1,
        WeightAverage = 2
    }

    public enum CostPosingMethods
    {
        None = 0,
        Perpetual = 1,
        EndOfFiscalYear = 2
    }
}
