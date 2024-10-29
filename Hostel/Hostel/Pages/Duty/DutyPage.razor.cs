using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using MudBlazor;
using Suo.Admin.Data.EditModel;
using Suo.Admin.Data.Service;
using Suo.Admin.Data.ViewModel;
using Suo.Admin.Pages.Duty;
using Suo.Admin.Pages.Duty.EditDuty;
using Suo.Admin.Pages.Rooms.EditUserRooms;
using System.Collections.Generic;

namespace Suo.Admin.Pages.Duty
{
    public class DutyUi : ComponentBase
    {
        [Inject] protected ISnackbar Snackbar { get; set; }
        [Inject] protected IJSRuntime Js { get; set; }
        [Inject] protected DutyService DutyService { get; set; }
        [Inject] protected UserRoomService UserRoomService { get; set; }
        [Inject] protected RoomService RoomService { get; set; }
        [Inject] private LogApplicationService LogService { get; set; }
        [Inject] private DutyOrderService DutyOrderService { get; set; }
        [Inject] protected IDialogService DialogService { get; set; }



        public bool bordered = true;
        public static bool flag = false;
        public static bool flag2 = false;
        public static bool flagNeedSaveChanges = false;
        public static bool emptyflag = false;
        public bool processing = false;
        public List<int> list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };


        public List<DutyOrderViewModel> dutyOrderViewModels = new List<DutyOrderViewModel>();
        public LogApplicationViewModel LogModel = new LogApplicationViewModel();
        public EditDutyPageViewModel EditViewModel = new EditDutyPageViewModel();
        public List<DateTime?> AllDaysList = new List<DateTime?>();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                EditViewModel.AllDutys = DutyService.Get();
                EditViewModel.DutyOrder = DutyOrderService.Get();
                EditViewModel.Rooms = RoomService.Get();
                EditViewModel.UserRooms = UserRoomService.Get();

