using System.ComponentModel.DataAnnotations;
using Ti.Pm.PmDb.Model;
using Ti.Pm.Web.Pages.TaskType;

namespace Ti.Pm.Web.Data.ViewModel
{
    public class TaskTypePmVieweModel
    {
        private TaskTypePm _item;
        public TaskTypePm Item => _item;

        public TaskTypePmVieweModel()
        {
            _item = new TaskTypePm();

        }

        public TaskTypePmVieweModel(TaskTypePm item)
        {
            _item = item;
        }
        public bool DeleteDisabled
        {
            get; set;
        }
        public int TaskTypeId
        {
            get => _item.TaskTypeId;
            set => _item.TaskTypeId = value;
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
    }
}
