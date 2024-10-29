using HostelDB.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using NPOI.SS.Formula.Functions;
using Suo.Admin.Data.Service;
using Suo.Admin.Data.ViewModel;
using System.Net.NetworkInformation;

namespace Suo.Admin.Pages.Duty2;

public class Duty2ViewModel : ComponentBase
{
    private int _floursSelected;
    private int _selectedMount;
    private int _selectedYear;
    public CalendarModel PdfCalendarModel { get; set; }

    public Dictionary<int, string> Flouts { get; set; } = new Dictionary<int, string>()
    {
        {1,"3 этаж"},
        {2,"4 этаж левое крыло"},
        {3,"4 этаж правое крыло"},
        {4,"5 этаж левое крыло"},
        {5,"5 этаж правое крыло"},
    };

    public Dictionary<int, string> Mounts { get; set; } = new Dictionary<int, string>()
    {
        { 1, "Январь" },
        { 2, "Февраль" },
        { 3, "Март" },
        { 4, "Апрель" },
        { 5, "Май" },
        { 6, "Июнь" },
        { 7, "Июль" },
        { 8, "Август" },
        { 9, "Сентябрь" },
        { 10, "Октябрь" },
        { 11, "Ноябрь" },
        { 12, "Декабрь" },
    };

    public List<int> Years { get; set; } = new List<int>();
    public bool processingSaving { get; set; }
    public int SelectedYear
    {
        get => _selectedYear;
        set
        {
            _selectedYear = value;
            LoadRooms();

        }
    }

    public int SelectedMount
    {
        get => _selectedMount;
        set
        {
            _selectedMount = value;
            LoadRooms();
        }
    }

    public int FloursSelected
    {
        get => _floursSelected;
        set
        {
            _floursSelected = value;
            LoadRooms();
        }
    }

    public bool IsPrinting { get; set; } = false;
    public CalendarModel CalendarModel { get; set; }
    [Inject] protected RoomService roomService { get; set; }
    [Inject] protected DutyService dutyService { get; set; }
    [Inject] protected ISnackbar Snackbar { get; set; }

    [Inject] protected IJSRuntime js { get; set; }
    public List<RoomViewModel> Rooms { get; set; } = new List<RoomViewModel>();
    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var nowYear = DateTime.Now.Year;
            SelectedYear = DateTime.Now.Year;
            SelectedMount = DateTime.Now.Month;
            for (int i = 0; i < 3; i++)
            {
                Years.Add(nowYear);
                nowYear++;
            }

