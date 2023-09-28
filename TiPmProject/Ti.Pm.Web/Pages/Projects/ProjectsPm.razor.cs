using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using MudBlazor;
using System.Linq;
using System.Text.Json;
using Ti.Pm.PmDb.Model;
using Ti.Pm.Web.Data.Service;
using Ti.Pm.Web.Data.ViewModel;
using Ti.Pm.Web.Pages.Projects.Edit;
using Ti.Pm.Web.Shared;

namespace Ti.Pm.Web.Pages.Projects
{
    public class ProjectsPmView : ComponentBase
    {
        public string mFilterTitle = "";

        [Inject] protected IDialogService DialogService { get; set; }
        [Inject] private LogApplicationService ApplicationErrorService { get; set; }
        [Inject] private ProjectPmService ProjectService { get; set; }
        [Inject] private TaskPmService TaskService { get; set; }
        public List<TaskPmVieweModel> TaskModels { get; set; } 
        public List<ProjectPmVieweModel> ProjectVieweModels { get; set; } 


        protected override async Task OnInitializedAsync()
        {
            try
            {
                ProjectVieweModels = await ProjectService.GetAll();
                TaskModels = await TaskService.GetAll();
                var UpdatedModels = new List<ProjectPmVieweModel>();
                foreach (var model in ProjectVieweModels)
                {
                    if (TaskModels.Any(x => x.ProjectId == model.ProjectId))
                    {
                        model.DeleteDisabled = true;
                        UpdatedModels.Add(model);
                    }
                    else
                    {
                        UpdatedModels.Add(model);
                    }
                }
                ProjectVieweModels = UpdatedModels;
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
                ProjectVieweModels = ProjectService.FilteringByTitle(mFilterTitle);
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

        protected async Task DeleteDialogAsync(ProjectPmVieweModel item)
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
                        ProjectService.Delete(item);
                        ProjectVieweModels.Remove(item);
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
                            var index = ProjectVieweModels.FindIndex(x => x.ProjectId == item.ProjectId);
                            ProjectVieweModels[index] = item;
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
                var newItem = new ProjectPmVieweModel();
                var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Medium };
                var parameters = new DialogParameters<EditProjectPm> { { x => x.ProjectViewModel, newItem } };
                var dialog = DialogService.Show<EditProjectPm>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    ProjectPmVieweModel returnModel = new ProjectPmVieweModel();
                    returnModel = newItem;
                    var newUser = ProjectService.Create(returnModel);
                    ProjectVieweModels.Add(newItem);
                    StateHasChanged();
                }

            }
            catch (Exception ex)
            {
                ApplicationErrorService.Cathcer(ex);
            }
        }

        public async Task EditItemDialog(ProjectPmVieweModel item)
        {
            try
            {
                var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Medium };
                var parameters = new DialogParameters<EditProjectPm> { { x => x.ProjectViewModel, item } };
                var dialog = DialogService.Show<EditProjectPm>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    ProjectPmVieweModel returnModel = new ProjectPmVieweModel();
                    returnModel = (ProjectPmVieweModel)result.Data;
                    var newItem = ProjectService.Update(returnModel);
                    var index = ProjectVieweModels.FindIndex(x => x.ProjectId == newItem.ProjectId);
                    newItem.DeleteDisabled = ProjectVieweModels[index].DeleteDisabled;
                    ProjectVieweModels[index] = newItem;
                    StateHasChanged();
                }
                else
                {
                    var oldItem = ProjectService.ReloadItem(item);
                    var index = ProjectVieweModels.FindIndex(x => x.ProjectId == oldItem.ProjectId);
                    ProjectVieweModels[index] = oldItem;
                    StateHasChanged();
                }

            }
            catch (Exception ex)
            {
                ApplicationErrorService.Cathcer(ex);
            }
        }
        public async Task ShowChangeLogDialog(ProjectPmVieweModel item)
        {
            try
            {
                var changeLogJson = string.IsNullOrEmpty(item.ChangeLogJson) ? new List<ChangeLog>() : JsonSerializer.Deserialize<List<ChangeLog>>(item.ChangeLogJson);
                var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Medium };
                var parameters = new DialogParameters<ChangeLogModal> { { x => x.ChangeLog, changeLogJson } };
                var dialog = DialogService.Show<ChangeLogModal>("", parameters, options);
                var result = await dialog.Result;

            }
            catch (Exception ex)
            {
                ApplicationErrorService.Cathcer(ex);
            }
        }


    }
}
