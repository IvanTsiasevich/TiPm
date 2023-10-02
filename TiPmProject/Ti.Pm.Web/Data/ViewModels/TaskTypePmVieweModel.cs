using System.ComponentModel.DataAnnotations;
using Ti.Pm.PmDb.Model;

namespace Ti.Pm.Web.Data.ViewModel
{
    public class TaskTypePmVieweModel
    {
        private TaskTypePm mDbModel;
        public TaskTypePm DbModel => mDbModel;

        public TaskTypePmVieweModel()
        {
            mDbModel = new TaskTypePm();
        }

        public TaskTypePmVieweModel(TaskTypePm item)
        {
            mDbModel = item;
        }
        public bool DeleteDisabled
        {
            get; set;
        }
        public int TaskTypeId
        {
            get => mDbModel.TaskTypeId;
            set => mDbModel.TaskTypeId = value;
        }

        [Required]
        public string Title
        {
            get => mDbModel.Title;
            set => mDbModel.Title = value;
        }

        public string? ChangeLogJson
        {
            get => mDbModel.ChangeLogJson;
            set => mDbModel.ChangeLogJson = value;
        }
    }
}
