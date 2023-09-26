using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using MudBlazor;
using Ti.Pm.Web.Data.Service;
using Ti.Pm.Web.Data.ViewModel;
using Ti.Pm.Web.Pages.Tasks.Edit;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Ti.Pm.Web.Pages.KanbanBoard
{
    public class KanbanBoardViewe : ComponentBase
    {
        [Inject] protected IDialogService DialogService { get; set; }
        [Inject] private LogApplicationService applicationErrorService { get; set; }
        [Inject] private TaskPmService TaskPmService { get; set; }
        [Inject] public TaskTypePmService TaskTypePmService { get; set; }
        [Inject] public ProjectPmService ProjectPmService { get; set; }
        [Inject] public StatusPmService StatusPmService { get; set; }

        public List<TaskTypePmVieweModel> TaskTypePmVieweModels { get; set; }
        public List<ProjectPmVieweModel> ProjectPmVieweModels { get; set; }
        public List<StatusPmVieweModel> StatusPmVieweModels { get; set; }

        public List<TaskPmVieweModel> VieweModels { get; set; } = new List<TaskPmVieweModel>();

        public int filterProjectId = new();
        public string filterTaskType = "";
        public string filterTaskName = "";

        public List<DropItem> _items = new();

        public List<StatusPmVieweModel> _selectors = new();

        public MudDropContainer<DropItem> _container;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                TaskTypePmVieweModels = await TaskTypePmService.GetAll();
                ProjectPmVieweModels = await ProjectPmService.GetAll();
                StatusPmVieweModels = await StatusPmService.GetAll();
                VieweModels = await TaskPmService.GetAll();
                if (!ProjectPmVieweModels.IsNullOrEmpty())
                {
                    filterProjectId = ProjectPmVieweModels[0].ProjectId;
                }
                var SortStatusPmVieweModels = StatusPmVieweModels.OrderBy(x => x.OrderId);
                foreach (var item in SortStatusPmVieweModels)
                {
                    _selectors.Add(item);
                }
                GrandFilter();
                MakeItems();
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

        protected void GrandFilter()
        {
            try
            {
                VieweModels = TaskPmService.FilteringByProject(filterProjectId);
                if (filterTaskType != "")
                {
                    if (TaskTypePmVieweModels.FirstOrDefault(x => x.Title.Contains(filterTaskType)) != null)
                    {
                        var filteredList = VieweModels.Where(x => x.TaskTypeId == (TaskTypePmVieweModels.FirstOrDefault(x => x.Title.Contains(filterTaskType)).TaskTypeId)).ToList();
                        VieweModels = filteredList;
                    }
                    else
                    {
                        VieweModels.Clear();
                    }
                }
                if (FilterTaskName != "")
                {
                    var filteredSecondList = VieweModels.Where(x => x.Title.Contains(filterTaskName)).ToList();
                    VieweModels = filteredSecondList;
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

        public int FilterByProject
        {
            get => filterProjectId;

            set
            {
                filterProjectId = value;
                GrandFilter();
                MakeItems();
                RefreshContainer();
            }
        }
        public string FilterByTaskType
        {
            get => filterTaskType;
            set
            {
                filterTaskType = value;
                GrandFilter();
                MakeItems();
                RefreshContainer();
            }
        }
        public string FilterTaskName
        {
            get => filterTaskName;
            set
            {
                filterTaskName = value;
                GrandFilter();
                MakeItems();
                RefreshContainer();
            }
        }

        protected void ClearSearch()
        {
            try
            {
                filterTaskType = "";
                filterTaskName = "";
                GrandFilter();
                MakeItems();
                RefreshContainer();
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

        protected void MakeItems()
        {
            try
            {
                _items.Clear();
                foreach (var item in VieweModels)
                {
                    _items.Add(new DropItem() { Name = item, Selector = _selectors.FirstOrDefault(x => x.StatusId == item.StatusId) });
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

        protected void Delete(TaskPmVieweModel item)
        {
            try
            {
                TaskPmService.Delete(item);
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
        public async Task AddItemDialog(StatusPmVieweModel select, int FilterByProject)
        {
            try
            {
                var newItem = new TaskPmVieweModel();
                newItem.StatusId = select.StatusId;
                newItem.ProjectId = FilterByProject;
                var options = new DialogOptions() { CloseButton = false, MaxWidth = MaxWidth.Medium };
                var parameters = new DialogParameters<EditTaskPm> { { x => x.TaskPmVieweModel, newItem } };
                var dialog = DialogService.Show<EditTaskPm>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    TaskPmVieweModel returnModel = new TaskPmVieweModel();
                    returnModel = newItem;
                    var newUser = TaskPmService.Create(returnModel);
                    VieweModels.Add(newItem);
                    MakeItems();
                    RefreshContainer();
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
                    var newItem = TaskPmService.Update(returnModel);
                    var index = VieweModels.FindIndex(x => x.TaskId == newItem.TaskId);
                    VieweModels[index] = newItem;
                    MakeItems();
                    RefreshContainer();
                }
                else
                {
                    TaskPmVieweModel oldItem = new TaskPmVieweModel();
                    oldItem = TaskPmService.ReloadItem(item);
                    var index = VieweModels.FindIndex(x => x.TaskId == oldItem.TaskId);
                    VieweModels[index] = oldItem;
                    MakeItems();
                    RefreshContainer();
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

        public void ItemUpdated(MudItemDropInfo<DropItem> dropItem)
        {
            try
            {
                dropItem.Item.Selector = _selectors.FirstOrDefault(x => x.Title == dropItem.DropzoneIdentifier);
                dropItem.Item.Name.StatusId = dropItem.Item.Selector.StatusId;
                TaskPmService.Update(dropItem.Item.Name);
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

        public class DropItem
        {
            public TaskPmVieweModel Name { get; init; }
            public StatusPmVieweModel Selector { get; set; }
        }
        private void RefreshContainer()
        {
            try
            {
                StateHasChanged();
                _container.Refresh();
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
