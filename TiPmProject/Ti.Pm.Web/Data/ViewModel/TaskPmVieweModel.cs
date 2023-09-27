using System.ComponentModel.DataAnnotations;
using Ti.Pm.PmDb.Model;

namespace Ti.Pm.Web.Data.ViewModel
{
    public class TaskPmVieweModel
    {
        private TaskPm mDbModel;
        public TaskPm DbModel => mDbModel;

        public TaskPmVieweModel()
        {
            mDbModel = new TaskPm();
        }

        public TaskPmVieweModel(TaskPm item)
        {
            mDbModel = item;
        }
        public int TaskId
        {
            get => mDbModel.TaskId;
            set => mDbModel.TaskId = value;
        }
        [Required]
        [Range(1, 10000000000000000000, ErrorMessage = "Need to choose Task Type")]

        public int TaskTypeId
        {
            get => mDbModel.TaskTypeId;
            set => mDbModel.TaskTypeId =value;
        }
        [Required]
        [Range(1, 10000000000000000000, ErrorMessage = "Need to choose Project")]

        public int ProjectId
        {
            get => mDbModel.ProjectId;
            set => mDbModel.ProjectId = value;
        }
        [Required]
        [Range(1, 10000000000000000000, ErrorMessage = "Need to choose Status")]
        public int StatusId
        {
            get => mDbModel.StatusId;
            set => mDbModel.StatusId = value;
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
        [Required]
        public string Description
        {
            get => mDbModel.Description;
            set => mDbModel.Description = value;
        }
    }
}
