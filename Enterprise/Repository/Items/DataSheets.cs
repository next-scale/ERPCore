
using ERPCore.Enterprise.Models.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ERPCore.Enterprise.Repository.Items
{
    public class DataSheets : ERPNodeDalRepository
    {
        public DataSheets(Organization organization) : base(organization)
        {

        }

        public IQueryable<DataSheet> Query => erpNodeDBContext.DataSheets;
        public List<DataSheet> ListAll => erpNodeDBContext.DataSheets.ToList();
        public DataSheet Find(Guid id) => erpNodeDBContext.DataSheets.Find(id);

        public void Delete(Guid id)
        {
            var dataSheet = erpNodeDBContext.DataSheets.Find(id);

            erpNodeDBContext.DataSheets.Remove(dataSheet);
            organization.SaveChanges();
        }
        public DataSheet CreateNew(DataSheet dataSheet)
        {
            dataSheet.Id = Guid.NewGuid();
            erpNodeDBContext.DataSheets.Add(dataSheet);
            return dataSheet;
        }
        public DataSheet CreateNew(string name)
        {
            var newDataSheet = new DataSheet()
            {
                Id = Guid.NewGuid(),
                Name = name
            };

            erpNodeDBContext.DataSheets.Add(newDataSheet);
            erpNodeDBContext.SaveChanges();
            return newDataSheet;
        }
    }
}
