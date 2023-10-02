using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.Json;
using Ti.Pm.PmDb.Model;
using Ti.Pm.Web.Data.Service;
using Ti.Pm.Web.Data.Services;
using Ti.Pm.Web.Data.ViewModel;
using Ti.Pm.Web.Pages.Tasks.Edit;
using Ti.Pm.Web.Shared;

namespace Ti.Pm.Web.Pages.Tasks
{
    public class TasksPmViewe : ComponentBase
    {
        public string mFilterTitle = "";

        public List<TaskTypePmVieweModel> TaskTypePmVieweModels { get; set; }
        public List<ProjectPmVieweModel> ProjectPmVieweModels { get; set; }
        public List<StatusPmVieweModel> StatusPmVieweModels { get; set; }
        public List<TaskPmVieweModel> TaskPmVieweModels { get; set; }

        [Inject] protected IDialogService DialogService { get; set; }
        [Inject] protected CreateDialogOptionService DialogOptionService { get; set; }
        [Inject] private LogApplicationService ApplicationErrorService { get; set; }
        [Inject] private TaskPmService TaskPmService { get; set; }
        [Inject] public TaskTypePmService TaskTypePmService { get; set; }
        [Inject] public ProjectPmService ProjectPmService { get; set; }
        [Inject] public StatusPmService StatusPmService { get; set; }

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
                ApplicationErrorService.ErrorCathcer(ex);
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
                ApplicationErrorService.ErrorCathcer(ex);
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
                var options = DialogOptionService.CreateDialogOptions();
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
                        ApplicationErrorService.ErrorCathcer(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationErrorService.ErrorCathcer(ex);
            }

        }

        public async Task AddItemDialog()
        {
            try
            {
                var newModel = new TaskPmVieweModel();
                var options = DialogOptionService.CreateDialogOptions();
                //передача параметра, стандартная конструкция МудБлазора
                var parameters = new DialogParameters<EditTaskPm> { { x => x.TaskPmVieweModel, newModel } };
                var dialog = DialogService.Show<EditTaskPm>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    TaskPmService.Create(newModel);
                    TaskPmVieweModels.Add(newModel);
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                ApplicationErrorService.ErrorCathcer(ex);
            }
        }

        public async Task EditItemDialog(TaskPmVieweModel updateModel)
        {
            try
            {
                var options = DialogOptionService.CreateDialogOptions();
                //передача параметра, стандартная конструкция МудБлазора
                var parameters = new DialogParameters<EditTaskPm> { { x => x.TaskPmVieweModel, updateModel } };
                var dialog = DialogService.Show<EditTaskPm>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    var newItem = TaskPmService.Update(updateModel);                   
                    StateHasChanged();
                }
                else
                {
                    var oldItem = TaskPmService.ReloadItem(updateModel);
                    var index = TaskPmVieweModels.FindIndex(x => x.TaskTypeId == oldItem.TaskTypeId);
                    TaskPmVieweModels[index] = oldItem;
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                ApplicationErrorService.ErrorCathcer(ex);
            }
        }
        public async Task ShowChangeLogDialog(TaskPmVieweModel item)
        {
            try
            {
                var changeLogJson = string.IsNullOrEmpty(item.ChangeLogJson) ? new List<ChangeLog>() : JsonSerializer.Deserialize<List<ChangeLog>>(item.ChangeLogJson);
                var options = DialogOptionService.CreateDialogOptions();
                var parameters = new DialogParameters<ChangeLogModal> { { x => x.ChangeLog, changeLogJson } };
                DialogService.Show<ChangeLogModal>("", parameters, options);
            }
            catch (Exception ex)
            {
                ApplicationErrorService.ErrorCathcer(ex);
            }
        }
    }
}
