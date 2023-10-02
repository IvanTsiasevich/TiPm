using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using MudBlazor;
using System.Text.Json;
using Ti.Pm.PmDb.Model;
using Ti.Pm.Web.Data.Service;
using Ti.Pm.Web.Data.Services;
using Ti.Pm.Web.Data.ViewModel;
using Ti.Pm.Web.Pages.TaskType.Edit;
using Ti.Pm.Web.Shared;

namespace Ti.Pm.Web.Pages.TaskType
{
    public class TaskTypeView : ComponentBase
    {
        public string mFilterTitle = "";

        public List<TaskTypePmVieweModel> TaskTypeVieweModel { get; set; }
        public List<TaskPmVieweModel> TaskModels { get; set; }

        [Inject] protected IDialogService DialogService { get; set; }
        [Inject] protected CreateDialogOptionService DialogOptionService { get; set; }
        [Inject] private LogApplicationService ApplicationErrorService { get; set; }
        [Inject] private TaskTypePmService TaskTypePmService { get; set; }
        [Inject] private TaskPmService TaskPmService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                TaskTypeVieweModel = await TaskTypePmService.GetAll();
                TaskModels = await TaskPmService.GetAll();
                var UpdatedModels = new List<TaskTypePmVieweModel>();
                foreach (var model in TaskTypeVieweModel)
                {
                    bool asnser = TaskPmService.CheckConnection(model.TaskTypeId, "taskType");
                    {
                        if (asnser)
                        {
                            model.DeleteDisabled = true;
                            UpdatedModels.Add(model);
                        }
                        else
                        {
                            UpdatedModels.Add(model);
                        }
                    }
                }
                TaskTypeVieweModel = UpdatedModels;
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
                TaskTypeVieweModel = TaskTypePmService.FilteringByTitle(mFilterTitle);
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

        protected async Task DeleteDialogAsync(TaskTypePmVieweModel modelForDelete)
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
                        TaskTypePmService.Delete(modelForDelete);
                        TaskTypeVieweModel.Remove(modelForDelete);
                        StateHasChanged();
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException is SqlException || ex is SqlException)
                        {
                            options = DialogOptionService.CreateDialogOptions();
                            dialog = DialogService.Show<DeleteErrorModal>("", options);
                            modelForDelete.DeleteDisabled = true;
                            StateHasChanged();
                            return;
                        }
                        else
                        {
                            ApplicationErrorService.ErrorCathcer(ex);
                        }
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
                var newModel = new TaskTypePmVieweModel();
                var options = DialogOptionService.CreateDialogOptions();
                //передача параметра, стандартная конструкция МудБлазора
                var parameters = new DialogParameters<EditTaskTypePm> { { x => x.TaskTypePmVieweModel, newModel } };
                var dialog = DialogService.Show<EditTaskTypePm>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    TaskTypePmService.Create(newModel);
                    TaskTypeVieweModel.Add(newModel);
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                ApplicationErrorService.ErrorCathcer(ex);
            }
        }

        public async Task EditItemDialog(TaskTypePmVieweModel updateModel)
        {
            try
            {
                var options = DialogOptionService.CreateDialogOptions();
                //передача параметра, стандартная конструкция МудБлазора
                var parameters = new DialogParameters<EditTaskTypePm> { { x => x.TaskTypePmVieweModel, updateModel } };
                var dialog = DialogService.Show<EditTaskTypePm>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    TaskTypePmService.Update(updateModel);                    
                    StateHasChanged();
                }
                else
                {
                    var oldItem = TaskTypePmService.ReloadItem(updateModel);
                    oldItem.DeleteDisabled = updateModel.DeleteDisabled;
                    var index = TaskTypeVieweModel.FindIndex(x => x.TaskTypeId == oldItem.TaskTypeId);
                    TaskTypeVieweModel[index] = oldItem;
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                ApplicationErrorService.ErrorCathcer(ex);
            }
        }
        public async Task ShowChangeLogDialog(TaskTypePmVieweModel item)
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
