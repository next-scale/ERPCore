using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace ERPCore.Enterprise.Models.Organizations
{

    public class NewOrganizationModel
    {
        public String Name { get; set; }

        public String TaxId { get; set; }
    }

}
