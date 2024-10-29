namespace Suo.Admin.Extentions;

public class AppConfiguration
{
    public string Secret { get; set; }
    public string IdentityServer { get; set; }
    public string IdentityServerUseHttps { get; set; }
    public string TelegrammString { get; set; }
    public string ConnectionString { get; set; }

    //public bool BehindSSLProxy { get; set; }

    //public string ProxyIP { get; set; }

    //public string ApplicationUrl { get; set; }
}