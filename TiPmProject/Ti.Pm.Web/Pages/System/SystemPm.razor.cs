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

        public List<ApplicationErrorViewModel> ApplicationErrorVieweModels { get; set; }

        [Inject] private LogApplicationService ApplicationErrorService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                ApplicationErrorVieweModels = await ApplicationErrorService.GetAll();
            }
            catch (Exception ex)
            {
                ApplicationErrorService.ErrorCathcer(ex);
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
                ApplicationErrorService.ErrorCathcer(ex);
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
                ApplicationErrorService.ErrorCathcer(ex);
            }

        }
        protected void ClearSearch()
        {
            FilterByErrorMsg = "";
            mFilterDate = DateTime.Now;
        }
    }
}
