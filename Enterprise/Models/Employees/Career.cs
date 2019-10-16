using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPCore.Enterprise.Models.Employees
{
    [Table("ERP_Employees_Positions")]
    public class EmployeePosition
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(255)]
        public String Title { get; set; }

        public String Description { get; set; }


        public int Requried { get; set; }

        public virtual ICollection<Employee> AllEmployees { get; set; }

        public virtual int EmployeeCount => this.AllEmployees?.Count() ?? 0;

        public void Update(EmployeePosition position)
        {
            this.Title = position.Title;
            this.Description = position.Description;
        }
    }
}
