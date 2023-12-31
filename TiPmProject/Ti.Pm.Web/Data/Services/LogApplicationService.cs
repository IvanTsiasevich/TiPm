﻿using Ti.Pm.PmDb;
using Ti.Pm.PmDb.Model;
using Ti.Pm.Web.Data.ViewModel;

namespace Ti.Pm.Web.Data.Service
{
    public class LogApplicationService
    {
        EFRepository<ApplicationError> mRepoLog;
        private readonly IHttpContextAccessor httpContextAccessor;

        public LogApplicationService(TiPmDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            mRepoLog = new EFRepository<ApplicationError>(context);
            this.httpContextAccessor = httpContextAccessor;
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

        public ApplicationErrorViewModel Create(ApplicationErrorViewModel viewModel)
        {
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
            var result = filteredListLogs.Select(x => Convert(x)).ToList();
            result.Reverse();
            return result;
        }

        public void ErrorCathcer(Exception ex)
        {
            {
                var viewModel = new ApplicationErrorViewModel
                {
                    InsertDate = DateTime.Now,
                    ErrorMessage = ex.Message,
                    ErrorContext = ex.StackTrace,
                    UserName = httpContextAccessor.HttpContext.User.Identity.Name
                };
                if (ex.InnerException != null)
                {
                    viewModel.ErrorInnerException = ex.InnerException.StackTrace;
                };
                Create(viewModel);
            }

        }

    }
}

