using Microsoft.AspNetCore.Mvc;
using Ti.Pm.PmDb;
using Ti.Pm.PmDb.Model;
using Ti.Pm.Web.Data.SharedServices;

namespace Ti.Pm.Web.Data.Account
{
    public class AccountController : Controller
    {

        public AccountController(TiPmDbContext context, SecurityService securityService)
        {
            dbContext = context;
            Service = securityService;
        }
        public TiPmDbContext dbContext { get; set; }
        public SecurityService Service { get; set; }
        [HttpPost]
        public async Task<IActionResult> Login(string name, string password)
        {
            if (!String.IsNullOrEmpty(name))
            {
                var user = GetUser(name, password);
                if (user != null)
                {
                    await Service.LoginAsync(user, Globals.UserRoleList.FirstOrDefault(x => x.RoleId == user.RoleId).RoleName);
                    return Json(new { message = "Correct login details", status = 1 });
                }
            }
            return Json(new { message = "Invalid login details", status = 0 });
            //return RedirectToPage("/Account/_Login");
        }
        public async Task<IActionResult> RedirectToHost(int t) => await Task.FromResult(LocalRedirect($"~/"));
        public async Task<IActionResult> Logout()
        {
            await Service.Logout();
            return RedirectToPage("/Account/_Login");
        }
        private User GetUser(string login, string password)
        {
            return dbContext.Login(login, password);
        }
    }
}