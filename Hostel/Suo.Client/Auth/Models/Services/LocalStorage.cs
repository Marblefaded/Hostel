using Microsoft.AspNetCore.Mvc.Filters;

namespace Suo.Client.Data.Models.Services;

public class LocalStorage 
{
    public HttpContext Context { get; }
    private readonly bool IsSessionEnabled = false;
    /*private readonly Blazored.LocalStorage.ILocalStorageService _localStorage;
    private readonly IHttpContextAccessor _accessor;*/
    public LocalStorage(HttpContext context)
    {
        Context = context;
    }


    public void SetItem(string key, string value)
    {
        if (IsSessionEnabled)
        {
            SaveSessionValue(key, value);
        }
        else
        {
            SaveCookieValue(key, value);
        }
    }

    public string? GetItem(string key)
    {
        if (IsSessionEnabled)
        {
            return GetSessionValue(key);
        }
        else
        {
            return GetCookieValue(key);
        }
    }


    private void SaveCookieValue(string key, string value)
    {
        Context.Response.Cookies.Append(key, value, new CookieOptions(){ Expires = DateTimeOffset.Now.AddDays(7)}); 

    }
    private void SaveSessionValue(string key, string value)
    {
        Context.Session.SetString(key, value);
    }

    private string? GetCookieValue(string key)
    {
        string resultValue = "";
        if (
            Context.Request.Cookies.TryGetValue(key, out resultValue))
        {
            return resultValue;
        }
        else
        {
            return null;
        }

    }

    private string? GetSessionValue(string key)
    {
        return Context.Session.GetString(key);
    }
}