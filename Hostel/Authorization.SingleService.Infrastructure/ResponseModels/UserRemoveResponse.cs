namespace Suo.Autorization.SingleService.Infrastructure.ResponseModels;

public class UserRemoveResponse
{
    public bool IsSuccess { get; set; }
    public string Errors { get; set; }


    public static UserRemoveResponse Error(string message)
    {
        UserRemoveResponse userRemove = new UserRemoveResponse();
        userRemove.IsSuccess = false;
        userRemove.Errors = message;
        return userRemove;
    }

    public static UserRemoveResponse Success()
    {
        UserRemoveResponse userRemove = new UserRemoveResponse();
        userRemove.IsSuccess = true;
        return userRemove;
    }
}