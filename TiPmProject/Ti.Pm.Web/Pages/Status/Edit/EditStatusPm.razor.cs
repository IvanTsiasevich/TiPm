using Microsoft.AspNetCore.Components;
using MudBlazor;
using Ti.Pm.Web.Data.ViewModel;


namespace Ti.Pm.Web.Pages.Status
{
    public class EditStatusPmViewe : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter]
        public StatusPmVieweModel StatusPmVieweModel { get; set; }
        public void Cancel()
        {
            MudDialog.Cancel();
        }
        public void Save()
        {
            MudDialog.Close(DialogResult.Ok(StatusPmVieweModel));
        }
    }
}
