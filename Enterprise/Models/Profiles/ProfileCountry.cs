using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPCore.Enterprise.Models.Profiles
{
    [Table("ERP_Profiles_ProfileCountries")]
    public class ProfileCountry
    {
        [Key]
        public Guid Id { get; set; }
        public String Name_Th { get; set; }
        public String Name_En { get; set; }
        bool IsDefault { get; set; }
    }
}
