using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Suo.Admin.Data.Service;
using Suo.Admin.Data.ViewModel;
using RouteData = Microsoft.AspNetCore.Components.RouteData;

namespace Suo.Admin.Pages.Login;

public class AuthResponderViewModel : ComponentBase
{
    [Inject] protected NavigationManager NavigationManager { get; set; }

    [Parameter] public RouteData RouteDataP { get; set; }

    [Inject]
    private AuthenticationStateProvider _stateProvider { get; set; }
    [Inject] private LogApplicationService LogService { get; set; }
    public LogApplicationViewModel LogModel = new LogApplicationViewModel();
    protected override bool ShouldRender()
    {   try
        {
            return base.ShouldRender();
        }
        catch (Exception ex)
        {
            LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
            return false;
        }
    }


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            //if (state != new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())))
            //{
            //    NavigationManager.NavigateTo("");
            //    //DeviceInfo = await js.InvokeAsync<string>("GetDeviceInfo");
            //    //_tokenModel.UserDeviceUnfo = DeviceInfo;
            //}
        }
        catch (Exception ex)
        {
            LogService.Create(LogModel, ex.Message, ex.StackTrace, DateTime.Now);
        }
    }

    private AuthenticationState Test()
    {
        throw new NotImplementedException();
    }
}