using Ti.Pm.PmDb.Model;
using Ti.Pm.PmDb;
using Ti.Pm.Web.Data.ViewModel;

namespace Ti.Pm.Web.Data.Service
{
    public class TaskTypePmService
    {
        EFRepository<TaskTypePm> mRepoTaskType;

        public TaskTypePmService(TiPmDbContext context)
        {
            mRepoTaskType = new EFRepository<TaskTypePm>(context);
        }
        public async Task<List<TaskTypePmVieweModel>> GetAll()
        {
            var dbModels = mRepoTaskType.Get();
            var vieweModels = dbModels.Select(x => Convert(x)).ToList();
            vieweModels.Reverse();
            return await Task.FromResult(vieweModels);
        }

        private static TaskTypePmVieweModel Convert(TaskTypePm dbModel)
        {
            var vieweModel = new TaskTypePmVieweModel(dbModel);
            return vieweModel;
        }

        public TaskTypePmVieweModel Update(TaskTypePmVieweModel vieweModel)
        {
            var dbModel = mRepoTaskType.FindById(vieweModel.TaskTypeId);
            dbModel.Title = vieweModel.Title;
            return Convert(mRepoTaskType.Update(dbModel));
        }
        public TaskTypePmVieweModel ReloadItem(TaskTypePmVieweModel vieweModel)
        {
            var dbModel = mRepoTaskType.Reload(vieweModel.TaskTypeId);
            if (dbModel == null)
            {
                return null;
            }
            return Convert(dbModel);
        }

        public void Delete(TaskTypePmVieweModel vieweModel)
        {
            var dbModel = mRepoTaskType.FindById(vieweModel.TaskTypeId);
            mRepoTaskType.Remove(dbModel);
        }

        public TaskTypePmVieweModel Create(TaskTypePmVieweModel vieweModel)
        {
            var newDbModel = mRepoTaskType.Create(vieweModel.DbModel);
            return Convert(newDbModel);
        }

        public List<TaskTypePmVieweModel> FilteringByTitle(string message)
        {
            var filteredList = mRepoTaskType.GetQuery().Where(x => x.Title.ToLower().Contains(message.ToLower())).ToList();
            var result = filteredList.Select(Convert).ToList();
            result.Reverse();
            return result;
        }
    }
}
