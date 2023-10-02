using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Ti.Pm.Web.Shared
{
    public class DeleteModalView : ComponentBase
    {
        public bool mAnswer = true;

        [CascadingParameter] MudDialogInstance MudDialog { get; set; }                
        public void Delete()
        {
            MudDialog.Close(DialogResult.Ok(mAnswer));
        }
        public void Cancel()
        {
            MudDialog.Cancel();
        }
    }
}
