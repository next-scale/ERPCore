using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPCore.Enterprise.Repository.Transactions
{
    public class QueryRange
    {
        private DateTime _startDate;
        private DateTime _enDate;


        public QueryRange(DateTime startDate, DateTime endDate)
        {
            _startDate = startDate;
            _enDate = endDate;
        }
    }
}