            CalendarModel = new CalendarModel(new DateTime(SelectedYear, SelectedMount, 1));
            StateHasChanged();
        }
    }


    public async Task LoadRooms()
    {
        if (FloursSelected == 0)
        {
            return;
        }
        CalendarModel = null;
        StateHasChanged();
        await Task.Delay(1);
        int floour = 0;
        string wing = "";
        switch (FloursSelected)
        {
            case 1:
                {
                    floour = 3;
                    break;
                }
            case 2:
                {
                    floour = 4;
                    wing = "l";
                    break;
                }
            case 3:
                {
                    floour = 4;
                    wing = "r";

                    break;
                }
            case 4:
                {
                    floour = 5;
                    wing = "l";

                    break;
                }
            case 5:
                {
                    floour = 5;
                    wing = "r";
                    break;
                }
        }

        Rooms = await roomService.GetRoomsFiltered(floour, 1, wing);

        if (await dutyService.IsAnyDutys(floour, SelectedMount,SelectedYear, wing))
        {
            var dutys = dutyService.GetForDateFilter(floour, SelectedMount, SelectedYear, wing);
            var dutysDict = dutyService.ConvertDutyToDayDutyDictionary(dutys);

            if (DateTime.DaysInMonth(SelectedYear, SelectedMount) != dutysDict.Count)
            {
                var daysCount = DateTime.DaysInMonth(SelectedYear, SelectedMount);
                for (int i = 1; i < daysCount; i++)
                {
                    dutysDict.TryAdd(i, null);
                }
               
            }

            CalendarModel = new CalendarModel(new DateTime(SelectedYear, SelectedMount, 1), dutysDict);

        }
        else
        {
            CalendarModel = new CalendarModel(new DateTime(SelectedYear, SelectedMount, 1));

        }




        StateHasChanged();
        await Task.Delay(1);
        try
        {
            await js.InvokeVoidAsync("StartDraggbleItems");

        }
        catch (Exception e)
        {
            Console.WriteLine(e);

        }
    }
    public async Task RemoveItemInDisplay(CalendarItem calendarItem)
    {
        await js.InvokeVoidAsync("RemoveItemInDisplay", $"tdday{calendarItem.Date.GetValueOrDefault().Day}");
    }


    public async Task<int> GetRoomIdInDutyTable(int day)
    {
        var value = await js.InvokeAsync<string>("GetRoomIdInDutyTable", $"tdday{day}");

        return int.Parse(value);
    }

    public async Task<string> GetRoomNumberInDutyTable(int day)
    {
        var value = await js.InvokeAsync<string>("GetRoomNumberInDutyTable", $"tdday{day}");

        return value;
    }

    
    public async Task SaveDutys()
    {
        if (FloursSelected == 0)
        {
            return;
        }
        processingSaving = true;
        StateHasChanged();
        var daysCount = DateTime.DaysInMonth(SelectedYear, SelectedMount);
        


        int floour = 0;
        string wing = "";
        switch (FloursSelected)
        {
            case 1:
            {
                floour = 3;
                break;
            }
            case 2:
            {
                floour = 4;
                wing = "l";
                break;
            }
            case 3:
            {
                floour = 4;
                wing = "r";

                break;
            }
            case 4:
            {
                floour = 5;
                wing = "l";

                break;
            }
            case 5:
            {
                floour = 5;
                wing = "r";
                break;
            }
        }




        List<HostelDB.Model.Duty> dutys = new List<HostelDB.Model.Duty>();

        await dutyService.ClearOldIfExist(SelectedYear, SelectedMount, floour, wing);

        for (int i = 1; i <= daysCount; i++)
        {
            HostelDB.Model.Duty model = new ();

            model.Date = new DateTime(SelectedYear, SelectedMount, i);

            model.Floor = floour;
            model.Wing = wing;
            model.RoomNumber = await GetRoomNumberInDutyTable(i);

            //model.RoomId = roomid;
            //model.RoomNumber = roomService.FindById(roomid).NumberRoom;
            dutys.Add(model);
        }

        await dutyService.Create(dutys);



        processingSaving = false;
        StateHasChanged();
        Snackbar.Add("Расписание сохранено", Severity.Success);
    }
    public async Task PrintPDF()
    {
        int floour = 0;
        string wing = "";
        switch (FloursSelected)
        {
            case 1:
            {
                floour = 3;
                break;
            }
            case 2:
            {
                floour = 4;
                wing = "l";
                break;
            }
            case 3:
            {
                floour = 4;
                wing = "r";

                break;
            }
            case 4:
            {
                floour = 5;
                wing = "l";

                break;
            }
            case 5:
            {
                floour = 5;
                wing = "r";
                break;
            }
        }

        var dutys = dutyService.GetForDateFilter(floour, SelectedMount, SelectedYear, wing);
        var dutysDict = dutyService.ConvertDutyToDayDutyDictionary(dutys);

        if (DateTime.DaysInMonth(SelectedYear, SelectedMount) != dutysDict.Count)
        {
            var daysCount = DateTime.DaysInMonth(SelectedYear, SelectedMount);
            for (int i = 1; i < daysCount; i++)
            {
                dutysDict.TryAdd(i, null);
            }

        }

        PdfCalendarModel = new CalendarModel(new DateTime(SelectedYear, SelectedMount, 1), dutysDict);

        IsPrinting = true;
        StateHasChanged();
        await Task.Delay(1);

        await js.InvokeVoidAsync("savePDF", $"{DateTime.Now.ToString("dd.MM.yyyy")} {Mounts[SelectedMount]} {Flouts[FloursSelected]}");

        IsPrinting = false;
        StateHasChanged();
        await Task.Delay(1);

    }

}