using Ti.Pm.PmDb.Model;
using Ti.Pm.PmDb;
using Ti.Pm.Web.Data.ViewModel;

namespace Ti.Pm.Web.Data.Service
{
    public class TaskTypePmService
    {
        EFRepository<TaskTypePm> repoTaskType;
        private static TiPmDbContext DbContext;

        public TaskTypePmService(TiPmDbContext context)
        {
            repoTaskType = new EFRepository<TaskTypePm>(context);
            DbContext = context;
        }
        public async Task<List<TaskTypePmVieweModel>> GetAll()
        {
            var listItems = repoTaskType.Get();
            var result = listItems.Select(x => Convert(x)).ToList();
            result.Reverse();
            return await Task.FromResult(result);
        }

        private static TaskTypePmVieweModel Convert(TaskTypePm r)
        {
            var item = new TaskTypePmVieweModel(r);
            return item;
        }

        public TaskTypePmVieweModel Update(TaskTypePmVieweModel item)
        {
            var x = repoTaskType.FindById(item.TaskTypeId);
            x.Title = item.Title;
            return Convert(repoTaskType.Update(x));
        }
        public TaskTypePmVieweModel ReloadItem(TaskTypePmVieweModel item)
        {
            var x = repoTaskType.Reload(item.TaskTypeId);
            if (x == null)
            {
                return null;
            }
            return Convert(x);
        }

        public void Delete(TaskTypePmVieweModel item)
        {
            var x = repoTaskType.FindById(item.TaskTypeId);
            repoTaskType.Remove(x);
        }

        public TaskTypePmVieweModel Create(TaskTypePmVieweModel item)
        {
            var newItem = repoTaskType.Create(item.Item);
            return Convert(newItem);
        }

        public List<TaskTypePmVieweModel> FilteringText(string message)
        {
            var filteredListLogs = repoTaskType.GetQuery().Where(x => (x.Title.Contains(message))).ToList();
            var result = filteredListLogs.Select(Convert).ToList();
            result.Reverse();
            return result;
        }
    }
}
