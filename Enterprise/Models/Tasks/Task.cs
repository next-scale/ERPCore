using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ERPCore.Enterprise.Models.Security;
using ERPCore.Enterprise.Models.Tasks.Enums;

namespace ERPCore.Enterprise.Models.Tasks
{
    [Table("ERP_Tasks")]
    public class Task
    {

        [Key]
        public Guid Id { get; set; }
        public Enums.TaskStatus Status { get; set; }
        public String Title { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CloseDate { get; set; }


        public Guid? ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Projects.Project Project { get; set; }


        public int Age
        {
            get
            {
                if (CloseDate != null)
                    return (int)((DateTime)CloseDate - CreatedDate).TotalDays;
                else
                    return (int)(DateTime.Today - CreatedDate).TotalDays;
            }
        }

        public String Detail { get; set; }
        public void SetStatus(TaskStatus status) => this.Status = status;

        public Guid? ResponsibleMemberId { get; set; }
        [ForeignKey("ResponsibleMemberId")]
        public virtual Member ResponsibleMember { get; set; }



        public void ChangeStatus(TaskStatus status)
        {
            this.Status = status;
        }
    }
}
