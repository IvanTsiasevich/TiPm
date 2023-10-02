using System.ComponentModel.DataAnnotations;
using Ti.Pm.PmDb.Model;

namespace Ti.Pm.Web.Data.ViewModel
{
    public class StatusPmVieweModel
    {
        private StatusPm mDbModel;
        public StatusPm DbModel => mDbModel;

        public StatusPmVieweModel()
        {
            mDbModel = new StatusPm();
        }

        public StatusPmVieweModel(StatusPm item)
        {
            mDbModel = item;
        }
        public bool DeleteDisabled
        {
            get;set;
        }
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
        public int OrderId
        {
            get => mDbModel.OrderId;
            set => mDbModel.OrderId = value;
        }
    }
}
