using Microsoft.AspNetCore.Components;
using MudBlazor;
using Ti.Pm.Web.Data.Service;
using Ti.Pm.Web.Data.ViewModel;

namespace Ti.Pm.Web.Pages.System
{
    public class SystemPmView : ComponentBase
    {
        public string mFilterErrorMsg = "";
        public DateTime mFilterDate = DateTime.Now;

        [Inject] private LogApplicationService ApplicationErrorService { get; set; }
        public List<ApplicationErrorViewModel> ApplicationErrorVieweModels { get; set; } 

        protected override async Task OnInitializedAsync()
        {
            try
            {
                ApplicationErrorVieweModels = await ApplicationErrorService.GetAll();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    ApplicationErrorService.Create(ex.Message, ex.StackTrace, DateTime.Now, ex.InnerException.StackTrace);
                }
                else
                {
                    string? message = null;
                    ApplicationErrorService.Create(ex.Message, ex.StackTrace, DateTime.Now, message);
                }
            }

        }
        public DateTime FilterByDate
        {
            get => mFilterDate;

            set
            {
                mFilterDate = value;
                FiltersByDate();
            }
        }
        public string FilterByErrorMsg
        {
            get => mFilterErrorMsg;

            set
            {
                mFilterErrorMsg = value;
                FiltersByErrorMsg();
            }
        }
        protected void FiltersByDate()
        {
            try
            {
                ApplicationErrorVieweModels = ApplicationErrorService.FilteringByDate(mFilterDate);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    ApplicationErrorService.Create(ex.Message, ex.StackTrace, DateTime.Now, ex.InnerException.StackTrace);
                }
                else
                {
                    string? message = null;
                    ApplicationErrorService.Create(ex.Message, ex.StackTrace, DateTime.Now, message);
                }
            }

        }
        protected void FiltersByErrorMsg()
        {
            try
            {
                ApplicationErrorVieweModels = ApplicationErrorService.FilteringByErrorMsg(mFilterErrorMsg);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    ApplicationErrorService.Create(ex.Message, ex.StackTrace, DateTime.Now, ex.InnerException.StackTrace);
                }
                else
                {
                    string? message = null;
                    ApplicationErrorService.Create(ex.Message, ex.StackTrace, DateTime.Now, message);
                }
            }

        }
        protected void ClearSearch()
        {
            FilterByErrorMsg = "";
            mFilterDate = DateTime.Now;
        }
    }
}
