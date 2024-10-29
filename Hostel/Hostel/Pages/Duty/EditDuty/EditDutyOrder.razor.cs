using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using Suo.Admin.Data.EditModel;
using Suo.Admin.Data.Service;
using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Pages.Duty
{
    public class EditDutyOrderView : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        public LogApplicationViewModel LogModel = new LogApplicationViewModel();
        [Inject] protected DutyOrderService Service { get; set; }
        [Inject] private LogApplicationService LogService { get; set; }

        [Parameter]
        public EditDutyPageViewModel EditViewModel { get; set; }
        public List<DropItem> _dropzoneItems = new();
        public List<DropItem> _serverData = new();

        public string FloorWing
        {
            get => _floorWing;
            set
            {
                if (value!= _floorWing)
                {
                    Task.Run(async () =>
                    {
                        await SwitchFloor();

                    });

                }

                _floorWing = value;
            }
        }

        public MudDropContainer<DropItem> _container;
        private string _floorWing;


        public string Filter { get; set; }

        protected override async Task OnInitializedAsync()
        {
            FloorWing = "3";
            var i = 0;
            foreach (var item in EditViewModel.ListRommsOn3FloorLeft)
            {
                var rez = new DropItem() { Order = i, RoomViewModel = item, Selector = "1" };
                _serverData.Add(rez);
                i++;
            }
            await LoadServerData();
        }

        public async Task SwitchFloor()
        {
            _serverData = new();
            switch (FloorWing)
            {
                case "3":
                    var i = 0;
                    foreach (var item in EditViewModel.ListRommsOn3FloorLeft)
                    {
                        var rez = new DropItem() { Order = i, RoomViewModel = item, Selector = "1" };
                        _serverData.Add(rez);
                        i++;
                    }
                    await LoadServerData();
                    break;
                case "4l":
                    i = 0;
                    foreach (var item in EditViewModel.ListRommsOn4FloorLeft)
                    {
                        var rez = new DropItem() { Order = i, RoomViewModel = item, Selector = "1" };
                        _serverData.Add(rez);
                        i++;
                    }
                    await LoadServerData();
                    break;
                case "4r":
                    i = 0;
                    foreach (var item in EditViewModel.ListRommsOn4FloorRight)
                    {
                        var rez = new DropItem() { Order = i, RoomViewModel = item, Selector = "1" };
                        _serverData.Add(rez);
                        i++;
                    }
                    await LoadServerData();
                    break;
                case "5l":
                    i = 0;
                    foreach (var item in EditViewModel.ListRommsOn5FloorLeft)
                    {
                        var rez = new DropItem() { Order = i, RoomViewModel = item, Selector = "1" };
                        _serverData.Add(rez);
                        i++;
                    }
                    await LoadServerData();
                    break;
                case "5r":
                    i = 0;
                    foreach (var item in EditViewModel.ListRommsOn5FloorRight)
                    {
                        var rez = new DropItem() { Order = i, RoomViewModel = item, Selector = "1" };
                        _serverData.Add(rez);
                        i++;
                    }
                    await LoadServerData();
                    break;

            }
            await RefreshContainer();
        
            

            await InvokeAsync(StateHasChanged);
            await Task.CompletedTask;
        }

        public void Cancel()
        {
            MudDialog.Cancel();
        }

        public void Save()
        {
            SaveData();
            switch (FloorWing)
            {
                case "3":
                    EditViewModel.ListRommsOn3FloorLeft = new();
                    foreach (var item in _serverData)
                    {
                        EditViewModel.ListRommsOn3FloorLeft.Add(item.RoomViewModel);
                    }
                    MudDialog.Close(DialogResult.Ok(EditViewModel));
                    break;
                case "4l":
                    EditViewModel.ListRommsOn4FloorLeft = new();
                    foreach (var item in _serverData)
                    {
                        EditViewModel.ListRommsOn4FloorLeft.Add(item.RoomViewModel);
                    }
                    MudDialog.Close(DialogResult.Ok(EditViewModel));
                    break;
                case "4r":
                    EditViewModel.ListRommsOn4FloorRight = new();
                    foreach (var item in _serverData)
                    {
                        EditViewModel.ListRommsOn4FloorRight.Add(item.RoomViewModel);
                    }
                    MudDialog.Close(DialogResult.Ok(EditViewModel));
                    break;
                case "5l":
                    EditViewModel.ListRommsOn5FloorLeft = new();
                    foreach (var item in _serverData)
                    {
                        EditViewModel.ListRommsOn5FloorLeft.Add(item.RoomViewModel);
                    }
                    MudDialog.Close(DialogResult.Ok(EditViewModel));
                    break;
                case "5r":
                    EditViewModel.ListRommsOn5FloorRight = new();
                    foreach (var item in _serverData)
                    {
                        EditViewModel.ListRommsOn5FloorRight.Add(item.RoomViewModel);
                    }
                    MudDialog.Close(DialogResult.Ok(EditViewModel));
                    break;
            }

        }


        public void ItemUpdated(MudItemDropInfo<DropItem> dropItem)
        {
            dropItem.Item.Selector = dropItem.DropzoneIdentifier;

            var indexOffset = dropItem.DropzoneIdentifier switch
            {
                "2" => _dropzoneItems.Count(x => x.Selector == "1"),
                _ => 0
            };

            _dropzoneItems.UpdateOrder(dropItem, item => item.Order, indexOffset);
        }


        private async Task RefreshContainer()
        {
            //update the binding to the container

            await InvokeAsync(StateHasChanged);
            await Task.Delay(1);
            //the container refreshes the internal state
            _container?.Refresh();
            
            await Task.CompletedTask;
        }

        private async Task LoadServerData()
        {
            _dropzoneItems = _serverData
                .OrderBy(x => x.Order)
                .Select(item => new DropItem
                {
                    Order = item.Order,
                    RoomViewModel = item.RoomViewModel,
                    Selector = item.Selector
                })
                .ToList();
            await RefreshContainer();

            //  RefreshContainer();
        }

        private void SaveData()
            => _serverData = _dropzoneItems
                .OrderBy(x => x.Order)
                .Select(item => new DropItem
                {
                    Order = item.Order,
                    RoomViewModel = item.RoomViewModel,
                    Selector = item.Selector
                })
                .ToList();


        public class DropItem
        {
            public RoomViewModel RoomViewModel { get; init; }
            public string Selector { get; set; }
            public int Order { get; set; }
        }

    }
}
