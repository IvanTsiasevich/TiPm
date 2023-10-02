using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Ti.Pm.Web.Shared
{
    public class DeleteErrorView : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }       
        public void Cancel()
        {
            MudDialog.Cancel();
        }
    }
}
