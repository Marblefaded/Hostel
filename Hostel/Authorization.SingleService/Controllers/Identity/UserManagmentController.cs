using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suo.Autorization.SingleService.Infrastructure.Auth;
using Suo.Autorization.SingleService.Infrastructure.Interfaces.IdentityServices;
using Suo.Autorization.SingleService.Infrastructure.RequestModels;
using Suo.Autorization.SingleService.Infrastructure.ResponseModels;

namespace Suo.Autorization.SingleService.Controllers.Identity
{

    //[Authorize("Administrator")]
    [Authorize(Roles = "Administrator,SysAdmin")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserManagmentController : ControllerBase, IUserManagmentService
    {
        private readonly ICurrentUserService _currentUser;

        private readonly IUserManagmentService _userManagmentService;

        public UserManagmentController(IUserManagmentService userManagmentService)
        {
            _userManagmentService = userManagmentService;
        }

        [HttpPost]
        public async Task<int> GetCount()
        {
            return await _userManagmentService.GetCount();
        }

        [HttpPost]
        public async Task<UserResponse> GetUser(string userId)
        {
            return await _userManagmentService.GetUser(userId);
        }
        [HttpPost]
        public async Task<UserResponse> GetHeadman(string userId)
        {
            return await _userManagmentService.GetHeadman(userId);
        }

        [HttpPost]
        public async Task<List<UserResponse>> GetAll()
        {
            return await _userManagmentService.GetAll();
        }
        [HttpPost]
        public async Task<List<UserResponse>> GetAllHeadman()
        {
            return await _userManagmentService.GetAllHeadman();
        }

        [HttpPost]
        public async Task<List<UserResponse>> Take(int count, int skip)
        {
            return await _userManagmentService.Take(count, skip);
        }

        /*[HttpPost]
        public async Task<List<UserResponse>> TakeHeadman(int count, int skip)
        {
            return await _userManagmentService.TakeHeadman(count, skip);
        }*/
        [HttpPost]

        public async Task<UserRemoveResponse> RemoveUser(string userId)
        {
            return await _userManagmentService.RemoveUser(userId);
        }
        /*public async Task<UserRemoveResponse> RemoveHeadman(string userId)
        {
            return await _userManagmentService.RemoveHeadman(userId);
        }*/

        /*[AllowAnonymous]*/
        [HttpPost]
        public async Task<UserAddResponse> AddUser(UserAddRequest user)
        {
            var result = await _userManagmentService.AddUser(user);
            return result;
        }
        [HttpPost]
        public async Task<UserAddResponse> AddHeadman(UserAddRequest user)
        {
            var result = await _userManagmentService.AddHeadman(user);
            return result;
        }
        [HttpPost]
        public async Task<UserUpdateResponse> UpdateUser(UserUpdateRequest user)
        { 
            var result = await _userManagmentService.UpdateUser(user);
            return result;
        }
        public async Task<UserResponse> FindByLegacyId(int legacyId)
        {
            return await _userManagmentService.FindByLegacyId(legacyId);
        }
        public Task<bool> CheckExistUserEmail(string email)
        {
            return _userManagmentService.CheckExistUserEmail(email);
        }
    }
}
