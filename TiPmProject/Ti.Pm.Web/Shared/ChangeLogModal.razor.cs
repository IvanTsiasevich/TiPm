using Microsoft.AspNetCore.Components;
using MudBlazor;
using Ti.Pm.PmDb.Model;
using Ti.Pm.Web.Data.ViewModel;

namespace Ti.Pm.Web.Shared
{
    public class ChangeLogModalViewe : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter]
        public List<ChangeLog> ChangeLog { get; set; }
        public void Cancel()
        {
            MudDialog.Cancel();
        }        
    }

}
