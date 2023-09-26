using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using MudBlazor;
using System.Linq;
using Ti.Pm.Web.Data.Service;
using Ti.Pm.Web.Data.ViewModel;
using Ti.Pm.Web.Pages.Projects.Edit;
using Ti.Pm.Web.Shared;

namespace Ti.Pm.Web.Pages.Projects
{
    public class ProjectsPmView : ComponentBase
    {
        [Inject] protected IDialogService DialogService { get; set; }
        [Inject] private LogApplicationService applicationErrorService { get; set; }
        [Inject] private ProjectPmService Service { get; set; }
        [Inject] private TaskPmService TaskService { get; set; }
        public List<TaskPmVieweModel> TaskModels { get; set; } = new List<TaskPmVieweModel>();
        public List<ProjectPmVieweModel> VieweModels { get; set; } = new List<ProjectPmVieweModel>();

        public string filterText = "";

        protected override async Task OnInitializedAsync()
        {
            try
            {
                VieweModels = await Service.GetAll();
                TaskModels = await TaskService.GetAll();
                var UpdatedModels = new List<ProjectPmVieweModel>();
                foreach (var model in VieweModels)
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
                            var index = VieweModels.FindIndex(x => x.ProjectId == item.ProjectId);
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
                var newItem = new ProjectPmVieweModel();
                var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Medium };
                var parameters = new DialogParameters<EditProjectPm> { { x => x.ProjectViewModel, newItem } };
                var dialog = DialogService.Show<EditProjectPm>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    ProjectPmVieweModel returnModel = new ProjectPmVieweModel();
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
                    var newItem = Service.Update(returnModel);
                    var index = VieweModels.FindIndex(x => x.ProjectId == newItem.ProjectId);
                    newItem.DeleteDisabled = VieweModels[index].DeleteDisabled;
                    VieweModels[index] = newItem;
                    StateHasChanged();
                }
                else
                {
                    var oldItem = Service.ReloadItem(item);
                    var index = VieweModels.FindIndex(x => x.ProjectId == oldItem.ProjectId);
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
