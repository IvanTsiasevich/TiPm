using System.ComponentModel.DataAnnotations;
using Ti.Pm.PmDb.Model;
using Ti.Pm.Web.Pages.Status;

namespace Ti.Pm.Web.Data.ViewModel
{
    public class StatusPmVieweModel
    {
        private StatusPm _item;
        public StatusPm Item => _item;

        public StatusPmVieweModel()
        {
            _item = new StatusPm();

        }

        public StatusPmVieweModel(StatusPm item)
        {
            _item = item;
        }
        public bool DeleteDisabled
        {
            get;set;
        }
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
        public int OrderId
        {
            get => _item.OrderId;
            set => _item.OrderId = value;
        }
    }
}