                if (EditViewModel.DutyOrder.Count > 0)
                {
                    EditViewModel.ListRommsOn3FloorLeft = EditViewModel.Rooms.Where(x => x.Floor == 3 && x.RoomTypeId == 1).ToList();
                    EditViewModel.ListRommsOn4FloorLeft = EditViewModel.Rooms.Where(x => x.Floor == 4 && x.RoomTypeId == 1 && x.Wing == "l").ToList();
                    EditViewModel.ListRommsOn4FloorRight = EditViewModel.Rooms.Where(x => x.Floor == 4 && x.RoomTypeId == 1 && x.Wing == "r").ToList();
                    EditViewModel.ListRommsOn5FloorLeft = EditViewModel.Rooms.Where(x => x.Floor == 5 && x.RoomTypeId == 1 && x.Wing == "l").ToList();
                    EditViewModel.ListRommsOn5FloorRight = EditViewModel.Rooms.Where(x => x.Floor == 5 && x.RoomTypeId == 1 && x.Wing == "r").ToList();
                }
                await RoomOrderGenerator();
                var curentmonth = DateTime.Now.Month;
                EditViewModel.InputMonth = curentmonth;
                EditViewModel.InputYear = DateTime.Now.Year;
                EditViewModel.Date = new DateTime(EditViewModel.InputYear, EditViewModel.InputMonth, 1);
                DutyLoader();
                emptyflag = false;
                await base.OnInitializedAsync();
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }

        }

        public async Task EditOrderDialog(EditDutyPageViewModel EditViewModel)
        {

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium };
            var parameters = new DialogParameters<EditDutyOrder> { { x => x.EditViewModel, EditViewModel } };
            //parameters.Add(x => x.Title, $"Комната №{EditViewModel.RoomModels.FirstOrDefault(x => x.RoomId == EditViewModel.RoomModelId).NumberRoom}");
            var dialog = DialogService.Show<EditDutyOrder>($"Порядок", parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                EditDutyPageViewModel model = new EditDutyPageViewModel();
                model = (EditDutyPageViewModel)result.Data;
                DutyOrderService.Clear();
                for (var i = 0; i < model.ListRommsOn3FloorLeft.Count; i++)
                {
                    var OrderItem = new DutyOrderViewModel();
                    OrderItem.Order = i;
                    OrderItem.RoomId = model.ListRommsOn3FloorLeft[i].RoomId;
                    DutyOrderService.Create(OrderItem);
                }
                for (var i = 0; i < model.ListRommsOn4FloorLeft.Count; i++)
                {
                    var OrderItem = new DutyOrderViewModel();
                    OrderItem.Order = i;
                    OrderItem.RoomId = model.ListRommsOn4FloorLeft[i].RoomId;
                    DutyOrderService.Create(OrderItem);
                }
                for (var i = 0; i < model.ListRommsOn4FloorRight.Count; i++)
                {
                    var OrderItem = new DutyOrderViewModel();
                    OrderItem.Order = i;
                    OrderItem.RoomId = model.ListRommsOn4FloorRight[i].RoomId;
                    DutyOrderService.Create(OrderItem);
                }
                for (var i = 0; i < model.ListRommsOn5FloorLeft.Count; i++)
                {
                    var OrderItem = new DutyOrderViewModel();
                    OrderItem.Order = i;
                    OrderItem.RoomId = model.ListRommsOn5FloorLeft[i].RoomId;
                    DutyOrderService.Create(OrderItem);
                }
                for (var i = 0; i < model.ListRommsOn5FloorRight.Count; i++)
                {
                    var OrderItem = new DutyOrderViewModel();
                    OrderItem.Order = i;
                    OrderItem.RoomId = model.ListRommsOn5FloorRight[i].RoomId;
                    DutyOrderService.Create(OrderItem);
                }
                EditViewModel.ListRommsOn3FloorLeft = model.ListRommsOn3FloorLeft;
                EditViewModel.ListRommsOn4FloorLeft = model.ListRommsOn4FloorLeft;
                EditViewModel.ListRommsOn4FloorRight = model.ListRommsOn4FloorRight;
                EditViewModel.ListRommsOn5FloorLeft = model.ListRommsOn5FloorLeft;
                EditViewModel.ListRommsOn5FloorRight = model.ListRommsOn5FloorRight;

                StateHasChanged();
            }
            else
            {

            }
        }


        public async Task RoomOrderGenerator()
        {
            if (EditViewModel.DutyOrder.Count == 0)
            {
                var IdRoomsWithLodgers = EditViewModel.UserRooms.Select(x => x.RoomId).ToList();
                IdRoomsWithLodgers = IdRoomsWithLodgers.Distinct().ToList();

                var ListRommsOn3FloorLeft = EditViewModel.Rooms.Where(x => IdRoomsWithLodgers.Contains(x.RoomId) && x.Floor == 3 && x.RoomTypeId == 1).ToList();
                var ListRommsOn4FloorLeft = EditViewModel.Rooms.Where(x => IdRoomsWithLodgers.Contains(x.RoomId) && x.Floor == 4 && x.RoomTypeId == 1 && x.Wing == "l").ToList();
                var ListRommsOn4FloorRight = EditViewModel.Rooms.Where(x => IdRoomsWithLodgers.Contains(x.RoomId) && x.Floor == 4 && x.RoomTypeId == 1 && x.Wing == "r").ToList();
                var ListRommsOn5FloorLeft = EditViewModel.Rooms.Where(x => IdRoomsWithLodgers.Contains(x.RoomId) && x.Floor == 5 && x.RoomTypeId == 1 && x.Wing == "l").ToList();
                var ListRommsOn5FloorRight = EditViewModel.Rooms.Where(x => IdRoomsWithLodgers.Contains(x.RoomId) && x.Floor == 5 && x.RoomTypeId == 1 && x.Wing == "r").ToList();


                for (var i = 0; i < ListRommsOn3FloorLeft.Count; i++)
                {
                    var OrderItem = new DutyOrderViewModel();
                    OrderItem.Order = i;
                    OrderItem.RoomId = ListRommsOn3FloorLeft[i].RoomId;
                    DutyOrderService.Create(OrderItem);
                }
                for (var i = 0; i < ListRommsOn4FloorLeft.Count; i++)
                {
                    var OrderItem = new DutyOrderViewModel();
                    OrderItem.Order = i;
                    OrderItem.RoomId = ListRommsOn4FloorLeft[i].RoomId;
                    DutyOrderService.Create(OrderItem);
                }
                for (var i = 0; i < ListRommsOn4FloorRight.Count; i++)
                {
                    var OrderItem = new DutyOrderViewModel();
                    OrderItem.Order = i;
                    OrderItem.RoomId = ListRommsOn4FloorRight[i].RoomId;
                    DutyOrderService.Create(OrderItem);
                }
                for (var i = 0; i < ListRommsOn5FloorLeft.Count; i++)
                {
                    var OrderItem = new DutyOrderViewModel();
                    OrderItem.Order = i;
                    OrderItem.RoomId = ListRommsOn5FloorLeft[i].RoomId;
                    DutyOrderService.Create(OrderItem);
                }
                for (var i = 0; i < ListRommsOn5FloorRight.Count; i++)
                {
                    var OrderItem = new DutyOrderViewModel();
                    OrderItem.Order = i;
                    OrderItem.RoomId = ListRommsOn5FloorRight[i].RoomId;
                    DutyOrderService.Create(OrderItem);
                }
            }
            else
            {
                var result = await RoomService.GetRoomsWithOrders();
                EditViewModel.DutyOrder = result;
               //var test  = await RoomService.GetRoomsWithOrders();

               EditViewModel.ListRommsOn3FloorLeft = result.Where(x => x.Room.Floor == 3 && x.Room.RoomTypeId == 1).Select(x=> new RoomViewModel(x.Room)).ToList();
               EditViewModel.ListRommsOn4FloorLeft = result.Where(x => x.Room.Floor == 4 && x.Room.RoomTypeId == 1 && x.Room.Wing == "l").Select(x => new RoomViewModel(x.Room)).ToList();
               EditViewModel.ListRommsOn4FloorRight = result.Where(x => x.Room.Floor == 4 && x.Room.RoomTypeId == 1 && x.Room.Wing == "r").Select(x => new RoomViewModel(x.Room)).ToList();
               EditViewModel.ListRommsOn5FloorLeft = result.Where(x => x.Room.Floor == 5 && x.Room.RoomTypeId == 1 && x.Room.Wing == "l").Select(x => new RoomViewModel(x.Room)).ToList();
               EditViewModel.ListRommsOn5FloorRight = result.Where(x => x.Room.Floor == 5 && x.Room.RoomTypeId == 1 && x.Room.Wing == "r").Select(x => new RoomViewModel(x.Room)).ToList();
               //return;
               // foreach (var item in EditViewModel.DutyOrder)
               // {
               //     var roomModel = RoomService.FindById(item.RoomId);
               //     switch (roomModel.Floor, roomModel.Wing)
               //     {
               //         case (3, "l"):
               //             EditViewModel.ListRommsOn3FloorLeft[item.Order] = roomModel;
               //             continue;
               //         case (4, "l"):
               //             EditViewModel.ListRommsOn4FloorLeft[item.Order] = roomModel;
               //             continue;
               //         case (4, "r"):
               //             EditViewModel.ListRommsOn4FloorRight[item.Order] = roomModel;
               //             continue;
               //         case (5, "l"):
               //             EditViewModel.ListRommsOn5FloorLeft[item.Order] = roomModel;
               //             continue;
               //         case (5, "r"):
               //             EditViewModel.ListRommsOn5FloorRight[item.Order] = roomModel;
               //             continue;
               //     }
               // }
            }
        }


        public void DutyGenerator()
        {
            EditViewModel.AllDutys = DutyService.Get();
            EditViewModel.Rooms = RoomService.Get();
            EditViewModel.UserRooms = UserRoomService.Get();

            if (EditViewModel.UserRooms.Count == 0)
            {
                Snackbar.Add("В общежитии нет жильцов, вы не можете сгенерировать расписание", Severity.Success);
                return;
            }
            if (EditViewModel.DutyOrder.Count == 0)
            {
                Snackbar.Add("Установите порядок расписания, вы не можете сгенерировать расписание", Severity.Success);
                return;
            }

            int indexforstart = 0;
            try
            {
                //foreach (var item in EditViewModel.DutyOrder)
                //{
                //    var roomModel = RoomService.FindById(item.RoomId);
                //    switch (roomModel.Floor, roomModel.Wing)
                //    {
                //        case (3, "l"):
                //            EditViewModel.ListRommsOn3FloorLeft[item.Order] = roomModel;
                //            continue;
                //        case (4, "l"):
                //            EditViewModel.ListRommsOn4FloorLeft[item.Order] = roomModel;
                //            continue;
                //        case (4, "r"):
                //            EditViewModel.ListRommsOn4FloorRight[item.Order] = roomModel;
                //            continue;
                //        case (5, "l"):
                //            EditViewModel.ListRommsOn5FloorLeft[item.Order] = roomModel;
                //            continue;
                //        case (5, "r"):
                //            EditViewModel.ListRommsOn5FloorRight[item.Order] = roomModel;
                //            continue;
                //    }
                //}


                if (EditViewModel.InputMonth == 0)
                {
                    return;
                }
                EditViewModel.Date = new DateTime(EditViewModel.InputYear, EditViewModel.InputMonth, 1);

                EditViewModel.DutysForMonts = new List<DutyViewModel>();
                EditViewModel.DutysForPreviousMonts = new List<DutyViewModel>();
                if (EditViewModel.InputMonth > 1)
                {
                    var rez = EditViewModel.AllDutys.Where(x => x.Date.Month == EditViewModel.InputMonth - 1 && x.Date.Year == EditViewModel.InputYear).ToList();
                    EditViewModel.DutysForPreviousMonts = rez;
                }

                var dayinmonth = DateTime.DaysInMonth(EditViewModel.Date.Year, EditViewModel.Date.Month);
                var lastday = EditViewModel.Date.AddDays(-1);

                foreach (var item in EditViewModel.AllDutys)
                {
                    if (item.NotCompliteDuty == "true" && item.Date.Month == EditViewModel.Date.AddMonths(-1).Month && item.Date.Year == EditViewModel.Date.AddMonths(-1).Year)
                    {
                        EditViewModel.SlackerRooms.Add(item.RoomNumber);
                    }
                }

                /*3l */
                for (int i = EditViewModel.DutysForPreviousMonts.Count - 1; i >= 0; i--)
                {
                    if (EditViewModel.DutysForPreviousMonts[i].Date == lastday && EditViewModel.DutysForPreviousMonts[i].Floor == 3 /*&& EditViewModel.DutysForPreviousMonts[i].Wing == "l"*/ && EditViewModel.ListRommsOn3FloorLeft.Any(room => room.NumberRoom == EditViewModel.DutysForPreviousMonts[i].RoomNumber) == true)
                    {
                        var start3left = EditViewModel.DutysForPreviousMonts[i].RoomNumber;
                        int indexstart3left = EditViewModel.ListRommsOn3FloorLeft.FindIndex(room => room.NumberRoom == start3left);
                        indexforstart = indexstart3left + 1;
                        break;
                    }
                    else
                    {
                        indexforstart = 0;
                    }
                }
                for (int i = indexforstart, contol = 0; contol < dayinmonth; i++, contol++)
                {
                    var ViewModel = new DutyViewModel();
                    ViewModel.NotCompliteDuty = null;
                    if (i >= EditViewModel.ListRommsOn3FloorLeft.Count)
                    {
                        i = 0;
                    }
                    ViewModel.RoomNumber = EditViewModel.ListRommsOn3FloorLeft[i].NumberRoom;

                    ViewModel.Floor = EditViewModel.ListRommsOn3FloorLeft[i].Floor;
                    ViewModel.Wing = EditViewModel.ListRommsOn3FloorLeft[i].Wing;
                    ViewModel.Date = EditViewModel.Date;
                    EditViewModel.DutysForMonts.Add(ViewModel);
                    EditViewModel.Date = EditViewModel.Date.AddDays(1);
                    if (EditViewModel.SlackerRooms.Contains(EditViewModel.ListRommsOn3FloorLeft[i].NumberRoom))
                    {
                        ViewModel = new DutyViewModel();
                        EditViewModel.SlackerRooms.Remove(EditViewModel.ListRommsOn3FloorLeft[i].NumberRoom);
                        ViewModel.NotCompliteDuty = null;
                        ViewModel.RoomNumber = EditViewModel.ListRommsOn3FloorLeft[i].NumberRoom;
                        ViewModel.Floor = EditViewModel.ListRommsOn3FloorLeft[i].Floor;
                        ViewModel.Wing = EditViewModel.ListRommsOn3FloorLeft[i].Wing;
                        ViewModel.Date = EditViewModel.Date;
                        EditViewModel.DutysForMonts.Add(ViewModel);
                        EditViewModel.Date = EditViewModel.Date.AddDays(1);
                        contol++;
                    }
                    if (i == EditViewModel.ListRommsOn3FloorLeft.Count - 1 && contol < dayinmonth)
                    {
                        i = -1;
                    }
                }

                /* EditViewModel.Date = new DateTime(EditViewModel.InputYear, EditViewModel.InputMonth, 1);*///не уверен в целесообразности
                /*3r */
                //for (int i = EditViewModel.DutysForPreviousMonts.Count - 1; i >= 0; i--)
                //{
                //    if (EditViewModel.DutysForPreviousMonts[i].Date == EditViewModel.Date.AddDays(-1) && EditViewModel.AllDutys[i].Floor == 3 && EditViewModel.AllDutys[i].Wing == "r" && ListRommsOn3FloorRight.Any(room => room.NumberRoom == EditViewModel.AllDutys[i].RoomNumber) == true)
                //    {
                //        var start3right = EditViewModel.DutysForPreviousMonts[i].RoomNumber;
                //        int indexstart3right = ListRommsOn3FloorRight.FindIndex(room => room.NumberRoom == start3right);
                //        indexforstart = indexstart3right + 1;
                //        break;
                //    }
                //    else
                //    {
                //        indexforstart = 0;
                //    }
                //}
                //for (int i = indexforstart, contol = 0; contol < dayinmonth; i++, contol++)
                //{
                //    var ViewModel = new DutyViewModel();
                //    ViewModel.NotCompliteDuty = null;
                //    ViewModel.RoomNumber = ListRommsOn3FloorRight[i].NumberRoom;
                //    ViewModel.Floor = ListRommsOn3FloorRight[i].Floor;
                //    ViewModel.Wing = ListRommsOn3FloorRight[i].Wing;
                //    ViewModel.Date = EditViewModel.Date;
                //    EditViewModel.DutysForMonts.Add(ViewModel);
                //    EditViewModel.Date = EditViewModel.Date.AddDays(1);
                //    if (EditViewModel.SlackerRooms.Contains(ListRommsOn3FloorRight[i].NumberRoom))
                //    {
                //        ViewModel = new DutyViewModel();
                //        EditViewModel.SlackerRooms.Remove(ListRommsOn3FloorRight[i].NumberRoom);
                //        ViewModel.NotCompliteDuty = null;
                //        ViewModel.RoomNumber = ListRommsOn3FloorRight[i].NumberRoom;
                //        ViewModel.Floor = ListRommsOn3FloorRight[i].Floor;
                //        ViewModel.Wing = ListRommsOn3FloorRight[i].Wing;
                //        ViewModel.Date = EditViewModel.Date;
                //        EditViewModel.DutysForMonts.Add(ViewModel);
                //        EditViewModel.Date = EditViewModel.Date.AddDays(1);
                //        contol++;
                //    }
                //    if (i == ListRommsOn3FloorRight.Count - 1 && contol < dayinmonth)
                //    {
                //        i = -1;
                //    }
                //}

                EditViewModel.Date = new DateTime(EditViewModel.InputYear, EditViewModel.InputMonth, 1);
                /*4l*/
                for (int i = EditViewModel.DutysForPreviousMonts.Count - 1; i >= 0; i--)
                {
                    if (EditViewModel.AllDutys[i].Date == EditViewModel.Date.AddDays(-1) && EditViewModel.AllDutys[i].Floor == 4 && EditViewModel.DutysForPreviousMonts[i].Wing == "l" && EditViewModel.ListRommsOn4FloorLeft.Any(room => room.NumberRoom == EditViewModel.AllDutys[i].RoomNumber) == true)
                    {
                        var start4left = EditViewModel.DutysForPreviousMonts[i].RoomNumber;
                        int indexstart4left = EditViewModel.ListRommsOn4FloorLeft.FindIndex(room => room.NumberRoom == start4left);
                        indexforstart = indexstart4left + 1;
                        break;
                    }
                    else
                    {
                        indexforstart = 0;
                    }
                }
                for (int i = indexforstart, contol = 0; contol < dayinmonth; i++, contol++)
                {
                    var ViewModel = new DutyViewModel();
                    ViewModel.NotCompliteDuty = null;

                    if (i >= EditViewModel.ListRommsOn4FloorLeft.Count)
                    {
                        i = 0;
                    }

                    ViewModel.RoomNumber = EditViewModel.ListRommsOn4FloorLeft[i].NumberRoom;
                    ViewModel.Floor = EditViewModel.ListRommsOn4FloorLeft[i].Floor;
                    ViewModel.Wing = EditViewModel.ListRommsOn4FloorLeft[i].Wing;
                    ViewModel.Date = EditViewModel.Date;
                    EditViewModel.DutysForMonts.Add(ViewModel);
                    EditViewModel.Date = EditViewModel.Date.AddDays(1);
                    if (EditViewModel.SlackerRooms.Contains(EditViewModel.ListRommsOn4FloorLeft[i].NumberRoom))
                    {
                        ViewModel = new DutyViewModel();
                        EditViewModel.SlackerRooms.Remove(EditViewModel.ListRommsOn4FloorLeft[i].NumberRoom);
                        ViewModel.NotCompliteDuty = null;
                        ViewModel.RoomNumber = EditViewModel.ListRommsOn4FloorLeft[i].NumberRoom;
                        ViewModel.Floor = EditViewModel.ListRommsOn4FloorLeft[i].Floor;
                        ViewModel.Wing = EditViewModel.ListRommsOn4FloorLeft[i].Wing;
                        ViewModel.Date = EditViewModel.Date;
                        EditViewModel.DutysForMonts.Add(ViewModel);
                        EditViewModel.Date = EditViewModel.Date.AddDays(1);
                        contol++;
                    }
                    if (i == EditViewModel.ListRommsOn4FloorLeft.Count - 1 && contol < dayinmonth)
                    {
                        i = -1;
                    }
                }

                EditViewModel.Date = new DateTime(EditViewModel.InputYear, EditViewModel.InputMonth, 1);
                /*4r*/
                for (int i = EditViewModel.DutysForPreviousMonts.Count - 1; i >= 0; i--)
                {
                    if (EditViewModel.AllDutys[i].Date == EditViewModel.Date.AddDays(-1) && EditViewModel.DutysForPreviousMonts[i].Floor == 4 && EditViewModel.AllDutys[i].Wing == "r" && EditViewModel.ListRommsOn4FloorRight.Any(room => room.NumberRoom == EditViewModel.AllDutys[i].RoomNumber) == true)
                    {
                        var start4right = EditViewModel.DutysForPreviousMonts[i].RoomNumber;
                        int indexstart4right = EditViewModel.ListRommsOn4FloorRight.FindIndex(room => room.NumberRoom == start4right);
                        indexforstart = indexstart4right + 1;
                        break;
                    }
                    else
                    {
                        indexforstart = 0;
                    }
                }
                for (int i = 0, contol = 0; contol < dayinmonth; i++, contol++)
                {
                    var ViewModel = new DutyViewModel();
                    ViewModel.NotCompliteDuty = null;

                    if (i >= EditViewModel.ListRommsOn4FloorRight.Count)
                    {
                        i = 0;
                    }

                    ViewModel.RoomNumber = EditViewModel.ListRommsOn4FloorRight[i].NumberRoom;
                    ViewModel.Floor = EditViewModel.ListRommsOn4FloorRight[i].Floor;
                    ViewModel.Wing = EditViewModel.ListRommsOn4FloorRight[i].Wing;
                    ViewModel.Date = EditViewModel.Date;
                    EditViewModel.DutysForMonts.Add(ViewModel);
                    EditViewModel.Date = EditViewModel.Date.AddDays(1);
                    if (EditViewModel.SlackerRooms.Contains(EditViewModel.ListRommsOn4FloorRight[i].NumberRoom))
                    {
                        ViewModel = new DutyViewModel();
                        EditViewModel.SlackerRooms.Remove(EditViewModel.ListRommsOn4FloorRight[i].NumberRoom);
                        ViewModel.NotCompliteDuty = null;
                        ViewModel.RoomNumber = EditViewModel.ListRommsOn4FloorRight[i].NumberRoom;
                        ViewModel.Floor = EditViewModel.ListRommsOn4FloorRight[i].Floor;
                        ViewModel.Wing = EditViewModel.ListRommsOn4FloorRight[i].Wing;
                        ViewModel.Date = EditViewModel.Date;
                        EditViewModel.DutysForMonts.Add(ViewModel);
                        EditViewModel.Date = EditViewModel.Date.AddDays(1);
                        contol++;
                    }
                    if (i == EditViewModel.ListRommsOn4FloorRight.Count - 1 && contol < DateTime.DaysInMonth(EditViewModel.Date.Year, EditViewModel.Date.Month))
                    {
                        i = -1;
                    }
                }

                EditViewModel.Date = new DateTime(EditViewModel.InputYear, EditViewModel.InputMonth, 1);
                /*5l*/
                for (int i = EditViewModel.DutysForPreviousMonts.Count - 1; i >= 0; i--)
                {
                    if (EditViewModel.AllDutys[i].Date == EditViewModel.Date.AddDays(-1) && EditViewModel.AllDutys[i].Floor == 5 && EditViewModel.DutysForPreviousMonts[i].Wing == "l" && EditViewModel.ListRommsOn5FloorLeft.Any(room => room.NumberRoom == EditViewModel.AllDutys[i].RoomNumber) == true)
                    {
                        var start5left = EditViewModel.DutysForPreviousMonts[i].RoomNumber;
                        int indexstart5left = EditViewModel.ListRommsOn5FloorLeft.FindIndex(room => room.NumberRoom == start5left);
                        indexforstart = indexstart5left + 1;
                        break;
                    }
                    else
                    {
                        indexforstart = 0;
                    }
                }
                for (int i = indexforstart, contol = 0; contol < dayinmonth; i++, contol++)
                {
                    var ViewModel = new DutyViewModel();
                    ViewModel.NotCompliteDuty = null;

                    if (i >= EditViewModel.ListRommsOn5FloorLeft.Count)
                    {
                        i = 0;
                    }

                    ViewModel.RoomNumber = EditViewModel.ListRommsOn5FloorLeft[i].NumberRoom;
                    ViewModel.RoomNumber = EditViewModel.ListRommsOn5FloorLeft[i].NumberRoom;
                    ViewModel.Floor = EditViewModel.ListRommsOn5FloorLeft[i].Floor;
                    ViewModel.Wing = EditViewModel.ListRommsOn5FloorLeft[i].Wing;
                    ViewModel.Date = EditViewModel.Date;
                    EditViewModel.DutysForMonts.Add(ViewModel);
                    EditViewModel.Date = EditViewModel.Date.AddDays(1);
                    if (EditViewModel.SlackerRooms.Contains(EditViewModel.ListRommsOn5FloorLeft[i].NumberRoom))
                    {
                        ViewModel = new DutyViewModel();
                        EditViewModel.SlackerRooms.Remove(EditViewModel.ListRommsOn5FloorLeft[i].NumberRoom);
                        ViewModel.NotCompliteDuty = null;
                        ViewModel.RoomNumber = EditViewModel.ListRommsOn5FloorLeft[i].NumberRoom;
                        ViewModel.RoomNumber = EditViewModel.ListRommsOn5FloorLeft[i].NumberRoom;
                        ViewModel.Floor = EditViewModel.ListRommsOn5FloorLeft[i].Floor;
                        ViewModel.Wing = EditViewModel.ListRommsOn5FloorLeft[i].Wing;
                        ViewModel.Date = EditViewModel.Date;
                        EditViewModel.DutysForMonts.Add(ViewModel);
                        EditViewModel.Date = EditViewModel.Date.AddDays(1);
                        contol++;
                    }
                    if (i == EditViewModel.ListRommsOn5FloorLeft.Count - 1 && contol < dayinmonth)
                    {
                        i = -1;
                    }
                }

                EditViewModel.Date = new DateTime(EditViewModel.InputYear, EditViewModel.InputMonth, 1);
                /*5r*/
                for (int i = EditViewModel.DutysForPreviousMonts.Count - 1; i >= 0; i--)
                {
                    if (EditViewModel.DutysForPreviousMonts[i].Date == EditViewModel.Date.AddDays(-1) && EditViewModel.AllDutys[i].Floor == 5 && EditViewModel.AllDutys[i].Wing == "r" && EditViewModel.ListRommsOn5FloorRight.Any(room => room.NumberRoom == EditViewModel.AllDutys[i].RoomNumber) == true)
                    {
                        var start5right = EditViewModel.AllDutys[i].RoomNumber;
                        int indexstart5right = EditViewModel.ListRommsOn5FloorRight.FindIndex(room => room.NumberRoom == start5right);
                        indexforstart = indexstart5right + 1;
                        break;
                    }
                    else
                    {
                        indexforstart = 0;
                    }
                }
                for (int i = 0, contol = 0; contol < dayinmonth; i++, contol++)
                {
                    var ViewModel = new DutyViewModel();
                    ViewModel.NotCompliteDuty = null;

                    if (i >= EditViewModel.ListRommsOn5FloorRight.Count)
                    {
                        i = 0;
                    }

                    ViewModel.RoomNumber = EditViewModel.ListRommsOn5FloorRight[i].NumberRoom;
                    ViewModel.Floor = EditViewModel.ListRommsOn5FloorRight[i].Floor;
                    ViewModel.Wing = EditViewModel.ListRommsOn5FloorRight[i].Wing;
                    ViewModel.Date = EditViewModel.Date;
                    EditViewModel.DutysForMonts.Add(ViewModel);
                    EditViewModel.Date = EditViewModel.Date.AddDays(1);

                    if (EditViewModel.SlackerRooms.Contains(EditViewModel.ListRommsOn5FloorRight[i].NumberRoom))
                    {
                        ViewModel = new DutyViewModel();
                        ViewModel.NotCompliteDuty = null;
                        ViewModel.RoomNumber = EditViewModel.ListRommsOn5FloorRight[i].NumberRoom;
                        ViewModel.Floor = EditViewModel.ListRommsOn5FloorRight[i].Floor;
                        ViewModel.Wing = EditViewModel.ListRommsOn5FloorRight[i].Wing;
                        ViewModel.Date = EditViewModel.Date;
                        EditViewModel.DutysForMonts.Add(ViewModel);
                        EditViewModel.Date = EditViewModel.Date.AddDays(1);
                        contol++;
                    }
                    if (i == EditViewModel.ListRommsOn5FloorRight.Count - 1 && contol < dayinmonth)
                    {
                        i = -1;
                    }
                }

                EditViewModel.Date = new DateTime(EditViewModel.InputYear, EditViewModel.InputMonth, 1);
                AllDaysList = CalendarGenerator();
                flag = true;
                flag2 = true;
                flagNeedSaveChanges = true;
                emptyflag = false;
                Snackbar.Add("Для внесения изменений в месяце необходимо сохранение", Severity.Warning);
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }

        }
        public void DutyLoader()
        {
            try
            {
                EditViewModel.AllDutys = DutyService.Get();
                EditViewModel.Rooms = RoomService.Get();
                EditViewModel.UserRooms = UserRoomService.Get();

                var rez = EditViewModel.AllDutys.Where(x => x.Date.Month == EditViewModel.Date.Month && x.Date.Year == EditViewModel.InputYear).ToList();
                EditViewModel.DutysForMonts = rez;
                if (rez.Count == 0)
                {
                    flag = false;
                    flag2 = false;
                    emptyflag = true;
                    return;
                }
                AllDaysList = CalendarGenerator();
                flag2 = true;
                flag = true;
                flagNeedSaveChanges = false;
                emptyflag = false;
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }

        }
        public List<DateTime?> CalendarGenerator()
        {
            try
            {
                List<DateTime?> AllDaysList = new List<DateTime?>();

                for (int i = 1; i <= DateTime.DaysInMonth(EditViewModel.InputYear, EditViewModel.InputMonth); i++)
                {
                    AllDaysList.Add(new DateTime(EditViewModel.InputYear, EditViewModel.InputMonth, i));
                }

                if (AllDaysList[0].Value.DayOfWeek.ToString() == "Monday")
                {
                }
                else if (AllDaysList[0].Value.DayOfWeek.ToString() == "Tuesday")
                {
                    AllDaysList.InsertRange(0, Enumerable.Repeat<DateTime?>(null, 1));
                }
                else if (AllDaysList[0].Value.DayOfWeek.ToString() == "Wednesday")
                {
                    AllDaysList.InsertRange(0, Enumerable.Repeat<DateTime?>(null, 2));
                }
                else if (AllDaysList[0].Value.DayOfWeek.ToString() == "Thursday")
                {
                    AllDaysList.InsertRange(0, Enumerable.Repeat<DateTime?>(null, 3));
                }
                else if (AllDaysList[0].Value.DayOfWeek.ToString() == "Friday")
                {
                    AllDaysList.InsertRange(0, Enumerable.Repeat<DateTime?>(null, 4));
                }
                else if (AllDaysList[0].Value.DayOfWeek.ToString() == "Saturday")
                {
                    AllDaysList.InsertRange(0, Enumerable.Repeat<DateTime?>(null, 5));
                }
                else if (AllDaysList[0].Value.DayOfWeek.ToString() == "Sunday")
                {
                    AllDaysList.InsertRange(0, Enumerable.Repeat<DateTime?>(null, 6));
                }


                if (AllDaysList[^1].Value.DayOfWeek.ToString() == "Monday")
                {
                    AllDaysList.AddRange(Enumerable.Repeat<DateTime?>(null, 6));
                }
                else if (AllDaysList[^1].Value.DayOfWeek.ToString() == "Tuesday")
                {
                    AllDaysList.AddRange(Enumerable.Repeat<DateTime?>(null, 5));
                }
                else if (AllDaysList[^1].Value.DayOfWeek.ToString() == "Wednesday")
                {
                    AllDaysList.AddRange(Enumerable.Repeat<DateTime?>(null, 4));
                }
                else if (AllDaysList[^1].Value.DayOfWeek.ToString() == "Thursday")
                {
                    AllDaysList.AddRange(Enumerable.Repeat<DateTime?>(null, 3));
                }
                else if (AllDaysList[^1].Value.DayOfWeek.ToString() == "Friday")
                {
                    AllDaysList.AddRange(Enumerable.Repeat<DateTime?>(null, 2));
                }
                else if (AllDaysList[^1].Value.DayOfWeek.ToString() == "Saturday")
                {
                    AllDaysList.AddRange(Enumerable.Repeat<DateTime?>(null, 1));
                }
                else if (AllDaysList[^1].Value.DayOfWeek.ToString() == "Sunday")
                {
                    return AllDaysList;
                }
                return AllDaysList;
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
                return null;
            }

        }
        public DutyViewModel SelectDuty(DateTime? item, int floor, string wing)
        {
            try
            {
                var rez = EditViewModel.DutysForMonts.Where(x => x.Date == item && x.Floor == floor && x.Wing == wing).ToList();
                return rez[0];
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
                return null;
            }

        }
        public DutyViewModel SelectDutyFor3floor(DateTime? item, int floor)
        {
            try
            {
                var rez = EditViewModel.DutysForMonts.Where(x => x.Date == item && x.Floor == floor).ToList();
                return rez[0];
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
                return null;
            }

        }
        public async Task OnSelected(int selection)
        {
            try
            {
                EditViewModel.InputMonth = selection;
                //EditViewModel.InputYear = selection;
                EditViewModel.Date = new DateTime(EditViewModel.InputYear, EditViewModel.InputMonth, 1);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }

        }
        public async Task SaveMonth()
        {
            processing = true;
            await Task.Delay(2000);
            processing = false;
            try
            {
                var rez = EditViewModel.AllDutys.Where(x => x.Date.Month == EditViewModel.DutysForMonts[0].Date.Month && x.Date.Year == EditViewModel.DutysForMonts[0].Date.Year).ToList();

                if (rez.Count > 0)
                {
                    foreach (var item in EditViewModel.DutysForMonts)
                    {
                        var x = rez.FirstOrDefault(x => x.Date == item.Date && x.Floor == item.Floor && x.Wing == item.Wing && x.RoomNumber != item.RoomNumber);
                        if (x != null)
                        {
                            x.RoomNumber = item.RoomNumber;
                            DutyService.Update(x);
                        }
                    }
                }

                else
                {
                    foreach (var item in EditViewModel.DutysForMonts)
                    {
                        DutyService.Create(item);
                    }
                }
            }

            //catch (DbUpdateException ex) when (ex is DbUpdateException)
            //{
            //    foreach (var item in EditViewModel.DutysForMonts)
            //    {
            //        DutyService.Update(item);
            //    }
            //}
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
            if (EditViewModel.DutysForMonts.Count > 0)
            {
                Snackbar.Add("Расписание сохранено", Severity.Success);
            }
            EditViewModel.AllDutys = DutyService.Get();
            flagNeedSaveChanges = false;
            StateHasChanged();
        }

        public void Change(DutyViewModel item)
        {
            try
            {
                if (item.NotCompliteDuty == "true")
                {
                    item.NotCompliteDuty = null;
                }
                else item.NotCompliteDuty ??= "true";
                DutyService.Update(item);
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }

        }

        public void Print()
        {
            try
            {
                Js.InvokeVoidAsync("PrintSCR");
            }
            catch (Exception ex)
            {
                LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            }
        }

    }
}

