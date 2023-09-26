using Microsoft.AspNetCore.Components;
using MudBlazor;
using Ti.Pm.Web.Data.Service;
using Ti.Pm.Web.Data.ViewModel;


namespace Ti.Pm.Web.Pages.TaskType
{
    public class EditTaskTypePmViewe : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter]
        public TaskTypePmVieweModel TaskTypePmVieweModel { get; set; }
        public void Cancel()
        {
            MudDialog.Cancel();
        }
        public void Save()
        {
            MudDialog.Close(DialogResult.Ok(TaskTypePmVieweModel));
        }
    }
}
