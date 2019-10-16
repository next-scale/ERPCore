
using ERPCore.Enterprise.Models.Projects;
using ERPCore.Enterprise.Models.Projects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPCore.Enterprise.Repository.Projects
{

    public class Projects : ERPNodeDalRepository
    {
        public Projects(Organization organization) : base(organization)
        {

        }

        public IQueryable<Project> Query => erpNodeDBContext.Projects;
        public List<Project> ListAll => erpNodeDBContext.Projects.ToList();
        public Project Find(Guid id) => erpNodeDBContext.Projects.Find(id);

        public Project CreateNew(Project newProject)
        {
            newProject.Id = Guid.NewGuid();
            erpNodeDBContext.Projects.Add(newProject);
            erpNodeDBContext.SaveChanges();
            return newProject;
        }

        public IList<Project> GetListByStatus(ProjectStatus status) => this.Query.Where(t => t.Status == status).ToList();

        public Project CreateNew(NewProjectModel newProjectModel)
        {
            var newProject = new Project()
            {
                Id = Guid.NewGuid(),
                Name = newProjectModel.Name,
                CreatedDate = DateTime.Now
            };

            erpNodeDBContext.Projects.Add(newProject);
            erpNodeDBContext.SaveChanges();
            return newProject;
        }

        public List<Models.Transactions.Commercial> GetCommercials(Guid id)
        {
            var project = this.Find(id);
            return project.Commercials.ToList();
        }
    }
}