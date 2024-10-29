namespace Suo.Admin.Data.Services;

public class LocalStorageService : ILocalStorageService
{
    //private readonly ProtectedLocalStorage _storage;
    private readonly Blazored.LocalStorage.ILocalStorageService _localStorage;
    private readonly IHttpContextAccessor _accessor;
    public LocalStorageService(Blazored.LocalStorage.ILocalStorageService localStorage )
    {
        //_storage = storage;
        _localStorage = localStorage;
    }
    public async Task<string> GetItemAsync<T>(string key)
    {
        var item = await _localStorage.GetItemAsync<string>(key);
        return item;

        //return "";
        //return
        //    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjM4Zjg0NjIzLWQyYzktNDc0OS05MzI2LWVmY2E5ZmUyZTRlYyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6ImFkbWluQGFkbWluLmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJWbGFkIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvc3VybmFtZSI6IkNoZXJlZG92IiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbW9iaWxlcGhvbmUiOiIiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbmlzdHJhdG9yIiwiUGVybWlzc2lvbiI6WyJQZXJtaXNzaW9ucy5Qcm9qZWN0LlZpZXciLCJQZXJtaXNzaW9ucy5Qcm9qZWN0LkNyZWF0ZSIsIlBlcm1pc3Npb25zLlByb2plY3QuRWRpdCIsIlBlcm1pc3Npb25zLlByb2plY3QuRGVsZXRlIiwiUGVybWlzc2lvbnMuUHJvamVjdC5FeHBvcnQiLCJQZXJtaXNzaW9ucy5Qcm9qZWN0LlNlYXJjaCIsIlBlcm1pc3Npb25zLlByb2plY3QuSW1wb3J0IiwiUGVybWlzc2lvbnMuVXNlcnMuVmlldyIsIlBlcm1pc3Npb25zLlVzZXJzLkNyZWF0ZSIsIlBlcm1pc3Npb25zLlVzZXJzLkVkaXQiLCJQZXJtaXNzaW9ucy5Vc2Vycy5EZWxldGUiLCJQZXJtaXNzaW9ucy5Vc2Vycy5FeHBvcnQiLCJQZXJtaXNzaW9ucy5Vc2Vycy5TZWFyY2giXSwiZXhwIjoxNjg2MjYzMjY4fQ.3ZfOZW5rSTR_3OFGl_aTeqggBGWwzwq7aTnpYtXsrTY";
    }

    public async Task RemoveItemAsync(string key)
    {
        await _localStorage.RemoveItemAsync(key);
    }

    public async Task SetItemAsync(string key, string value)
    {
        await _localStorage.SetItemAsync(key, value);
    }
}