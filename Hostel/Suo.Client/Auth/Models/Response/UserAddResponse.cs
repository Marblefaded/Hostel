namespace Suo.Client.Models.Responce;

public class UserAddResponse
{
    public UserAddResponse()
    {
        
    }

    public bool IsSuccess { get; set; }
    public string Errors { get; set; }



    public static UserAddResponse Error(string message)
    {
        UserAddResponse userRemove = new UserAddResponse();
        userRemove.IsSuccess = false;
        userRemove.Errors = message;
        return userRemove;
    }

    public static UserAddResponse Success()
    {
        UserAddResponse userRemove = new UserAddResponse();
        userRemove.IsSuccess = true;
        return userRemove;
    }
}