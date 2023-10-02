using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using MudBlazor;
using System.Text.Json;
using Ti.Pm.PmDb.Model;
using Ti.Pm.Web.Data.Service;
using Ti.Pm.Web.Data.Services;
using Ti.Pm.Web.Data.ViewModel;
using Ti.Pm.Web.Pages.Projects.Edit;
using Ti.Pm.Web.Shared;

namespace Ti.Pm.Web.Pages.Projects
{
    public class ProjectsPmView : ComponentBase
    {
        public string mFilterTitle = "";

        public List<TaskPmVieweModel> TaskModels { get; set; }
        public List<ProjectPmVieweModel> ProjectVieweModels { get; set; }

        [Inject] protected IDialogService DialogService { get; set; }
        [Inject] protected CreateDialogOptionService DialogOptionService { get; set; }
        [Inject] private LogApplicationService ApplicationErrorService { get; set; }
        [Inject] private ProjectPmService ProjectService { get; set; }
        [Inject] private TaskPmService TaskPmService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                ProjectVieweModels = await ProjectService.GetAll();
                TaskModels = await TaskPmService.GetAll();
                var UpdatedModels = new List<ProjectPmVieweModel>();
                foreach (var model in ProjectVieweModels)
                {
                    bool asnser = TaskPmService.CheckConnection(model.ProjectId, "project");
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
                ProjectVieweModels = ProjectService.FilteringByTitle(mFilterTitle);
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

        protected async Task DeleteDialogAsync(ProjectPmVieweModel item)
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
                        ProjectService.Delete(item);
                        ProjectVieweModels.Remove(item);
                        StateHasChanged();
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException is SqlException || ex is SqlException)
                        {
                            options = DialogOptionService.CreateDialogOptions();
                            dialog = DialogService.Show<DeleteErrorModal>("", options);
                            item.DeleteDisabled = true;
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
                var newModel = new ProjectPmVieweModel();
                var options = DialogOptionService.CreateDialogOptions();
                //передача параметра, стандартная конструкция МудБлазора
                var parameters = new DialogParameters<EditProjectPm> { { x => x.ProjectViewModel, newModel } };
                var dialog = DialogService.Show<EditProjectPm>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    ProjectService.Create(newModel);
                    ProjectVieweModels.Add(newModel);
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                ApplicationErrorService.ErrorCathcer(ex);
            }
        }

        public async Task EditItemDialog(ProjectPmVieweModel updateModel)
        {
            try
            {
                var options = DialogOptionService.CreateDialogOptions();
                //передача параметра, стандартная конструкция МудБлазора
                var parameters = new DialogParameters<EditProjectPm> { { x => x.ProjectViewModel, updateModel } };
                var dialog = DialogService.Show<EditProjectPm>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    ProjectService.Update(updateModel);                    
                    StateHasChanged();
                }
                else
                {
                    var oldItem = ProjectService.ReloadItem(updateModel);
                    oldItem.DeleteDisabled = updateModel.DeleteDisabled;
                    var index = ProjectVieweModels.FindIndex(x => x.ProjectId == oldItem.ProjectId);
                    ProjectVieweModels[index] = oldItem;
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                ApplicationErrorService.ErrorCathcer(ex);
            }
        }
        public async Task ShowChangeLogDialog(ProjectPmVieweModel item)
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
