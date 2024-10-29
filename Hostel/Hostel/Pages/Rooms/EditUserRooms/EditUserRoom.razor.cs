using Microsoft.AspNetCore.Components;
using MudBlazor;
using Suo.Admin.Data.EditModel;
using Suo.Admin.Data.Service;
using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Pages.Rooms

{
    public class EditUserRoomView : ComponentBase
    {

        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        private string filterVar = "";
        public string FilterVar
        {
            get => filterVar;
            set
            {
                filterVar = value;
                Filter();
            }
        }

        public LogApplicationViewModel LogModel = new LogApplicationViewModel();
        [Inject] protected UserRoomService Service { get; set; }
        [Inject] private LogApplicationService LogService { get; set; }

        [Parameter]
        public EditUserRoomItemVieweModel ViewModel { get; set; }
        [Parameter]
        public string Title { get; set; }

        public void Cancel()
        {
            MudDialog.Cancel();
        }

        public void Save()
        {
            MudDialog.Close(DialogResult.Ok(ViewModel));
        }

        protected void Filter()
        {
            try
            {
                ViewModel.UnsettledUserModels = Service.FilteringEmploers(FilterVar);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }

        public void AddItem(UserViewModel user)
        {
            try
            {
                ViewModel.UserRoomModel = new UserRoomViewModel();
                ViewModel.UserRoomModel.RoomId = ViewModel.RoomModelId;
                ViewModel.UserRoomModel.UserId = user.UserId;

                var item = ViewModel.UserRoomModel;
                ViewModel.UserRoomModels.Add(item);
                ViewModel.UnsettledUserModels.Remove(user);

                var room = ViewModel.RoomModels.Where(x => x.RoomId == ViewModel.RoomModelId).FirstOrDefault();
                var counter = ViewModel.UserRoomModels.Where(x => x.RoomId == ViewModel.RoomModelId).Count();
                //if(counter >= room.PeopleMax)
                //{
                //    ViewModel.maxPeople = true;
                //}

            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }

        public void RemoveItem(UserRoomViewModel item)
        {
            try
            {
                var index = ViewModel.UserRoomModels.FindIndex(x => x.UserId == item.UserId);
                var id = item.UserId;
                var user = ViewModel.UserModels.FirstOrDefault(x => x.UserId == id);
                if (user != null)
                {
                    ViewModel.UnsettledUserModels.Insert(0, user);
                }
                ViewModel.UserRoomModels.RemoveAt(index);

                var room = ViewModel.RoomModels.Where(x => x.RoomId == ViewModel.RoomModelId).FirstOrDefault();
                var counter = ViewModel.UserRoomModels.Where(x => x.RoomId == ViewModel.RoomModelId).Count();
                //if (counter < room.PeopleMax)
                //{
                //    ViewModel.maxPeople = false;
                //}
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }

    }
}
