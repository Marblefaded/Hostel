namespace Suo.Client.Models.Responce;

public class ChangePasswordResponse
{
    public ChangePasswordResponse()
    {
        
    }
    public ChangePasswordResponse(bool isSuccsesd, string displayMessage)
    {
        IsSuccsesd = isSuccsesd;
        DisplayMessage = displayMessage;
    }
    public bool IsSuccsesd { get; set; }

    public string DisplayMessage { get; set; }


    public static ChangePasswordResponse Succses()
    {
        
        return new ChangePasswordResponse(true, "Password changed");
    }

    public static ChangePasswordResponse Error(string displayMessage)
    {
        return new ChangePasswordResponse(true, displayMessage);

    }
}