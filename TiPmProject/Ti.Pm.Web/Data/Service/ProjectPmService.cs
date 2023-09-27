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
            var dbModels = mRepoProject.Get();
            var vieweModels = dbModels.Select(x => Convert(x)).ToList();
            vieweModels.Reverse();
            return await Task.FromResult(vieweModels);
        }

        private static ProjectPmVieweModel Convert(ProjectPm dbModel)
        {
            var vieweModel = new ProjectPmVieweModel(dbModel);
            return vieweModel;
        }

        public ProjectPmVieweModel Update(ProjectPmVieweModel updatedVieweModel)
        {
            var vieweModel = mRepoProject.FindById(updatedVieweModel.ProjectId);
            vieweModel.Title = updatedVieweModel.Title;
            return Convert(mRepoProject.Update(vieweModel));
        }
        public ProjectPmVieweModel ReloadItem(ProjectPmVieweModel updatedVieweModel)
        {
            var oldModel = mRepoProject.Reload(updatedVieweModel.ProjectId);
            if (oldModel == null)
            {
                return null;
            }
            return Convert(oldModel);
        }

        public void Delete(ProjectPmVieweModel vieweModel)
        {
            var dbModel = mRepoProject.FindById(vieweModel.ProjectId);
            mRepoProject.Remove(dbModel);
        }

        public ProjectPmVieweModel Create(ProjectPmVieweModel vieweModel)
        {           
            var dbModel = mRepoProject.Create(vieweModel.DbModel);
            return Convert(dbModel);
        }
  
        public List<ProjectPmVieweModel> FilteringByTitle(string title)
        {
            var filteredListLogs = mRepoProject.GetQuery().Where(x => x.Title.ToLower().Contains(title.ToLower())).ToList();
            var result = filteredListLogs.Select(x=>Convert(x)).ToList();
            result.Reverse();
            return result;
        }

    }
}
