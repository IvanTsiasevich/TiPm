using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using MudBlazor;
using System.Text.Json;
using Ti.Pm.PmDb.Model;
using Ti.Pm.Web.Data.Service;
using Ti.Pm.Web.Data.Services;
using Ti.Pm.Web.Data.ViewModel;
using Ti.Pm.Web.Pages.Status.Edit;
using Ti.Pm.Web.Shared;

namespace Ti.Pm.Web.Pages.Status
{
    public class StatusView : ComponentBase
    {
        public string mFilterTitle = "";

        public List<StatusPmVieweModel> StatusModels { get; set; }
        public List<TaskPmVieweModel> TaskModels { get; set; }

        [Inject] protected IDialogService DialogService { get; set; }
        [Inject] protected CreateDialogOptionService DialogOptionService { get; set; }
        [Inject] private LogApplicationService ApplicationErrorService { get; set; }
        [Inject] private StatusPmService StatusPmService { get; set; }
        [Inject] private TaskPmService TaskPmService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                StatusModels = await StatusPmService.GetAll();
                TaskModels = await TaskPmService.GetAll();
                var UpdatedModels = new List<StatusPmVieweModel>();
                foreach (var model in StatusModels)
                {
                    bool asnser = TaskPmService.CheckConnection(model.StatusId, "status");
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
                StatusModels = UpdatedModels;
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
                ApplicationErrorService.ErrorCathcer(ex);
            }

        }
        protected void ClearSearch()
        {
            FilterByTitle = "";
        }

        protected async Task DeleteDialogAsync(StatusPmVieweModel modelForDelete)
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
                        StatusPmService.Delete(modelForDelete);
                        StatusModels.Remove(modelForDelete);
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
                var newModel = new StatusPmVieweModel();
                var options = DialogOptionService.CreateDialogOptions();
                //передача параметра, стандартная конструкция МудБлазора
                var parameters = new DialogParameters<EditStatusPm> { { x => x.StatusPmVieweModel, newModel } };
                var dialog = DialogService.Show<EditStatusPm>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    StatusPmService.Create(newModel);
                    StatusModels.Add(newModel);
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                ApplicationErrorService.ErrorCathcer(ex);
            }
        }

        public async Task EditItemDialog(StatusPmVieweModel updateModel)
        {
            try
            {
                var options = DialogOptionService.CreateDialogOptions();
                //передача параметра, стандартная конструкция МудБлазора
                var parameters = new DialogParameters<EditStatusPm> { { x => x.StatusPmVieweModel, updateModel } };
                var dialog = DialogService.Show<EditStatusPm>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    StatusPmService.Update(updateModel);
                    StateHasChanged();
                }
                else
                {
                    var oldItem = StatusPmService.ReloadItem(updateModel);
                    oldItem.DeleteDisabled = updateModel.DeleteDisabled;
                    var index = StatusModels.FindIndex(x => x.StatusId == oldItem.StatusId);
                    StatusModels[index] = oldItem;
                    StateHasChanged();
                }

            }
            catch (Exception ex)
            {
                ApplicationErrorService.ErrorCathcer(ex);
            }
        }
        public async Task ShowChangeLogDialog(StatusPmVieweModel item)
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

