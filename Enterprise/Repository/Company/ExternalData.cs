using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPCore.Enterprise.Repository.Company
{
    public class ExternalData
    {
        public String Name { get; set; }
        public String TaxId { get; set; }
        public int MemberCount { get; set; }
        public int OpenTaskCount { get; set; }
    }
}
