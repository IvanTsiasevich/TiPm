using System.ComponentModel.DataAnnotations;
using Ti.Pm.PmDb.Model;

namespace Ti.Pm.Web.Data.ViewModel
{
    public class ApplicationErrorViewModel
    {
        private LogApplicationError _item;
        public LogApplicationError Item => _item;

        public ApplicationErrorViewModel()
        {
            _item = new LogApplicationError();

        }

        public ApplicationErrorViewModel(LogApplicationError item)
        {
            _item = item;
        }

        public int LogApplicationErrorId
		{
            get => _item.LogApplicationErrorId;
            set => _item.LogApplicationErrorId = value;
        }

        public DateTime InsertDate
        {
            get => _item.InsertDate;
            set => _item.InsertDate = value;
        }

        public string ErrorContext
        {
            get => _item.ErrorContext;
            set => _item.ErrorContext = value;
        }
        public string ErrorMessage
        {
            get => _item.ErrorMessage;
            set => _item.ErrorMessage = value;
        }
        public string? ErrorInnerException
        {
            get => _item.ErrorInnerException;
            set => _item.ErrorInnerException = value;
        }              
    }
}
