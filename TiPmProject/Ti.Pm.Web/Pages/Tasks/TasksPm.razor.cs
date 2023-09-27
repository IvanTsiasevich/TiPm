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
        public string mFilterTitle = "";

        [Inject] protected IDialogService DialogService { get; set; }
        [Inject] private LogApplicationService ApplicationErrorService { get; set; }
        [Inject] private TaskPmService TaskPmService { get; set; }
        [Inject] public TaskTypePmService TaskTypePmService { get; set; }
        [Inject] public ProjectPmService ProjectPmService { get; set; }
        [Inject] public StatusPmService StatusPmService { get; set; }

        public List<TaskTypePmVieweModel> TaskTypePmVieweModels { get; set; }
        public List<ProjectPmVieweModel> ProjectPmVieweModels { get; set; }
        public List<StatusPmVieweModel> StatusPmVieweModels { get; set; }
        public List<TaskPmVieweModel> TaskPmVieweModels { get; set; } 


        protected override async Task OnInitializedAsync()
        {
            try
            {
                TaskTypePmVieweModels = await TaskTypePmService.GetAll();
                ProjectPmVieweModels = await ProjectPmService.GetAll();
                StatusPmVieweModels = await StatusPmService.GetAll();
                TaskPmVieweModels = await TaskPmService.GetAll();
            }
            catch (Exception ex)
            {
                ApplicationErrorService.Cathcer(ex);
            }

        }

        public string FilterByTitle
        {
            get => mFilterTitle;

            set
            {
                mFilterTitle = value;
                FiltersByTitle();
            }
        }

        protected void FiltersByTitle()
        {
            try
            {
                TaskPmVieweModels = TaskPmService.FilteringByTitle(mFilterTitle);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                ApplicationErrorService.Cathcer(ex);
            }

        }
        protected void ClearSearch()
        {
            FilterByTitle = "";
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
                        TaskPmService.Delete(item);
                        TaskPmVieweModels.Remove(item);
                        StateHasChanged();
                    }
                    catch (Exception ex)
                    {
                        ApplicationErrorService.Cathcer(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationErrorService.Cathcer(ex);
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
                    var newUser = TaskPmService.Create(returnModel);
                    TaskPmVieweModels.Add(newItem);
                    StateHasChanged();
                }

            }
            catch (Exception ex)
            {
                ApplicationErrorService.Cathcer(ex);
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
                    var newItem = TaskPmService.Update(returnModel);
                    var index = TaskPmVieweModels.FindIndex(x => x.TaskTypeId == newItem.TaskTypeId);
                    TaskPmVieweModels[index] = newItem;
                    StateHasChanged();
                }
                else
                {
                    var oldItem = TaskPmService.ReloadItem(item);
                    var index = TaskPmVieweModels.FindIndex(x => x.TaskTypeId == oldItem.TaskTypeId);
                    TaskPmVieweModels[index] = oldItem;
                    StateHasChanged();
                }

            }
            catch (Exception ex)
            {
                ApplicationErrorService.Cathcer(ex);
            }
        }
    }
}
