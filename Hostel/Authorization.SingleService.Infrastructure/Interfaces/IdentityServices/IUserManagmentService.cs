using Suo.Autorization.SingleService.Infrastructure.RequestModels;
using Suo.Autorization.SingleService.Infrastructure.ResponseModels;

namespace Suo.Autorization.SingleService.Infrastructure.Interfaces.IdentityServices;

public interface IUserManagmentService
{
    Task<int> GetCount();
    Task<UserResponse> GetUser(string userId);
    Task<UserResponse> GetHeadman(string userId);
    public Task<List<UserResponse>> GetAll();
    public Task<List<UserResponse>> GetAllHeadman();
    public Task<List<UserResponse>> Take(int count,int skip);
   /* public Task<List<UserResponse>> TakeHeadman(int count, int skip);*/
    public Task<UserResponse> FindByLegacyId(int legacyId);
    public Task<bool> CheckExistUserEmail(string email);

    //Task<IResult> RegisterAsync(RegisterRequest request, string origin);
    //Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request);
    //Task<IResult> ResetPasswordAsync(ResetPasswordRequest request);
    Task<UserRemoveResponse> RemoveUser(string userId);
    /*Task<UserRemoveResponse> RemoveHeadman(string userId);*/

    Task<UserAddResponse> AddUser(UserAddRequest user);
    Task<UserAddResponse> AddHeadman(UserAddRequest user);
    Task<UserUpdateResponse> UpdateUser(UserUpdateRequest user);

}