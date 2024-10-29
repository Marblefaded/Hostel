namespace Suo.Autorization.SingleService.Infrastructure.Auth;

public interface ICurrentUserService
{
    string UserId { get; }
    public List<KeyValuePair<string, string>> Claims { get; set; }

}