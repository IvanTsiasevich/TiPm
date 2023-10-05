using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.Json;
using Ti.Pm.PmDb.Model;
using Ti.Pm.Web.Data.Service;
using Ti.Pm.Web.Data.Services;
using Ti.Pm.Web.Data.ViewModel;
using Ti.Pm.Web.Data.ViewModels;
using Ti.Pm.Web.Pages.Users.Edit;
using Ti.Pm.Web.Shared;

namespace Ti.Pm.Web.Pages.Users
{
    public class UsersView : ComponentBase
    {
        public List<UserVieweModel> UsersModels { get; set; }

        [Inject] protected IDialogService DialogService { get; set; }
        [Inject] protected CreateDialogOptionService DialogOptionService { get; set; }
        [Inject] private LogApplicationService ApplicationErrorService { get; set; }
        [Inject] private UserService UserService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                UsersModels = await UserService.GetAll();
            }
            catch (Exception ex)
            {
                ApplicationErrorService.ErrorCathcer(ex);
            }

        }

        protected async Task DeleteDialogAsync(UserVieweModel modelForDelete)
        {
            try
            {
                var options = DialogOptionService.CreateDialogOptions();
                var dialog = DialogService.Show<DeleteModal>("", options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    try
                    {
                        UserService.Delete(modelForDelete);
                        UsersModels.Remove(modelForDelete);
                        StateHasChanged();
                    }
                    catch (Exception ex)
                    {
                        ApplicationErrorService.ErrorCathcer(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationErrorService.ErrorCathcer(ex);
            }
        }

        public async Task AddItemDialog()
        {
            try
            {
                var newModel = new UserVieweModel();
                var options = DialogOptionService.CreateDialogOptions();
                //передача параметра, стандартная конструкция МудБлазора
                var parameters = new DialogParameters<EditUsers> { { x => x.UserVieweModel, newModel } };
                var dialog = DialogService.Show<EditUsers>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    UserService.Create(newModel);
                    UsersModels.Add(newModel);
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                ApplicationErrorService.ErrorCathcer(ex);
            }
        }

        public async Task EditItemDialog(UserVieweModel updateModel)
        {
            try
            {
                var options = DialogOptionService.CreateDialogOptions();
                //передача параметра, стандартная конструкция МудБлазора
                var parameters = new DialogParameters<EditUsers> { { x => x.UserVieweModel, updateModel } };
                var dialog = DialogService.Show<EditUsers>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    UserService.Update(updateModel);
                    StateHasChanged();
                }
                else
                {
                    var oldItem = UserService.ReloadItem(updateModel);
                    var index = UsersModels.FindIndex(x => x.UserId == oldItem.UserId);
                    UsersModels[index] = oldItem;
                    StateHasChanged();
                }

            }
            catch (Exception ex)
            {
                ApplicationErrorService.ErrorCathcer(ex);
            }
        }
        public async Task ShowChangeLogDialog(UserVieweModel item)
        {
            try
            {
                var changeLogJson = string.IsNullOrEmpty(item.ChangeLogJson) ? new List<ChangeLog>() : JsonSerializer.Deserialize<List<ChangeLog>>(item.ChangeLogJson);
                var options = DialogOptionService.CreateDialogOptions();
                var parameters = new DialogParameters<ChangeLogModal> { { x => x.ChangeLog, changeLogJson } };
                DialogService.Show<ChangeLogModal>("", parameters, options);
            }
            catch (Exception ex)
            {
                ApplicationErrorService.ErrorCathcer(ex);
            }
        }

    }
}


