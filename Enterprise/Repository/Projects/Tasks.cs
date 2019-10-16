

using ERPCore.Enterprise.Models.Projects;
using ERPCore.Enterprise.Models.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPCore.Enterprise.Repository.Projects
{

    public class Tasks : ERPNodeDalRepository
    {
        public Tasks(Organization organization) : base(organization)
        {

        }

        public List<Task> ListAll => erpNodeDBContext.Tasks.ToList();
        public IQueryable<Task> All => erpNodeDBContext.Tasks;
        public Task Find(Guid id) => erpNodeDBContext.Tasks.Find(id);

        public Task CreateNew(Task newTask)
        {
            newTask.Id = Guid.NewGuid();
            erpNodeDBContext.Tasks.Add(newTask);
            erpNodeDBContext.SaveChanges();
            return newTask;
        }

        public Task CreateNew(NewTask newTaskModel)
        {
            var newTask = new ERPCore.Enterprise.Models.Tasks.Task()
            {
                Id = Guid.NewGuid(),
                ResponsibleMemberId = newTaskModel.ResponsibleId,
                Title = newTaskModel.Title,
                CreatedDate = newTaskModel.CreatedDate ?? DateTime.Today
            };

            erpNodeDBContext.Tasks.Add(newTask);
            organization.SaveChanges();
            return newTask;
        }

        public void Remove(Task task)
        {
            erpNodeDBContext.Tasks.Remove(task);
            organization.SaveChanges();
        }

        public List<Task> ListByResponsible(Guid id) => erpNodeDBContext.Tasks.Where(p => p.ResponsibleMemberId == id).ToList();

    }
}