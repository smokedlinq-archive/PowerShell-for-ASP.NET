using System.Configuration;
using System.Linq;
using System.Management.Automation.Runspaces;

namespace System.Web.PowerShell
{
    public class HttpPowerShellConfigurationFactory<T> : IConfigurationFactory<T>
        where T : IConfiguration
    {
        string _path;
        PSThreadOptions _threadOptions;
        IHttpPowerShellHost _host;

        public static HttpPowerShellConfigurationFactory<T> FromFile(string path, IHttpPowerShellHost host = null, PSThreadOptions threadOptions = PSThreadOptions.UseCurrentThread)
        {
            return new HttpPowerShellConfigurationFactory<T>()
                {
                    _path = path,
                    _threadOptions = threadOptions,
                    _host = host
                };
        }

        public T CreateInstance()
        {
            using (var ps = HttpPowerShell.Create(this._host, this._threadOptions))
            {
                return HttpPowerShellInvoker.Invoke<T>(ps, this._path, isScript: true).FirstOrDefault();
            }
        }
    }
}
