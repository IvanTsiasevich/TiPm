using System.ComponentModel.DataAnnotations;
using Ti.Pm.PmDb.Model;
using Ti.Pm.Web.Pages.Projects;

namespace Ti.Pm.Web.Data.ViewModel
{
    public class ProjectPmVieweModel
    {
        private ProjectPm _item;
        public ProjectPm Item => _item;

        public ProjectPmVieweModel()
        {
            _item = new ProjectPm();
        }

        public ProjectPmVieweModel(ProjectPm item)
        {
            _item = item;
        }
        public bool DeleteDisabled
        {
            get; set;
        }
        public int ProjectId
        {
            get => _item.ProjectId;
            set => _item.ProjectId = value;
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
