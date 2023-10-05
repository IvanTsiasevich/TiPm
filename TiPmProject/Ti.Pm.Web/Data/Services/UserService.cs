using Ti.Pm.PmDb.Model;
using Ti.Pm.PmDb;
using Ti.Pm.Web.Data.ViewModel;
using Ti.Pm.Web.Data.ViewModels;

namespace Ti.Pm.Web.Data.Services
{
    public class UserService
    {
        EFRepository<User> mUsers;
        IHttpContextAccessor httpContextAccessor;
        public UserService(TiPmDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            mUsers = new EFRepository<User>(context);
            this.httpContextAccessor = httpContextAccessor;
            mUsers.SetUserNameForLog(httpContextAccessor.HttpContext.User.Identity.Name);
        }
        public async Task<List<UserVieweModel>> GetAll()
        {
            var dbModels = mUsers.Get();
            var vieweModels = dbModels.Select(x => Convert(x)).ToList();
            vieweModels.Reverse();
            return await Task.FromResult(vieweModels);
        }

        private static UserVieweModel Convert(User dbModel)
        {
            var vieweModel = new UserVieweModel(dbModel);
            return vieweModel;
        }

        public UserVieweModel Update(UserVieweModel vieweModel)
        {
            return Convert(mUsers.Update(vieweModel.DbModel));
        }
        public UserVieweModel ReloadItem(UserVieweModel vieweModel)
        {
            var dbModel = mUsers.Reload(vieweModel.UserId);
            if (dbModel == null)
            {
                return null;
            }
            return Convert(dbModel);
        }

        public void Delete(UserVieweModel vieweModel)
        {
            var dbModel = mUsers.FindById(vieweModel.UserId);
            mUsers.Remove(dbModel);
        }

        public UserVieweModel Create(UserVieweModel vieweModel)
        {
            var newDbModel = mUsers.Create(vieweModel.DbModel);
            return Convert(newDbModel);
        }       
    }
}
