namespace Suo.Admin.Data.Services;

public interface ILocalStorageService
{
    Task<string> GetItemAsync<T>(string key);
    Task RemoveItemAsync(string key);
    Task SetItemAsync(string key,string value);
}