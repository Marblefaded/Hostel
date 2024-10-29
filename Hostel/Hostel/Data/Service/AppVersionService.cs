using System.Reflection;

namespace Suo.Admin.Data.Service
{
    public class AppVersionService
    {
        public async Task<string> GetCurrentAppVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            return version;
        }
    }
}
