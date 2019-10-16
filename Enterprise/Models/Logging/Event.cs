using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPCore.Enterprise.Models.Logging
{
    public enum EventLogLevel
    {
        Unspecific = 0,
        Error = 1,
        Warning = 2,
        Information = 3,
        SuccessAudit = 4,
        FailureAudit = 5
    }

    [Table("ERP_Event_Logs")]
    public class EventLog
    {
        [Key]
        public Guid Id { get; set; }
        public string Code { get; set; }
        [MaxLength(256)]
        public string Title { get; set; }
        public string Reference { get; set; }
        public string Detail { get; set; }

        public void Update(EventLog member)
        {
            throw new NotImplementedException();
        }

        public DateTime EventDateTime { get; set; }

        public EventLogLevel Level { get; set; }
    }
}
