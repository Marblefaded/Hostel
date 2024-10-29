using Suo.Autorization.Data.Models;

namespace Suo.Autorization.SingleService.Infrastructure.ResponseModels;

public class UserAddResponse
{
    public UserResponse User { get; set; }
    public UserAddResponse()
    {
        
    }
    public UserAddResponse(AmsUser user)
    {
        User = new UserResponse(user);
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

    public static UserAddResponse Success(AmsUser superUser)
    {
        UserAddResponse userRemove = new UserAddResponse(superUser);
        userRemove.IsSuccess = true;
        return userRemove;
    }
    public static UserAddResponse Success()
    {
        UserAddResponse userRemove = new UserAddResponse();
        userRemove.IsSuccess = true;
        return userRemove;
    }
}