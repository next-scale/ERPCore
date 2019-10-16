using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Models.Notes
{
    [Table("ERP_Notes")]
    public class Note
    {
        [Key]
        public Guid Id { get; set; }

        public String Content { get; set; }

        public DateTime CreatedDate { get; set; }


        public Guid CreateByProfileGuid { get; set; }
        [ForeignKey("CreateByProfileGuid")]
        public Profiles.Profile CreateByProfile { get; set; }



        public Guid AssignToProfileGuid { get; set; }
        [ForeignKey("AssignToProfileGuid")]
        public Profiles.Profile AssignToProfile { get; set; }

    }




}
