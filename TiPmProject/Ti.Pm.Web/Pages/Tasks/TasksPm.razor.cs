using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.IO;
using Ti.Pm.Web.Data.Service;
using Ti.Pm.Web.Data.ViewModel;
using Ti.Pm.Web.Pages.Tasks.Edit;
using Ti.Pm.Web.Pages.TaskType.Edit;
using Ti.Pm.Web.Shared;

namespace Ti.Pm.Web.Pages.Tasks
{
    public class TasksPmViewe : ComponentBase
    {
        [Inject] protected IDialogService DialogService { get; set; }
        [Inject] private LogApplicationService applicationErrorService { get; set; }
        [Inject] private TaskPmService Service { get; set; }
        [Inject] public TaskTypePmService TaskTypePmService { get; set; }
        [Inject] public ProjectPmService ProjectPmService { get; set; }
        [Inject] public StatusPmService StatusPmService { get; set; }

        public List<TaskTypePmVieweModel> TaskTypePmVieweModels { get; set; }
        public List<ProjectPmVieweModel> ProjectPmVieweModels { get; set; }
        public List<StatusPmVieweModel> StatusPmVieweModels { get; set; }

        public List<TaskPmVieweModel> VieweModels { get; set; } = new List<TaskPmVieweModel>();

        public string filterText = "";


        protected override async Task OnInitializedAsync()
        {
            try
            {
                TaskTypePmVieweModels = await TaskTypePmService.GetAll();
                ProjectPmVieweModels = await ProjectPmService.GetAll();
                StatusPmVieweModels = await StatusPmService.GetAll();
                VieweModels = await Service.GetAll();
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
                filterText = value;
                FiltersText();
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

        protected async Task DeleteDialogAsync(TaskPmVieweModel item)
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
                var newItem = new TaskPmVieweModel();
                var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Medium };
                var parameters = new DialogParameters<EditTaskPm> { { x => x.TaskPmVieweModel, newItem } };
                var dialog = DialogService.Show<EditTaskPm>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    TaskPmVieweModel returnModel = new TaskPmVieweModel();
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

        public async Task EditItemDialog(TaskPmVieweModel item)
        {
            try
            {
                var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Medium };
                var parameters = new DialogParameters<EditTaskPm> { { x => x.TaskPmVieweModel, item } };
                var dialog = DialogService.Show<EditTaskPm>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    TaskPmVieweModel returnModel = new TaskPmVieweModel();
                    returnModel = (TaskPmVieweModel)result.Data;
                    var newItem = Service.Update(returnModel);
                    var index = VieweModels.FindIndex(x => x.TaskTypeId == newItem.TaskTypeId);
                    VieweModels[index] = newItem;
                    StateHasChanged();
                }
                else
                {
                    var oldItem = Service.ReloadItem(item);
                    var index = VieweModels.FindIndex(x => x.TaskTypeId == oldItem.TaskTypeId);
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
