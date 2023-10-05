using Ti.Pm.PmDb.Model;
using Ti.Pm.PmDb;
using Ti.Pm.Web.Data.ViewModel;

namespace Ti.Pm.Web.Data.Service
{
    public class StatusPmService
    {
        EFRepository<StatusPm> mRepoStatusPm;
        private readonly IHttpContextAccessor httpContextAccessor;

        public StatusPmService(TiPmDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            mRepoStatusPm = new EFRepository<StatusPm>(context);
            this.httpContextAccessor = httpContextAccessor;
            mRepoStatusPm.SetUserNameForLog(httpContextAccessor.HttpContext.User.Identity.Name);
        }
        public async Task<List<StatusPmVieweModel>> GetAll()
        {
            var dbModels = mRepoStatusPm.Get();
            var vieweModels = dbModels.Select(x => Convert(x)).ToList();
            vieweModels.Reverse();
            return await Task.FromResult(vieweModels);
        }

        private static StatusPmVieweModel Convert(StatusPm dbModel)
        {
            var vieweModel = new StatusPmVieweModel(dbModel);
            return vieweModel;
        }

        public StatusPmVieweModel Update(StatusPmVieweModel vieweModel)
        {          
            return Convert(mRepoStatusPm.Update(vieweModel.DbModel));
        }
        public StatusPmVieweModel ReloadItem(StatusPmVieweModel vieweModel)
        {
            var dbModel = mRepoStatusPm.Reload(vieweModel.StatusId);
            if (dbModel == null)
            {
                return null;
            }
            return Convert(dbModel);
        }

        public void Delete(StatusPmVieweModel vieweModel)
        {
            var dbModel = mRepoStatusPm.FindById(vieweModel.StatusId);
            mRepoStatusPm.Remove(dbModel);
        }

        public StatusPmVieweModel Create(StatusPmVieweModel vieweModel)
        {
            var newDbModel = mRepoStatusPm.Create(vieweModel.DbModel);
            return Convert(newDbModel);
        }

        public List<StatusPmVieweModel> FilteringByTitle(string message)
        {
            var filteredListStatuses = mRepoStatusPm.GetQuery().Where(x => x.Title.ToLower().Contains(message.ToLower())).ToList();
            var result = filteredListStatuses.Select(Convert).ToList();
            result.Reverse();
            return result;
        }
    }
}

