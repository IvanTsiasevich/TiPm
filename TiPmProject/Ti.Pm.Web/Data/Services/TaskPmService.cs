using Ti.Pm.PmDb.Model;
using Ti.Pm.PmDb;
using Ti.Pm.Web.Data.ViewModel;
using Microsoft.IdentityModel.Tokens;

namespace Ti.Pm.Web.Data.Service
{
    public class TaskPmService
    {
        EFRepository<TaskPm> mRepoTask;

        public TaskPmService(TiPmDbContext context)
        {
            mRepoTask = new EFRepository<TaskPm>(context);
        }
        public async Task<List<TaskPmVieweModel>> GetAll()
        {
            var dbModels = mRepoTask.Get();
            var vieweModels = dbModels.Select(x => Convert(x)).ToList();
            vieweModels.Reverse();
            return await Task.FromResult(vieweModels);
        }

        private static TaskPmVieweModel Convert(TaskPm dbModel)
        {
            var vieweModel = new TaskPmVieweModel(dbModel);
            return vieweModel;
        }

        public TaskPmVieweModel Update(TaskPmVieweModel vieweModel)
        {            
            return Convert(mRepoTask.Update(vieweModel.DbModel));
        }
        public TaskPmVieweModel ReloadItem(TaskPmVieweModel vieweModel)
        {
            var dbModel = mRepoTask.Reload(vieweModel.TaskId);
            if (dbModel == null)
            {
                return null;
            }
            return Convert(dbModel);
        }

        public void Delete(TaskPmVieweModel vieweModel)
        {
            var dbModel = mRepoTask.FindById(vieweModel.TaskId);
            mRepoTask.Remove(dbModel);
        }

        public TaskPmVieweModel Create(TaskPmVieweModel vieweModel)
        {
            var newDbModel = mRepoTask.Create(vieweModel.DbModel);
            return Convert(newDbModel);
        }
        public TaskPmVieweModel FindById(TaskPmVieweModel item)
        {
            return Convert(mRepoTask.FindById(item.TaskId));
        }
        public List<TaskPmVieweModel> FilteringByTitle(string message)
        {
            var filteredList = mRepoTask.GetQuery().Where(x => x.Title.ToLower().Contains(message.ToLower())).ToList();
            var result = filteredList.Select(x=>Convert(x)).ToList();
            result.Reverse();
            return result;
        }
       
        public List<TaskPmVieweModel> FilteringByProject(int projectId)
        {
            var filteredList = mRepoTask.GetQuery().Where(x => (x.ProjectId == projectId)).ToList();
            var result = filteredList.Select(Convert).ToList();
            result.Reverse();
            return result;
        }

        public bool CheckConnection(int foregenKey, string typeOfCheck)
        {
            switch (typeOfCheck)
            {
                case "project":
                    if (mRepoTask.GetQuery().Where(x => (x.ProjectId == foregenKey)).ToList().IsNullOrEmpty())
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                case "status":
                    if (mRepoTask.GetQuery().Where(x => (x.StatusId == foregenKey)).ToList().IsNullOrEmpty())
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                case "taskType":
                    if (mRepoTask.GetQuery().Where(x => (x.TaskTypeId == foregenKey)).ToList().IsNullOrEmpty())
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
            }           
            return true;
        }
    }
}
