using ERPCore.Enterprise.Models.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using ERPCore.Enterprise.Models.Taxes;
using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Accounting.Enums;
using ERPCore.Enterprise.Models.Logging;

namespace ERPCore.Enterprise.Repository.Logging
{
    public class EventLogs : ERPNodeDalRepository
    {

        public EventLogs(Organization organization) : base(organization)
        {

        }


        public List<EventLog> ListAll => erpNodeDBContext.EventLogs.ToList();
        public IQueryable<EventLog> All => erpNodeDBContext.EventLogs;
        public EventLog Find(Guid LogId) => erpNodeDBContext.EventLogs.Find(LogId);

        public int GetAmount(EventLogLevel level)
        {
            return erpNodeDBContext.EventLogs.Where(ev => ev.Level == level).Count();
        }

        public void NewEventLog(EventLogLevel level, string code, string title, string reference, string detail, DateTime? eventDateTime = null)
        {
            eventDateTime = eventDateTime ?? DateTime.Now;
            Console.WriteLine(title);

            var newEventLog = new EventLog()
            {
                Id = Guid.NewGuid(),
                Code = code.PadLeft(5, '0'),
                Title = title,
                Reference = reference,
                Detail = detail,
                Level = level,
                EventDateTime = eventDateTime ?? DateTime.Now
            };

            this.erpNodeDBContext.EventLogs.Add(newEventLog);
            this.erpNodeDBContext.SaveChanges();
        }

        public void Remove(EventLog eventLog)
        {
            erpNodeDBContext.EventLogs.Remove(eventLog);
            organization.SaveChanges();
        }
    }
}