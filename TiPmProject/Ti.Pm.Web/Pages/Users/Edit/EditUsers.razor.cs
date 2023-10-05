using Microsoft.AspNetCore.Components;
using MudBlazor;
using Ti.Pm.Web.Data.ViewModel;
using Ti.Pm.Web.Data.ViewModels;

namespace Ti.Pm.Web.Pages.Users.Edit
{
    public class EditUsersViewe : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter]
        public UserVieweModel UserVieweModel { get; set; }
        public void Cancel()
        {
            MudDialog.Cancel();
        }
        public void Save()
        {
            MudDialog.Close(DialogResult.Ok(UserVieweModel));
        }
    }
}
