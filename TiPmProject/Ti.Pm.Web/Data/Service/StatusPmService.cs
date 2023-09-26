using Ti.Pm.PmDb.Model;
using Ti.Pm.PmDb;
using Ti.Pm.Web.Data.ViewModel;

namespace Ti.Pm.Web.Data.Service
{
    public class StatusPmService
    {
        EFRepository<StatusPm> repoStatusPm;
        private static TiPmDbContext DbContext;

        public StatusPmService(TiPmDbContext context)
        {
            repoStatusPm = new EFRepository<StatusPm>(context);
            DbContext = context;
        }
        public async Task<List<StatusPmVieweModel>> GetAll()
        {
            var listItems = repoStatusPm.Get();
            var result = listItems.Select(x => Convert(x)).ToList();
            result.Reverse();
            return await Task.FromResult(result);
        }

        private static StatusPmVieweModel Convert(StatusPm r)
        {
            var item = new StatusPmVieweModel(r);
            return item;
        }

        public StatusPmVieweModel Update(StatusPmVieweModel item)
        {
            var x = repoStatusPm.FindById(item.StatusId);
            x.Title = item.Title;
            x.OrderId = item.OrderId;
            return Convert(repoStatusPm.Update(x));
        }
        public StatusPmVieweModel ReloadItem(StatusPmVieweModel item)
        {
            var x = repoStatusPm.Reload(item.StatusId);
            if (x == null)
            {
                return null;
            }
            return Convert(x);
        }

        public void Delete(StatusPmVieweModel item)
        {
            var x = repoStatusPm.FindById(item.StatusId);
            repoStatusPm.Remove(x);
        }

        public StatusPmVieweModel Create(StatusPmVieweModel item)
        {
            var newItem = repoStatusPm.Create(item.Item);
            return Convert(newItem);
        }

        public List<StatusPmVieweModel> FilteringText(string message)
        {
            var filteredListLogs = repoStatusPm.GetQuery().Where(x => (x.Title.Contains(message))).ToList();
            var result = filteredListLogs.Select(Convert).ToList();
            result.Reverse();
            return result;
        }
    }
}

