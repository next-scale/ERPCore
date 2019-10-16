using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Models.Projects
{
    public class NewTask
    {
        public Guid ProfileGuid { get; set; }
        public String Title { get; set; }
        public Guid ResponsibleId { get; set; }
        public Guid CreatorId { get; set; }
        public DateTime? CreatedDate { get;  set; }
    }
}