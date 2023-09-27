using System.Reflection;
using Ti.Pm.PmDb;
using Ti.Pm.PmDb.Model;
using Ti.Pm.Web.Data.ViewModel;

namespace Ti.Pm.Web.Data.Service
{
    public class LogApplicationService
    {
        EFRepository<ApplicationError> mRepoLog;

        public LogApplicationService(TiPmDbContext context)
        { 
            mRepoLog = new EFRepository<ApplicationError>(context);
        }

        public async Task<List<ApplicationErrorViewModel>> GetAll()
        {
            var listItems = mRepoLog.Get();
            var result = listItems.Select(x => Convert(x)).ToList();//Более читаемо касательно вопроса почему конверт работает без передачи
            result.Reverse();
            return await Task.FromResult(result);
        }

        private static ApplicationErrorViewModel Convert(ApplicationError dbModel)
        {
            var model = new ApplicationErrorViewModel(dbModel);
            return model;
        }

        public ApplicationErrorViewModel Create(string msg, string stackTrace, DateTime date, string? innerEx)
        {
            var viewModel = new ApplicationErrorViewModel
			{
				InsertDate = date,
                ErrorMessage = msg,
                ErrorContext = stackTrace,
				ErrorInnerException = innerEx
			};
            var dbModel = mRepoLog.Create(viewModel.DbModel);
            return Convert(dbModel);
        }
              
        public List<ApplicationErrorViewModel> FilteringByDate(DateTime? dateFilter)
        {
            var filteredListLogs = mRepoLog.GetQuery().Where(x => x.InsertDate.Date == dateFilter.GetValueOrDefault().Date).ToList();
            var result = filteredListLogs.Select(Convert).ToList();
            result.Reverse();
            return result;
        }
        public List<ApplicationErrorViewModel> FilteringByErrorMsg(string message)
        {
            var filteredListLogs = mRepoLog.GetQuery().Where(x => x.ErrorMessage.ToLower().Contains(message.ToLower())).ToList();
            var result = filteredListLogs.Select(x=>Convert(x)).ToList();
            result.Reverse();
            return result;
        }
    }
}

