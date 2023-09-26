using Microsoft.AspNetCore.Components;
using MudBlazor;
using Ti.Pm.Web.Data.Service;
using Ti.Pm.Web.Data.ViewModel;

namespace Ti.Pm.Web.Pages.System
{
    public class SystemPmView : ComponentBase
    {
        [Inject] private LogApplicationService applicationErrorService { get; set; }

        public List<ApplicationErrorViewModel> applicationErrorVieweModel = new List<ApplicationErrorViewModel>();
        public string filterText = "";
        public DateTime filterDate { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                FilterDate = DateTime.Now;
                applicationErrorVieweModel = await applicationErrorService.GetAll();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    applicationErrorService.Create(ex.Message, ex.StackTrace, DateTime.Now, ex.InnerException.StackTrace);
                }
                else
                {
                    string? message = null;
                    applicationErrorService.Create(ex.Message, ex.StackTrace, DateTime.Now, message);
                }
            }

        }
        public DateTime FilterDate
        {
            get => filterDate;

            set
            {
                filterDate = value;
                Filter();
            }
        }
        public string FilterText
        {
            get => filterText;

            set
            {
                filterText = value;
                FiltersText();
            }
        }
        protected void Filter()
        {
            try
            {
                applicationErrorVieweModel = applicationErrorService.Filtering(filterDate);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    applicationErrorService.Create(ex.Message, ex.StackTrace, DateTime.Now, ex.InnerException.StackTrace);
                }
                else
                {
                    string? message = null;
                    applicationErrorService.Create(ex.Message, ex.StackTrace, DateTime.Now, message);
                }
            }

        }
        protected void FiltersText()
        {
            try
            {
                applicationErrorVieweModel = applicationErrorService.FilteringError(filterText);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    applicationErrorService.Create(ex.Message, ex.StackTrace, DateTime.Now, ex.InnerException.StackTrace);
                }
                else
                {
                    string? message = null;
                    applicationErrorService.Create(ex.Message, ex.StackTrace, DateTime.Now, message);
                }
            }

        }
        protected void ClearSearch()
        {
            FilterText = "";
        }
    }
}
