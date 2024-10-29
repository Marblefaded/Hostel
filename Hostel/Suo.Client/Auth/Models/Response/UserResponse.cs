namespace Suo.Client.Data.Models.Responce;

public class UserResponse
{
    public UserResponse(AmsUser user)
    {
        Id = user.Id;
        UserName = user.UserName;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Email = user.Email;
        IsActive = user.IsActive;
        EmailConfirmed = user.EmailConfirmed;
        PhoneNumber = user.PhoneNumber;
    }

    public UserResponse()
    {
        
    }

    public string Id { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; } = true;
    public bool EmailConfirmed { get; set; }
    public string PhoneNumber { get; set; }
    public int LegacyId { get; set; }
    public string Initials { 
        get 
        {
            if (FirstName.Length != 0 && LastName.Length != 0)
            {
                return FirstName[0] + "" + LastName[0];
            }
            else {
                return "00";
            }
        } 
    }

}