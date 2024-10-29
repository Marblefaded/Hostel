namespace Suo.Client.Extentions;

public class AppConfiguration
{
    public string Secret { get; set; }
    public string IdentityServer { get; set; }
    public string AdminProjectUrl { get; set; }
    public string TelegramLink { get; set; }

    //public bool BehindSSLProxy { get; set; }

    //public string ProxyIP { get; set; }

    //public string ApplicationUrl { get; set; }
}

public static class AppConfigGlobals
{
    public static string IdentityServer { get; set; }
    public static string Secret { get; set; }
}