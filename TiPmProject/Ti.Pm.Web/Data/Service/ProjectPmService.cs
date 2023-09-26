using Ti.Pm.PmDb.Model;
using Ti.Pm.PmDb;
using Ti.Pm.Web.Data.ViewModel;
using Ti.Pm.Web.Pages.Projects;

namespace Ti.Pm.Web.Data.Service
{
    public class ProjectPmService
    {
        EFRepository<ProjectPm> mRepoProject;

        public ProjectPmService(TiPmDbContext context)
        {
            mRepoProject = new EFRepository<ProjectPm>(context);
        }
        public async Task<List<ProjectPmVieweModel>> GetAll()
        {
            var listItems = mRepoProject.Get();
            var result = listItems.Select(x => Convert(x)).ToList();
            result.Reverse();
            return await Task.FromResult(result);
        }

        private ProjectPmVieweModel Convert(ProjectPm model)
        {
            var item = new ProjectPmVieweModel(model);
            return item;
        }

        public ProjectPmVieweModel Update(ProjectPmVieweModel item)
        {
            var model = mRepoProject.FindById(item.ProjectId);
            model.Title = item.Title;
            return Convert(mRepoProject.Update(model));
        }
        public ProjectPmVieweModel ReloadItem(ProjectPmVieweModel item)
        {
            var model = mRepoProject.Reload(item.ProjectId);
            if (model == null)
            {
                return null;
            }
            return Convert(model);
        }

        public void Delete(ProjectPmVieweModel item)
        {
            var model = mRepoProject.FindById(item.ProjectId);
            mRepoProject.Remove(model);
        }

        public ProjectPmVieweModel Create(ProjectPmVieweModel item)
        {           
            var newItem = mRepoProject.Create(item.Item);
            return Convert(newItem);
        }
  
        public List<ProjectPmVieweModel> FilteringText(string message)
        {
            var filteredListLogs = mRepoProject.GetQuery().Where(x => (x.Title.Contains(message))).ToList();
            var result = filteredListLogs.Select(Convert).ToList();
            result.Reverse();
            return result;
        }

    }
}
