using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;
using MudBlazor;
using System.Text.Json;
using Ti.Pm.PmDb.Model;
using Ti.Pm.Web.Data.Service;
using Ti.Pm.Web.Data.Services;
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
                ApplicationErrorService.ErrorCathcer(ex);
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
                        int taskTypeId = TaskTypePmVieweModels.FirstOrDefault(x => x.Title.ToLower().Contains(mFilterTaskType.ToLower())).TaskTypeId;
                        var filteredList = TaskPmVieweModels.Where(x => x.TaskTypeId == taskTypeId).ToList();
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
                ApplicationErrorService.ErrorCathcer(ex);
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
                ApplicationErrorService.ErrorCathcer(ex);
            }

        }

        protected void MakeItems()
        {
            try
            {
                mItems.Clear();
                foreach (var item in TaskPmVieweModels)
                {
                    mItems.Add(new DropItem()
                    {
                        Name = item,
                        Selector = mSelectors.FirstOrDefault(x => x.StatusId == item.StatusId)
                    });
                }
            }
            catch (Exception ex)
            {
                ApplicationErrorService.ErrorCathcer(ex);
            }

        }
       
        public async Task AddItemDialog(StatusPmVieweModel selector, int FilterByProject)
        {
            try
            {
                var newModel = new TaskPmVieweModel()
                {
                    StatusId = selector.StatusId,
                    ProjectId = FilterByProject
                };
                var options = DialogOptionService.CreateDialogOptions();
                //передача параметра, стандартная конструкция МудБлазора
                var parameters = new DialogParameters<EditTaskPm>{ { x => x.TaskPmVieweModel,  newModel  } };
                var dialog = DialogService.Show<EditTaskPm>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    TaskPmService.Create(newModel);
                    TaskPmVieweModels.Add(newModel);
                    MakeItems();
                    RefreshContainer();
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
                    TaskPmService.Update(updateModel);                    
                    MakeItems();
                    RefreshContainer();
                }
                else
                {
                    var oldItem = TaskPmService.ReloadItem(updateModel);
                    var index = TaskPmVieweModels.FindIndex(x => x.TaskId == oldItem.TaskId);
                    TaskPmVieweModels[index] = oldItem;
                    MakeItems();
                    RefreshContainer();
                }

            }
            catch (Exception ex)
            {
                ApplicationErrorService.ErrorCathcer(ex);
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
                ApplicationErrorService.ErrorCathcer(ex);
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
                ApplicationErrorService.ErrorCathcer(ex);
            }

        }
        public async Task ShowChangeLogDialog(TaskPmVieweModel model)
        {
            try
            {
                var item = TaskPmService.FindById(model);
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
        public class DropItem
        {
            public TaskPmVieweModel Name { get; init; }
            public StatusPmVieweModel Selector { get; set; }
        }
    }
}