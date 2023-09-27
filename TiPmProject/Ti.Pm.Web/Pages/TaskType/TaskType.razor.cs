using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using MudBlazor;
using Ti.Pm.Web.Data.Service;
using Ti.Pm.Web.Data.ViewModel;
using Ti.Pm.Web.Pages.Projects.Edit;
using Ti.Pm.Web.Pages.TaskType.Edit;
using Ti.Pm.Web.Shared;

namespace Ti.Pm.Web.Pages.TaskType
{
    public class TaskTypeView : ComponentBase
    {
        //Является полем которому мы присваем значение по умолчанию, и изменяем его введением фильтра при пролучении FilterByTitle,
        //и только после этого фильтруем функцией FiltersByTitle() которая поле и использует и сразу же вызывается после присвоения поля.
        public string mFilterTitle = "";
        [Inject] protected IDialogService DialogService { get; set; }
        [Inject] private LogApplicationService ApplicationErrorService { get; set; }
        [Inject] private TaskTypePmService TaskTypePmService { get; set; }
        [Inject] private TaskPmService TaskPmService { get; set; }

        public List<TaskTypePmVieweModel> TaskTypeVieweModel { get; set; } 
        public List<TaskPmVieweModel> TaskModels { get; set; } 
        protected override async Task OnInitializedAsync()
        {
            try
            {
                TaskTypeVieweModel = await TaskTypePmService.GetAll();
                TaskModels = await TaskPmService.GetAll();
                var UpdatedModels = new List<TaskTypePmVieweModel>();
                foreach (var model in TaskTypeVieweModel)
                {
                    if (TaskModels.Any(x => x.TaskTypeId == model.TaskTypeId))
                    {
                        model.DeleteDisabled = true;
                        UpdatedModels.Add(model);
                    }
                    else
                    {
                        UpdatedModels.Add(model);
                    }
                }
                TaskTypeVieweModel = UpdatedModels;
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
                TaskTypeVieweModel = TaskTypePmService.FilteringByTitle(mFilterTitle);
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

        protected async Task DeleteDialogAsync(TaskTypePmVieweModel item)
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
                        TaskTypePmService.Delete(item);
                        TaskTypeVieweModel.Remove(item);
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
                            var index = TaskTypeVieweModel.FindIndex(x => x.TaskTypeId == item.TaskTypeId);
                            TaskTypeVieweModel[index] = item;
                            StateHasChanged();
                            return;
                        }
                        else
                        {
                            ApplicationErrorService.Cathcer(ex);
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
                var newItem = new TaskTypePmVieweModel();
                var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Medium };
                var parameters = new DialogParameters<EditTaskTypePm> { { x => x.TaskTypePmVieweModel, newItem } };
                var dialog = DialogService.Show<EditTaskTypePm>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    TaskTypePmVieweModel returnModel = new TaskTypePmVieweModel();
                    returnModel = newItem;
                    var newUser = TaskTypePmService.Create(returnModel);
                    TaskTypeVieweModel.Add(newItem);
                    StateHasChanged();
                }

            }
            catch (Exception ex)
            {
                ApplicationErrorService.Cathcer(ex);
            }
        }

        public async Task EditItemDialog(TaskTypePmVieweModel item)
        {
            try
            {
                var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Medium };
                var parameters = new DialogParameters<EditTaskTypePm> { { x => x.TaskTypePmVieweModel, item } };
                var dialog = DialogService.Show<EditTaskTypePm>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    TaskTypePmVieweModel returnModel = new TaskTypePmVieweModel();
                    returnModel = (TaskTypePmVieweModel)result.Data;
                    var newItem = TaskTypePmService.Update(returnModel);
                    var index = TaskTypeVieweModel.FindIndex(x => x.TaskTypeId == newItem.TaskTypeId);
                    newItem.DeleteDisabled = TaskTypeVieweModel[index].DeleteDisabled;                    
                    TaskTypeVieweModel[index] = newItem;
                    StateHasChanged();
                }
                else
                {
                    var oldItem = TaskTypePmService.ReloadItem(item);
                    var index = TaskTypeVieweModel.FindIndex(x => x.TaskTypeId == oldItem.TaskTypeId);
                    TaskTypeVieweModel[index] = oldItem;
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
