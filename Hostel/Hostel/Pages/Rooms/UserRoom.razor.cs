using Microsoft.AspNetCore.Components;
using MudBlazor;
using Suo.Admin.Data.EditModel;
using Suo.Admin.Data.Service;
using Suo.Admin.Data.ViewModel;
using Suo.Admin.Pages.Rooms.EditUserRooms;

namespace Suo.Admin.Pages.Rooms
{
    public class UserRoomUi : ComponentBase
    {
        public bool bordered = true;
        private string filterVar = "";
        private string filterVarforempoers = "";
        private int floorVar = -1;
        private int popVar = -1;
        public string FilterVar
        {
            get => filterVar;

            set
            {
                filterVar = value;
                Filter();
            }
        }
        public int FloorVar
        {
            get => floorVar;
            set
            {
                floorVar = value;
                Filter();
            }
        }
        public int PopVar
        {
            get => popVar;
            set
            {
                popVar = value;
                Filter();
            }
        }
        [Inject] protected UserRoomService Service { get; set; }
        [Inject] protected RoomService RoomService { get; set; }
        [Inject] protected DutyOrderService DutyOrderService { get; set; }
        [Inject] protected UserService UserService { get; set; }
        [Inject] private LogApplicationService LogService { get; set; }
        [Inject] protected IDialogService DialogService { get; set; }


        public LogApplicationViewModel LogModel = new LogApplicationViewModel();
        protected List<UserRoomViewModel> RoomUserModels { get; set; }
        protected List<UserViewModel> UserModels { get; set; }
        protected List<RoomViewModel> RoomModels { get; set; }
       
        public EditUserRoomItemVieweModel EditViewModel { get; set; } = new EditUserRoomItemVieweModel();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                RoomUserModels = Service.Get();
                UserModels = UserService.Get();
                var rez = Service.Filtering("", -1, -1);
                RoomModels = rez.Select(RoomService.Convert).ToList();
                await base.OnInitializedAsync();
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }

        public async Task AddEmploersDialog(int roomnumber)
        {
            RoomUserModels = Service.Get();
            UserModels = UserService.Get();
            EditViewModel = new EditUserRoomItemVieweModel();
            EditViewModel.UnsettledUserModels = Service.FilteringEmploers(filterVarforempoers);
            EditViewModel.RoomModelId = roomnumber;
            EditViewModel.RoomModels = RoomModels;
            EditViewModel.UserModels = UserModels;
            EditViewModel.UserRoomModels = RoomUserModels;

            var room = EditViewModel.RoomModels.Where(x => x.RoomId == EditViewModel.RoomModelId).FirstOrDefault();
            var counter = EditViewModel.UserRoomModels.Where(x => x.RoomId == EditViewModel.RoomModelId).Count();
            //if (counter >= room.PeopleMax)
            //{
            //    EditViewModel.maxPeople = true;
            //}
            //else
            //{
            //    EditViewModel.maxPeople = false;
            //}

                var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
            var parameters = new DialogParameters<EditUserRoom> { { x => x.ViewModel, EditViewModel } };
            parameters.Add(x => x.Title, $"Комната №{EditViewModel.RoomModels.FirstOrDefault(x => x.RoomId == EditViewModel.RoomModelId).NumberRoom}");
            var dialog = DialogService.Show<EditUserRoom>($"Комната", parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                EditUserRoomItemVieweModel model = new EditUserRoomItemVieweModel();
                model = (EditUserRoomItemVieweModel)result.Data;
                var newmodels = model.UserRoomModels;
                var oldadata = Service.Get();
                var differencetodelite = oldadata.Except(newmodels, new UserRoomViewModelComparer()).ToList();
                var differencetoadd = newmodels.Except(oldadata, new UserRoomViewModelComparer()).ToList();

                foreach (var item in differencetodelite)
                {
                    if (item != null)
                    {
                        Service.Remove(item);
                    }
                }
                foreach (var item in differencetoadd)
                {
                    Service.Create(item);
                }
                StateHasChanged();
            }
            else
            {
                RoomUserModels = Service.Get();
                UserModels = UserService.Get();
                var rez = Service.Filtering("", -1, -1);
                RoomModels = rez.Select(RoomService.Convert).ToList();
            }
        }

        protected void Filter()
        {
            try
            {
                var rez = Service.Filtering(FilterVar, FloorVar, PopVar);
                RoomModels = rez.Select(RoomService.Convert).ToList();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }

        public void Change(RoomViewModel item)
        {
            try
            {
                if (item.RoomTypeId == 1)
                {
                    item.RoomTypeId = 2;
                    DutyOrderService.RemoveWithOrder(item);
                }
                else
                {
                    item.RoomTypeId = 1;
                    DutyOrderService.UpdateWithOrder(item);
                }
                RoomService.Update(item);
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }

        }

        protected void Sort(KeyValuePair<string, string> pair)
        {
            if (RoomModels != null)
            {
                RoomModels = pair.Value == "desc" ? RoomModels.OrderByDescending(x => x.GetType().GetProperty(pair.Key)?.GetValue(x, null)).ToList()
                : RoomModels.OrderBy(x => x.GetType().GetProperty(pair.Key)?.GetValue(x, null)).ToList();
                StateHasChanged();
            }
        }


    }
}
