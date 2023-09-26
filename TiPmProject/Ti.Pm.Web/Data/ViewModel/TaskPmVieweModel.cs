using System.ComponentModel.DataAnnotations;
using Ti.Pm.PmDb.Model;

namespace Ti.Pm.Web.Data.ViewModel
{
    public class TaskPmVieweModel
    {
        private TaskPm _item;
        public TaskPm Item => _item;

        public TaskPmVieweModel()
        {
            _item = new TaskPm();

        }

        public TaskPmVieweModel(TaskPm item)
        {
            _item = item;
        }
        public int TaskId
        {
            get => _item.TaskId;
            set => _item.TaskId = value;
        }
        [Required]
        [Range(1, 10000000000000000000, ErrorMessage = "Need to choose Task Type")]

        public int TaskTypeId
        {
            get => _item.TaskTypeId;
            set => _item.TaskTypeId =value;
        }
        [Required]
        [Range(1, 10000000000000000000, ErrorMessage = "Need to choose Project")]

        public int ProjectId
        {
            get => _item.ProjectId;
            set => _item.ProjectId = value;
        }
        [Required]
        [Range(1, 10000000000000000000, ErrorMessage = "Need to choose Status")]
        public int StatusId
        {
            get => _item.StatusId;
            set => _item.StatusId = value;
        }       

        [Required]
        public string Title
        {
            get => _item.Title;
            set => _item.Title = value;
        }

        public string? ChangeLogJson
        {
            get => _item.ChangeLogJson;
            set => _item.ChangeLogJson = value;
        }
        [Required]
        public string Description
        {
            get => _item.Description;
            set => _item.Description = value;
        }
    }
}
