using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPCore.Enterprise.Models.Profiles
{
    public class NewPassword
    {
        public Guid ProfileGuid { get; set; }
        public int pin { get; set; }
    }
}
