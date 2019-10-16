using ERPCore.Enterprise.Models.ChartOfAccount;
using ERPCore.Enterprise.Models.Transactions;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ERPCore.Enterprise.Repository.Transactions
{
    public class Commercials : ERPNodeDalRepository
    {
        public Commercials(Organization organization) : base(organization)
        {

        }

        public IQueryable<Commercial> Query => erpNodeDBContext.Commercials;

        public List<Commercial> ListAll => erpNodeDBContext.Commercials
                    .Where(t => t.TransactionType == transactionType)
                    .ToList();

        public List<Commercial> ListOpen => erpNodeDBContext.Commercials
                    .Where(t => t.TransactionType == transactionType)
                    .Where(t => t.Status == CommercialStatus.Open)
                    .ToList();

        public IQueryable<Commercial> QueryOpen => erpNodeDBContext.Commercials
                    .Where(t => t.TransactionType == transactionType)
                    .Where(t => t.Status == CommercialStatus.Open);


        public IQueryable<Commercial> ActiveAndClose => erpNodeDBContext.Commercials
                    .Where(t => t.TransactionType == transactionType);

        public List<Commercial> LastMonth
        {
            get
            {
                var LastMonth = DateTime.Today.AddMonths(-1);
                var lastmonthCommercials = erpNodeDBContext.Commercials
                    .Where(t => t.TransactionType == transactionType)
                    .Where(t => t.TransactionDate.Year == LastMonth.Year && t.TransactionDate.Month == LastMonth.Month)
                    .Where(t => t.Status == CommercialStatus.Open || t.Status == CommercialStatus.Paid)
                    .OrderBy(tr => tr.TransactionDate)
                    .ToList();


                return lastmonthCommercials;
            }
        }

       
        public void CreateNew(Commercial commercial)
        {
            throw new NotImplementedException();
        }

        public List<Commercial> ThisMonth
        {
            get
            {
                var ThisMonth = DateTime.Today;
                var lastmonthCommercials = erpNodeDBContext.Commercials
                    .Where(t => t.TransactionType == transactionType)
                    .Where(t => t.TransactionDate.Year == ThisMonth.Year && t.TransactionDate.Month == ThisMonth.Month)
                    .Where(t => t.Status == CommercialStatus.Open)
                    .ToList();

                return lastmonthCommercials;
            }
        }

        public List<Commercial> LastYear
        {
            get
            {
                var LastYear = DateTime.Today.AddYears(-1);
                var lastmonthCommercials = erpNodeDBContext.Commercials
                    .Where(t => t.TransactionType == transactionType)
                    .Where(t => t.TransactionDate.Year == LastYear.Year)
                    .Where(t => t.Status == CommercialStatus.Open)
                    .ToList();

                return lastmonthCommercials;
            }
        }

        public List<Commercial> ThisYear
        {
            get
            {
                var ThisYear = DateTime.Today;
                var lastmonthCommercials = erpNodeDBContext.Commercials
                    .Where(t => t.TransactionType == transactionType)
                    .Where(t => t.TransactionDate.Year == ThisYear.Year)
                    .ToList();

                return lastmonthCommercials;
            }
        }

        public List<Commercial> OverDue
        {
            get
            {
                DateTime endDate = DateTime.Today.AddDays(-30);

                var overDueCommercials = ListOpen
                    .Where(t => t.TransactionDate <= endDate)
                    .ToList();

                return overDueCommercials;
            }
        }


        public void Remove(Models.Transactions.Commercial entity)
        {
            var _CommercialItems = entity.CommercialItems.ToList();

            foreach (var TransactionItem in _CommercialItems)
            {
                erpNodeDBContext.CommercialItems.Attach(TransactionItem);
                erpNodeDBContext.CommercialItems.Remove(TransactionItem);
            }
            erpNodeDBContext.Commercials.Attach(entity);
            erpNodeDBContext.Commercials.Remove(entity);
        }




        public Models.Transactions.Commercial Find(Guid id) => erpNodeDBContext.Commercials.Find(id);
        public Models.Transactions.Commercial Find(int transactionNo) => erpNodeDBContext.Commercials.Where(t => t.No == transactionNo).FirstOrDefault();

        public void ChangeStatus(Guid Id, CommercialStatus NewStatus)
        {
            var transaction = this.Find(Id);
            transaction.Status = NewStatus;
            this.SaveChanges();
        }


        public void ChangeAddress(Guid Id, Guid addreessGuid)
        {
            var _Transaction = this.Find(Id);
            _Transaction.ProfileAddressGuid = addreessGuid;
            this.SaveChanges();
        }


        public int NextNumber => (erpNodeDBContext.Commercials
                 .Where(Tr => Tr.TransactionType == this.transactionType)
                 .Max(e => (int?)e.No) ?? 0) + 1;



        public void UpdatePayments()
        {
            erpNodeDBContext.Commercials.ToList().ForEach(c => c.UpdatePayment());
            erpNodeDBContext.SaveChanges();
        }





    }
}