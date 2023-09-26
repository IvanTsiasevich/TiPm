using Ti.Pm.PmDb.Model;
using Ti.Pm.PmDb;
using Ti.Pm.Web.Data.ViewModel;

namespace Ti.Pm.Web.Data.Service
{
    public class TaskPmService
    {
        EFRepository<TaskPm> repoTask;
        private static TiPmDbContext DbContext;

        public TaskPmService(TiPmDbContext context)
        {
            repoTask = new EFRepository<TaskPm>(context);
            DbContext = context;
        }
        public async Task<List<TaskPmVieweModel>> GetAll()
        {
            var listItems = repoTask.Get();
            var result = listItems.Select(x => Convert(x)).ToList();
            result.Reverse();
            return await Task.FromResult(result);
        }

        private static TaskPmVieweModel Convert(TaskPm r)
        {
            var item = new TaskPmVieweModel(r);
            return item;
        }

        public TaskPmVieweModel Update(TaskPmVieweModel item)
        {
            var x = repoTask.FindById(item.TaskId);
            x.StatusId = item.StatusId;
            x.ProjectId = item.ProjectId;
            x.TaskTypeId = item.TaskTypeId;
            x.Title = item.Title;
            x.Description = item.Description;
            return Convert(repoTask.Update(x));
        }
        public TaskPmVieweModel ReloadItem(TaskPmVieweModel item)
        {
            var x = repoTask.Reload(item.TaskId);
            if (x == null)
            {
                return null;
            }
            return Convert(x);
        }

        public void Delete(TaskPmVieweModel item)
        {
            var x = repoTask.FindById(item.TaskId);
            repoTask.Remove(x);
        }

        public TaskPmVieweModel Create(TaskPmVieweModel item)
        {
            var newItem = repoTask.Create(item.Item);
            return Convert(newItem);
        }

        public List<TaskPmVieweModel> FilteringText(string message)
        {
            var filteredList = repoTask.GetQuery().Where(x => (x.Title.Contains(message))).ToList();
            var result = filteredList.Select(Convert).ToList();
            result.Reverse();
            return result;
        }
       
        public List<TaskPmVieweModel> FilteringByProject(int ProgectId)
        {
            var filteredList = repoTask.GetQuery().Where(x => (x.ProjectId == ProgectId)).ToList();
            var result = filteredList.Select(Convert).ToList();
            result.Reverse();
            return result;
        }
    }
}
