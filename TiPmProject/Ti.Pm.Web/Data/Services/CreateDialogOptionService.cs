using MudBlazor;

namespace Ti.Pm.Web.Data.Services
{
    public class CreateDialogOptionService
    {
        public DialogOptions CreateDialogOptions()
        {
            var options = new DialogOptions()
            {
                CloseButton = false,
                MaxWidth = MaxWidth.Medium
            };
            return options;
        }
    }
}
