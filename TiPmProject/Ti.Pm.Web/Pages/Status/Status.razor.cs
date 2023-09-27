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
        public string mFilterTitle = "";

        [Inject] protected IDialogService DialogService { get; set; }
        [Inject] private LogApplicationService ApplicationErrorService { get; set; }
        [Inject] private StatusPmService StatusPmService { get; set; }
        [Inject] private TaskPmService TaskPmService { get; set; }

        public List<StatusPmVieweModel> StatusModels { get; set; } 
        public List<TaskPmVieweModel> TaskModels { get; set; } 


        protected override async Task OnInitializedAsync()
        {
            try
            {
                StatusModels = await StatusPmService.GetAll();
                TaskModels = await TaskPmService.GetAll();
                var UpdatedModels = new List<StatusPmVieweModel>();
                foreach (var model in StatusModels)
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
                StatusModels = UpdatedModels;
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

        public string FilterByTitle
        {
            get => mFilterTitle;

            set
            {
                {
                    mFilterTitle = value;
                    FiltersByTitle();
                }
            }
        }

        protected void FiltersByTitle()
        {
            try
            {
                StatusModels = StatusPmService.FilteringByTitle(mFilterTitle);
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
            FilterByTitle = "";
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
                        StatusPmService.Delete(item);
                        StatusModels.Remove(item);
                        StateHasChanged();
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException is SqlException || ex is SqlException)
                        {
                            options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Medium };
                            dialog = DialogService.Show<DeleteError>("", options);
                            result = await dialog.Result;
                            item.DeleteDisabled = true;
                            var index = StatusModels.FindIndex(x => x.StatusId == item.StatusId);
                            StatusModels[index] = item;
                            StateHasChanged();
                            return;
                        }
                        else
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
                }
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
                    var newUser = StatusPmService.Create(returnModel);
                    StatusModels.Add(newItem);
                    StateHasChanged();
                }
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
                    var newItem = StatusPmService.Update(returnModel);
                    var index = StatusModels.FindIndex(x => x.StatusId == newItem.StatusId);
                    newItem.DeleteDisabled = StatusModels[index].DeleteDisabled;
                    StatusModels[index] = newItem;
                    StateHasChanged();
                }
                else
                {
                    var oldItem = StatusPmService.ReloadItem(item);
                    var index = StatusModels.FindIndex(x => x.StatusId == oldItem.StatusId);
                    StatusModels[index] = oldItem;
                    StateHasChanged();
                }

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


    }
}
