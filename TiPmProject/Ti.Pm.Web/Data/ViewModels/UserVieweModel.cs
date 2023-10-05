using System.ComponentModel.DataAnnotations;
using Ti.Pm.PmDb.Model;

namespace Ti.Pm.Web.Data.ViewModels
{
    public class UserVieweModel
    {
        private User mDbModel;
        public User DbModel => mDbModel;

        public UserVieweModel()
        {
            mDbModel = new User();
        }

        public UserVieweModel(User item)
        {
            mDbModel = item;
        }

        public int UserId
        {
            get => mDbModel.UserId;
            set => mDbModel.UserId = value;
        }

        [Required]
        public string PersonName
        {
            get => mDbModel.PersonName;
            set => mDbModel.PersonName = value;
        }
        [Required]
        public string PersonSurname
        {
            get => mDbModel.PersonSurname;
            set => mDbModel.PersonSurname = value;
        }
        [Required]
        public string Password
        {
            get => mDbModel.Password;
            set => mDbModel.Password = value;
        }
        [Required]
        [EmailAddress]
        public string Login
        {
            get => mDbModel.Login;
            set => mDbModel.Login = value;
        }
        [Required]        
        public int RoleId
        {
            get => mDbModel.RoleId;
            set => mDbModel.RoleId = value;
        }
        public string? ChangeLogJson
        {
            get => mDbModel.ChangeLogJson;
            set => mDbModel.ChangeLogJson = value;
        }
    }
}

