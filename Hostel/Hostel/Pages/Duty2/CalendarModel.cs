using MudBlazor.Extensions;
using Suo.Admin.Data.ViewModel;

namespace Suo.Admin.Pages.Duty2;

public class CalendarModel
{
    public CalendarModel(DateTime date)
    {
        Calendar = new List<List<CalendarItem>>();
        var dayofweek = GetDayOfWeek(date);

        //FIRST WEEK
        Calendar.Add(new List<CalendarItem>());


        for (int i = 1; i < dayofweek; i++)
        {
            Calendar.FirstOrDefault().Add(new CalendarItem());
        }
        var iteratorDate = date;

        for (int i = dayofweek; i < 8; i++)
        {
            Calendar.FirstOrDefault().Add(new CalendarItem(iteratorDate));
            iteratorDate = iteratorDate.AddDays(1);
        }

        var daysCount = DateTime.DaysInMonth(iteratorDate.Year, iteratorDate.Month);

        //for (int i = iteratorDate.Day; i <= daysCount; i++)
        //{
        //Add Next WEEK
        Calendar.Add(new List<CalendarItem>());

        while (iteratorDate.Day < daysCount)
        {

            Calendar.LastOrDefault().Add(new CalendarItem(iteratorDate));

            if (GetDayOfWeek(iteratorDate) == 7)
            {
                Calendar.Add(new List<CalendarItem>());
            }

            iteratorDate = iteratorDate.AddDays(1);
        }
        //}
        //Add last Day
        Calendar.LastOrDefault().Add(new CalendarItem(iteratorDate));


        for (int i = GetDayOfWeek(iteratorDate); i < 7; i++)
        {
            Calendar.LastOrDefault().Add(new CalendarItem());
            iteratorDate = iteratorDate.AddDays(1);
        }
    }

    public CalendarModel(DateTime date, Dictionary<int, DutyViewModel> dutys)
    {
        Calendar = new List<List<CalendarItem>>();

        var dayofweek = GetDayOfWeek(date);

        //FIRST WEEK
        Calendar.Add(new List<CalendarItem>());


        for (int i = 1; i < dayofweek; i++)
        {
            Calendar.FirstOrDefault().Add(new CalendarItem());
        }
        var iteratorDate = date;

        for (int i = dayofweek; i < 8; i++)
        {
            Calendar.FirstOrDefault().Add(new CalendarItem(iteratorDate, dutys[iteratorDate.Day]));
            iteratorDate = iteratorDate.AddDays(1);
        }

        var daysCount = DateTime.DaysInMonth(iteratorDate.Year, iteratorDate.Month);

        //for (int i = iteratorDate.Day; i <= daysCount; i++)
        //{
        //Add Next WEEK
        Calendar.Add(new List<CalendarItem>());

        while (iteratorDate.Day < daysCount)
        {

            Calendar.LastOrDefault().Add(new CalendarItem(iteratorDate, dutys[iteratorDate.Day]));

            if (GetDayOfWeek(iteratorDate) == 7)
            {
                Calendar.Add(new List<CalendarItem>());
            }

            iteratorDate = iteratorDate.AddDays(1);
        }
        //}
        //Add last Day
        Calendar.LastOrDefault().Add(new CalendarItem(iteratorDate, dutys[iteratorDate.Day]));


        for (int i = GetDayOfWeek(iteratorDate); i < 7; i++)
        {
            Calendar.LastOrDefault().Add(new CalendarItem());
            iteratorDate = iteratorDate.AddDays(1);
        }
    }

    public List<List<CalendarItem>> Calendar { get; set; }



    private int GetDayOfWeek(DateTime date)
    {
        return ((int)date.DayOfWeek == 0) ? 7 : (int)date.DayOfWeek;
    }
}

public class CalendarItem
{
    public CalendarItem(DateTime? date = null, DutyViewModel item = null)
    {
        Date = date;
        DutyViewModel = item;
    }

    public DateTime? Date { get; set; }

    //public string Name { get; set; }
    public DutyViewModel DutyViewModel { get; set; }
}