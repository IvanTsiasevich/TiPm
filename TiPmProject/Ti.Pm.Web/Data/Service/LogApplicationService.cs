using System.Reflection;
using Ti.Pm.PmDb;
using Ti.Pm.PmDb.Model;
using Ti.Pm.Web.Data.ViewModel;

namespace Ti.Pm.Web.Data.Service
{
    public class LogApplicationService
    {
        EFRepository<LogApplicationError> repoLog;
        private static TiPmDbContext DbContext;

        public LogApplicationService(TiPmDbContext context)
        { 
            repoLog = new EFRepository<LogApplicationError>(context);
            DbContext = context;
        }
        public async Task<List<ApplicationErrorViewModel>> GetAll()
        {
            var listItems = repoLog.Get();
            var result = listItems.Select(x => Convert(x)).ToList();
            result.Reverse();
            return await Task.FromResult(result);
        }

        private static ApplicationErrorViewModel Convert(LogApplicationError r)
        {
            var item = new ApplicationErrorViewModel(r);
            return item;
        }

        public ApplicationErrorViewModel ReloadItem(ApplicationErrorViewModel item)
        {
            var x = repoLog.Reload(item.LogApplicationErrorId);
            if (x == null)
            {
                return null;
            }
            return Convert(x);
        }

        public void Delete(ApplicationErrorViewModel item)
        {
            var x = repoLog.FindById(item.LogApplicationErrorId);
            repoLog.Remove(x);
        }

        public ApplicationErrorViewModel Create(string msg, string stackTrace, DateTime date, string? innerEx)
        {
            var item = new ApplicationErrorViewModel
			{
				InsertDate = date,
                ErrorMessage = msg,
                ErrorContext = stackTrace,
				ErrorInnerException = innerEx

			};
            Console.WriteLine($"\n{msg}   {stackTrace}  {date}");
            var newItem = repoLog.Create(item.Item);
            return Convert(newItem);
        }
        
       
        public List<ApplicationErrorViewModel> Filtering(DateTime? y)
        {
            var filteredListLogs = repoLog.GetQuery().Where(x => x.InsertDate.Date == y.GetValueOrDefault().Date).ToList();
            var result = filteredListLogs.Select(Convert).ToList();
            result.Reverse();
            return result;
        }
        public List<ApplicationErrorViewModel> FilteringError(string message)
        {
            var filteredListLogs = repoLog.GetQuery().Where(x => (x.ErrorMessage.Contains(message))).ToList();
            var result = filteredListLogs.Select(Convert).ToList();
            result.Reverse();
            return result;
        }
    }
}

