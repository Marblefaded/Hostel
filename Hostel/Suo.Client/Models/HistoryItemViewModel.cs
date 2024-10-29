using HostelDB.Model;

namespace Suo.Client.Models;

public class HistoryItemViewModel
{
    public List<Claim> Claims { get; set; } = new List<Claim>();

    public HistoryItemViewModel(List<Claim> claims)
    {
        this.Claims = claims;
    }
}