using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using MudBlazor;
using System.Text.Json;
using Ti.Pm.PmDb.Model;
using Ti.Pm.Web.Data.Service;
using Ti.Pm.Web.Data.ViewModel;
using Ti.Pm.Web.Pages.Tasks.Edit;
using Ti.Pm.Web.Shared;

namespace Ti.Pm.Web.Pages.KanbanBoard
{
    public class KanbanBoardViewe : ComponentBase
    {
        public int mFilterProjectId = new();
        public string mFilterTaskType = "";
        public string mFilterTaskName = "";

        public List<DropItem> mItems = new();

        public List<StatusPmVieweModel> mSelectors = new();

        public MudDropContainer<DropItem> mContainer = new();

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
                if (!ProjectPmVieweModels.IsNullOrEmpty())
                {
                    mFilterProjectId = ProjectPmVieweModels[0].ProjectId;
                }
                var SortStatusPmVieweModels = StatusPmVieweModels.OrderBy(x => x.OrderId);
                foreach (var item in SortStatusPmVieweModels)
                {
                    mSelectors.Add(item);
                }
                GrandFilter();
                MakeItems();
            }
            catch (Exception ex)
            {
                ApplicationErrorService.Cathcer(ex);
            }
        }

        protected void GrandFilter()
        {
            try
            {
                TaskPmVieweModels = TaskPmService.FilteringByProject(mFilterProjectId);
                if (mFilterTaskType != "")
                {
                    if (TaskTypePmVieweModels.FirstOrDefault(x => x.Title.ToLower().Contains(mFilterTaskType.ToLower())) != null)
                    {
                        var filteredList = TaskPmVieweModels.Where(x => x.TaskTypeId == (TaskTypePmVieweModels.FirstOrDefault(x => x.Title.ToLower().Contains(mFilterTaskType.ToLower())).TaskTypeId)).ToList();
                        TaskPmVieweModels = filteredList;
                    }
                    else
                    {
                        TaskPmVieweModels.Clear();
                    }
                }
                if (FilterTaskName != "")
                {
                    var filteredSecondList = TaskPmVieweModels.Where(x => x.Title.ToLower().Contains(mFilterTaskName.ToLower())).ToList();
                    TaskPmVieweModels = filteredSecondList;
                }
            }
            catch (Exception ex)
            {
                ApplicationErrorService.Cathcer(ex);
            }

        }

        public int FilterByProject
        {
            get => mFilterProjectId;

            set
            {
                mFilterProjectId = value;
                GrandFilter();
                MakeItems();
                RefreshContainer();
            }
        }
        public string FilterByTaskType
        {
            get => mFilterTaskType;
            set
            {
                mFilterTaskType = value;
                GrandFilter();
                MakeItems();
                RefreshContainer();
            }
        }
        public string FilterTaskName
        {
            get => mFilterTaskName;
            set
            {
                mFilterTaskName = value;
                GrandFilter();
                MakeItems();
                RefreshContainer();
            }
        }

        protected void ClearSearch()
        {
            try
            {
                mFilterTaskType = "";
                mFilterTaskName = "";
                GrandFilter();
                MakeItems();
                RefreshContainer();
            }
            catch (Exception ex)
            {
                ApplicationErrorService.Cathcer(ex);
            }

        }

        protected void MakeItems()
        {
            try
            {
                mItems.Clear();
                foreach (var item in TaskPmVieweModels)
                {
                    mItems.Add(new DropItem() { Name = item, Selector = mSelectors.FirstOrDefault(x => x.StatusId == item.StatusId) });
                }
            }
            catch (Exception ex)
            {
                ApplicationErrorService.Cathcer(ex);
            }

        }

        protected void Delete(TaskPmVieweModel item)
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
                    TaskPmVieweModels.Add(newItem);
                    MakeItems();
                    RefreshContainer();
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
                    var index = TaskPmVieweModels.FindIndex(x => x.TaskId == newItem.TaskId);
                    TaskPmVieweModels[index] = newItem;
                    MakeItems();
                    RefreshContainer();
                }
                else
                {
                    TaskPmVieweModel oldItem = new TaskPmVieweModel();
                    oldItem = TaskPmService.ReloadItem(item);
                    var index = TaskPmVieweModels.FindIndex(x => x.TaskId == oldItem.TaskId);
                    TaskPmVieweModels[index] = oldItem;
                    MakeItems();
                    RefreshContainer();
                }

            }
            catch (Exception ex)
            {
                ApplicationErrorService.Cathcer(ex);
            }
        }

        public void ItemUpdated(MudItemDropInfo<DropItem> dropItem)
        {
            try
            {
                dropItem.Item.Selector = mSelectors.FirstOrDefault(x => x.Title == dropItem.DropzoneIdentifier);
                dropItem.Item.Name.StatusId = dropItem.Item.Selector.StatusId;
                TaskPmService.Update(dropItem.Item.Name);
            }
            catch (Exception ex)
            {
                ApplicationErrorService.Cathcer(ex);
            }
        }

        private void RefreshContainer()
        {
            try
            {
                StateHasChanged();
                mContainer.Refresh();
            }
            catch (Exception ex)
            {
                ApplicationErrorService.Cathcer(ex);
            }

        }
        public async Task ShowChangeLogDialog(TaskPmVieweModel model)
        {
            try
            {
                var item = TaskPmService.FindById(model);
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
        public class DropItem
        {
            public TaskPmVieweModel Name { get; init; }
            public StatusPmVieweModel Selector { get; set; }
        }
    }
}