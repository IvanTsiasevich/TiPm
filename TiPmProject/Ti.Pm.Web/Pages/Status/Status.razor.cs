using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using MudBlazor;
using Ti.Pm.Web.Data.Service;
using Ti.Pm.Web.Data.ViewModel;
using Ti.Pm.Web.Pages.Projects.Edit;
using Ti.Pm.Web.Pages.Status.Edit;
using Ti.Pm.Web.Pages.TaskType.Edit;
using Ti.Pm.Web.Shared;

namespace Ti.Pm.Web.Pages.Status
{
    public class StatusView : ComponentBase
    {
        [Inject] protected IDialogService DialogService { get; set; }
        [Inject] private LogApplicationService applicationErrorService { get; set; }
        [Inject] private StatusPmService Service { get; set; }
        [Inject] private TaskPmService TaskService { get; set; }

        public List<StatusPmVieweModel> VieweModels { get; set; } = new List<StatusPmVieweModel>();
        public List<TaskPmVieweModel> TaskModels { get; set; } = new List<TaskPmVieweModel>();

        public string filterText = "";

        protected override async Task OnInitializedAsync()
        {
            try
            {
                VieweModels = await Service.GetAll();
                TaskModels = await TaskService.GetAll();
                var UpdatedModels = new List<StatusPmVieweModel>();
                foreach (var model in VieweModels)
                {
                    if (TaskModels.Any(x => x.StatusId == model.StatusId))
                    {
                        model.DeleteDisabled = true;
                        UpdatedModels.Add(model);
                    }
                    else
                    {
                        UpdatedModels.Add(model);
                    }
                }
                VieweModels = UpdatedModels;
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

        public string FilterText
        {
            get => filterText;

            set
            {
                {
                    filterText = value;
                    FiltersText();
                }
            }
        }

        protected void FiltersText()
        {
            try
            {
                VieweModels = Service.FilteringText(filterText);
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

        protected async Task DeleteDialogAsync(StatusPmVieweModel item)
        {
            try
            {
                var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Medium };
                var dialog = DialogService.Show<DeleteModal>("", options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    try
                    {
                        Service.Delete(item);
                        VieweModels.Remove(item);
                        StateHasChanged();
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException is SqlException || ex is SqlException)
                        {
                            var options1 = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Medium };
                            var dialog1 = DialogService.Show<DeleteError>("", options);
                            var result1 = await dialog1.Result;
                            item.DeleteDisabled = true;
                            var index = VieweModels.FindIndex(x => x.StatusId == item.StatusId);
                            VieweModels[index] = item;
                            StateHasChanged();
                            return;
                        }
                        else
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
                }
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

        public async Task AddItemDialog()
        {
            try
            {
                var newItem = new StatusPmVieweModel();
                var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Medium };
                var parameters = new DialogParameters<EditStatusPm> { { x => x.StatusPmVieweModel, newItem } };
                var dialog = DialogService.Show<EditStatusPm>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    StatusPmVieweModel returnModel = new StatusPmVieweModel();
                    returnModel = newItem;
                    var newUser = Service.Create(returnModel);
                    VieweModels.Add(newItem);
                    StateHasChanged();
                }
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

        public async Task EditItemDialog(StatusPmVieweModel item)
        {
            try
            {
                var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Medium };
                var parameters = new DialogParameters<EditStatusPm> { { x => x.StatusPmVieweModel, item } };
                var dialog = DialogService.Show<EditStatusPm>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    StatusPmVieweModel returnModel = new StatusPmVieweModel();
                    returnModel = (StatusPmVieweModel)result.Data;
                    var newItem = Service.Update(returnModel);
                    var index = VieweModels.FindIndex(x => x.StatusId == newItem.StatusId);
                    newItem.DeleteDisabled = VieweModels[index].DeleteDisabled;
                    VieweModels[index] = newItem;
                    StateHasChanged();
                }
                else
                {
                    var oldItem = Service.ReloadItem(item);
                    var index = VieweModels.FindIndex(x => x.StatusId == oldItem.StatusId);
                    VieweModels[index] = oldItem;
                    StateHasChanged();
                }

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


    }
}
