using Microsoft.AspNetCore.Components;
using MudBlazor;
using Suo.Admin.Data.EditModel;
using Suo.Admin.Data.Service;
using Suo.Admin.Data.ViewModel;
using Suo.Admin.Pages.User.Edit;
using Suo.Admin.Shared;

namespace Suo.Admin.Pages.User
{
    public class UserView : ComponentBase
    {
        [Inject] protected UserService Service { get; set; }
        [Inject] protected IDialogService DialogService { get; set; }
        [Inject] protected ISnackbar Snackbar { get; set; }
        [Inject] protected TelegramUserService TelegramServ { get; set; }
        [Inject] protected UserRoomService UserRoomServ { get; set; }
        [Inject] protected AmsUserService AmsUserService { get; set; }
        [Inject] private LogApplicationService LogService { get; set; }
        [Inject] protected AspNetUserManagment AspNetUserManagment { get; set; }
        public LogApplicationViewModel LogModel = new LogApplicationViewModel();
        protected List<UserViewModel> Model { get; set; }
        public UserViewModel mCurrentItem;
        public EditUserItemViewModel mEditViewModel = new EditUserItemViewModel();

        public string CurrentPhoneNumber { get; set; }

        public List<UserRoomViewModel> UserRooms { get; set; }
        public List<TelegramUserVieweModel> TeleUsers { get; set; }
        public List<AmsUserViewModel> AmsUsers { get; set; }


        public bool isRemove;
        public string filterValue = "";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                UserRooms = UserRoomServ.Get();
                TeleUsers = TelegramServ.Get();
                AmsUsers = AmsUserService.Get();
                Model = Service.Get();
                await InvokeAsync(StateHasChanged);
            }
        }

        public string FilterValue
        {
            get => filterValue;

            set
            {
                filterValue = value;
                Filter();
            }
        }
        protected void Filter()
        {
            Model = Service.FilteringEmploers(filterValue);
            StateHasChanged();
        }
        //Как оптимизировать?
        public void FilterFacultet(UserViewModel item)
        {
            FilterValue = item.UserDeportament;
            StateHasChanged();
        }
        public void FilterСourse(UserViewModel item)
        {
            FilterValue = item.UserCourse;
            StateHasChanged();
        }
        public void FilterGroup(UserViewModel item)
        {
            FilterValue = item.UserGroup;
            StateHasChanged();
        }
        public async Task AddItemDialog()
        {
            try
            {
                var newItem = new UserViewModel();

                var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
                var parameters = new DialogParameters<EditUser> { { x => x.UserViewModel, newItem } };
                parameters.Add(x => x.Title, "Создание проживающего");
                var dialog = DialogService.Show<EditUser>("", parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    UserViewModel returnModel = new UserViewModel();
                    //returnModel = (UserViewModel)result.Data;
                    returnModel = newItem;
                    var newUser = Service.Create(returnModel);
                    Model.Add(newItem);
                    Snackbar.Add("Элемент сохранен", Severity.Success);
                    StateHasChanged();
                }

            }
            catch (Exception ex)
            {
                /*LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);*/
            }
        }

        public async void EditItemDialog(UserViewModel item)
        {
            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
            var parameters = new DialogParameters<EditUser> { { x => x.UserViewModel, item } };
            parameters.Add(x => x.Title, "Изменение проживающего");
            var dialog = DialogService.Show<EditUser>("", parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                UserViewModel returnModel = new UserViewModel();
                returnModel = (UserViewModel)result.Data;
                var newItem = Service.Update(returnModel);
                var index = Model.FindIndex(x => x.UserId == newItem.UserId);
                Model[index] = newItem;
                Snackbar.Add("Элемент сохранен", Severity.Success);
                StateHasChanged();
            }
            else
            {
                var oldItem = Service.ReloadItem(item);
                var index = Model.FindIndex(x => x.UserId == oldItem.UserId);
                Model[index] = oldItem;
                StateHasChanged();
            }

        }

        public async Task DeleteItemAsync(UserViewModel mCurrentItem)
        {
            try
            {
                var dialog = DialogService.Show<DeleteComponent>("Вы точно хотите удалить проживающего?");
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    var teldel = TeleUsers.FirstOrDefault(x => x.UserId == mCurrentItem.UserId);
                    if (teldel != null)
                    {
                        TelegramServ.Remove(teldel);
                        TeleUsers.Remove(teldel);
                    }

                    await AspNetUserManagment.RemoveUserInAuth(mCurrentItem.UserId);
                    var userroomdel = UserRooms.FirstOrDefault(x => x.UserId == mCurrentItem.UserId);
                    if (userroomdel != null)
                    {
                        UserRoomServ.Remove(userroomdel);
                        UserRooms.Remove(userroomdel);
                    }
                    var aspUser = AmsUsers.FirstOrDefault(x => x.UserId == mCurrentItem.UserId);
                    if (aspUser != null)
                    {
                        AmsUserService.Delete(aspUser);
                        AmsUsers.Remove(aspUser);
                    }
                    Service.Delete(mCurrentItem);
                    Model.Remove(mCurrentItem);
                    isRemove = false;
                    Snackbar.Add("Элемент удален", Severity.Success);
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }





        //public void DeleteItem(bool ans)
        //{
        //    try
        //    {
        //        if (ans == true)
        //        {

        //        }
        //        else
        //        {
        //            isRemove = false;
        //        }
        //        StateHasChanged();
        //    }
        //    catch (Exception ex)
        //    {
        //        LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now.ToString());
        //    }
        //}

        //public void ShowDialogDelete(UserViewModel item)
        //{
        //    try
        //    {
        //        mCurrentItem = item;
        //        isRemove = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now.ToString());
        //    }
        //}

        //public void Save(UserViewModel item)
        //{
        //    try
        //    {                                            
        //    }
        //    catch (Exception ex)
        //    {
        //        LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now.ToString());
        //        if (ex is DbUpdateConcurrencyException)
        //        {
        //            mEditViewModel.IsConcurency = true;
        //            mEditViewModel.ConcurencyErrorText = "Данные не актуальны! Обновите страницу";
        //        }
        //        return;
        //    }
        //}

        //public void ReloadItem(UserViewModel item)
        //{
        //    try
        //    {
        //        var reloadItem = Service.ReloadItem(item);
        //        if (reloadItem == null)
        //        {
        //            Model.Remove(item);
        //        }
        //        else
        //        {
        //            if (mEditViewModel.IsConcurency)
        //            {
        //                mEditViewModel.Item = reloadItem;
        //            }

        //            var index = Model.FindIndex(x => x.UserId == item.UserId);
        //            if (reloadItem.Item == null)
        //            {

        //                Model.RemoveAt(index);
        //            }
        //            else
        //            {
        //                mEditViewModel.IsConcurency = false;
        //                Model[index] = reloadItem;
        //            }
        //        }
        //        StateHasChanged();
        //    }
        //    catch (Exception ex)
        //    {
        //        mCurrentItem = item;
        //        LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now.ToString());
        //    }
        //}

        protected void Sort(KeyValuePair<string, string> pair)
        {
            try
            {
                if (Model != null)
                {
                    Model = pair.Value == "desc" ? Model.OrderByDescending(x => x.GetType().GetProperty(pair.Key)?.GetValue(x, null)).ToList()
                    : Model.OrderBy(x => x.GetType().GetProperty(pair.Key)?.GetValue(x, null)).ToList();
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }

    }
}
