
using Microsoft.AspNetCore.Mvc;

namespace GargamelinBurnu.Components;

public class ShowTimeViewComponent : ViewComponent
{
    public string Invoke(DateTime time)
    {
        string result = Zaman(time);
        return result;
    }
    
    string Zaman(DateTime time)
    {
        var timeDifference = DateTime.Now - time;
        double totalMinutes = timeDifference.TotalMinutes;
        
        if (totalMinutes < 60)
        {
            return $"{Math.Floor(totalMinutes)} dk önce";
        }
        else if(totalMinutes > 60 && time.Date == DateTime.Now.Date)
        {
            return $@"Bugün {time.ToString("HH:mm")}";
        }
        else if(totalMinutes > 60 && time.Date == DateTime.Now.Date.AddDays(-1))
        {
            return $@"Dün {time.ToString("HH:mm")}";
        }
        else
        {
            return $@"{time.ToString("dd.MM.yyyy HH:mm")}";
        }
    }
}