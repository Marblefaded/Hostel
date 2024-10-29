using HostelDB.Model;

namespace Suo.Client.Models;

public class HomeItemViewModel
{
    public List<Post> Posts { get; set; }
    public List<ClaimTemplate> Claims { get; set; }
    public Dictionary<int,bool> IsEnableTimeToGo { get; set; }
    public User User { get; set; }
    public DateTime? DateDuty { get; set; }
    public Dictionary<int, bool> IsEnableTimeToEnd { get; set; }
    public Dictionary<int, bool> IsEnableTimeToStart { get; set; }
    public Dictionary<int, bool> IsEnableReason { get; set; }
}