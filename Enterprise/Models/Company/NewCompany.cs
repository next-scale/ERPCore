using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPCore.Enterprise.Models.Company
{
    public class NewCompanyModel
    {
        [Required]
        public String Name { get; set; }


        [Required]
        public String TaxID { get; set; }
       

        public DateTime FirstDate { get; set; }

        }
}
