using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suo.Client.Models.Responce;
public class UserUpdateResponse
{
    public UserUpdateResponse()
    {

    }

    public bool IsSuccess { get; set; }
    public string Errors { get; set; }



    public static UserUpdateResponse Error(string message)
    {
        UserUpdateResponse userRemove = new UserUpdateResponse();
        userRemove.IsSuccess = false;
        userRemove.Errors = message;
        return userRemove;
    }

    public static UserUpdateResponse Success()
    {
        UserUpdateResponse userRemove = new UserUpdateResponse();
        userRemove.IsSuccess = true;
        return userRemove;
    }
}
