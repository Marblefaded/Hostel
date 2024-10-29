using HostelDB.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Suo.Admin.Data.Models.Request;
using Suo.Admin.Data.Models.Responce;
using Suo.Admin.Data.Service;
using MudBlazor;
using Suo.Admin.Pages.Headmans.Edit;
using Suo.Admin.Shared;
using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Pages.Headmans
{
    public class HeadmansView : ComponentBase
    {
        [Inject] protected ISnackbar Snackbar { get; set; }
        [Inject] protected IDialogService DialogService { get; set; }
        protected List<UserResponse> Model { get; set; }
        public List<AmsUserViewModel> AmsUsers { get; set; }
        [Inject] protected AspNetUserManagment Service { get; set; }
        [Inject] protected AmsUserService AmsUserService { get; set; }
        public UserResponse mCurrentItem;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Model = await Service.GetAllHeadmanInAuth();
                await InvokeAsync(StateHasChanged);
            }
        }

        public async Task AddItemDialog()
        {
            try
            {
                var newItem = new UserAddRequest();

                var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
                var parameters = new DialogParameters<EditHead> { { x => x.UserAdd, newItem } };
                parameters.Add(x => x.Title, "Создание старосты");
                var dialog = DialogService.Show<EditHead>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    UserAddRequest returnModel = new UserAddRequest();
                    returnModel = newItem;
                    /*returnModel.FirstName = "SUO";
                    returnModel.LastName = "Headman";*/
                    returnModel.PhoneNumber = "123141512";
                    returnModel.Id = "";
                    returnModel.UserName = returnModel.FirstName + returnModel.LastName;
                    var newUser = await Service.AddHeadmanInAuth(returnModel);
                    Model.Add(newUser);
                    Snackbar.Add("Элемент сохранен", Severity.Success);
                    StateHasChanged();
                }

            }
            catch (Exception ex)
            {
                /*LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);*/
            }
        }
        /*public async Task EditItemDialog(UserResponse item)
        {
            try
            {
                var newItem = new UserAddRequest();

                var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
                var parameters = new DialogParameters<EditHead> { { x => x.UserAdd, newItem } };
                parameters.Add(x => x.Title, "Создание старосты");
                var dialog = DialogService.Show<EditHead>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    UserAddRequest returnModel = new UserAddRequest();
                    returnModel = newItem;
                    returnModel.PhoneNumber = "123141512";
                    returnModel.Id = "";
                    returnModel.UserName = returnModel.FirstName + returnModel.LastName;
                    var newUser = await Service.AddHeadmanInAuth(returnModel);
                    Model.Add(newUser);
                    Snackbar.Add("Элемент сохранен", Severity.Success);
                    StateHasChanged();
                }

            }
            catch (Exception ex)
            {
                *//*LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);*//*
            }
        }*/
        public async Task DeleteItemAsync(UserResponse mCurrentItem)
        {
            try
            {
                var dialog = DialogService.Show<DeleteComponent>("Вы точно хотите удалить старосту?");
                var result = await dialog.Result;
                if (!result.Canceled)
                {

                    await Service.RemoveUser(mCurrentItem.Id);
                    /*var aspUser = AmsUsers.FirstOrDefault(x => x.UserId.ToString() == mCurrentItem.Id);
                    if (aspUser != null)
                    {
                        AmsUserService.Delete(aspUser);
                        AmsUsers.Remove(aspUser);
                    }*/
                    Model.Remove(mCurrentItem);
                    Snackbar.Add("Элемент удален", Severity.Success);
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                /*LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);*/
            }
        }
    }
}
