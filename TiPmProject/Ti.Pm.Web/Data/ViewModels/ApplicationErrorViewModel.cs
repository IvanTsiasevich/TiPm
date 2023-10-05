using Ti.Pm.PmDb.Model;

namespace Ti.Pm.Web.Data.ViewModel
{
    public class ApplicationErrorViewModel
    {
        private ApplicationError mDbModel;
        public ApplicationError DbModel => mDbModel;

        public ApplicationErrorViewModel()
        {
            mDbModel = new ApplicationError();
        }

        public ApplicationErrorViewModel(ApplicationError dbModel)
        {
            mDbModel = dbModel;
        }

        public int LogApplicationErrorId
		{
            get => mDbModel.LogApplicationErrorId;
            set => mDbModel.LogApplicationErrorId = value;
        }

        public DateTime InsertDate
        {
            get => mDbModel.InsertDate;
            set => mDbModel.InsertDate = value;
        }

        public string ErrorContext
        {
            get => mDbModel.ErrorContext;
            set => mDbModel.ErrorContext = value;
        }
        public string ErrorMessage
        {
            get => mDbModel.ErrorMessage;
            set => mDbModel.ErrorMessage = value;
        }
        public string? UserName
        {
            get => mDbModel.UserName;
            set => mDbModel.UserName = value;
        }
        public string? ErrorInnerException
        {
            get => mDbModel.ErrorInnerException;
            set => mDbModel.ErrorInnerException = value;
        }              
    }
}
