using System.ComponentModel.DataAnnotations;
using Ti.Pm.PmDb.Model;

namespace Ti.Pm.Web.Data.ViewModel
{
    public class ProjectPmVieweModel
    {
        private ProjectPm mDbModel;
        public ProjectPm DbModel => mDbModel;

        public ProjectPmVieweModel()
        {
            mDbModel = new ProjectPm();
        }

        public ProjectPmVieweModel(ProjectPm item)
        {
            mDbModel = item;
        }
        public bool DeleteDisabled
        {
            get; set;
        }
        public int ProjectId
        {
            get => mDbModel.ProjectId;
            set => mDbModel.ProjectId = value;
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
