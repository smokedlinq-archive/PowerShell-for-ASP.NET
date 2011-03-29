using System.Linq;

namespace System.Web.PowerShell
{
    public sealed class HttpPowerShellConfigurationFactory<T>
    {
        IHttpPowerShellCommand _command;

        public static HttpPowerShellConfigurationFactory<T> FromFile(string path, object parameters = null, bool useLocalScope = false)
        {
            return new HttpPowerShellConfigurationFactory<T>()
                {
                    _command = HttpPowerShellCommand.FromFile(path, parameters, useLocalScope)
                };
        }

        public static HttpPowerShellConfigurationFactory<T> FromScript(string script, object parameters = null, bool useLocalScope = false)
        {
            return new HttpPowerShellConfigurationFactory<T>()
                {
                    _command = HttpPowerShellCommand.FromScript(script, parameters, useLocalScope)
                };
        }

        public static HttpPowerShellConfigurationFactory<T> FromCommand(string command, object parameters = null, bool useLocalScope = false)
        {
            return new HttpPowerShellConfigurationFactory<T>()
                {
                    _command = HttpPowerShellCommand.FromCommand(command, parameters, useLocalScope)
                };
        }

        public T CreateInstance()
        {
            return HttpPowerShell.Invoke<T>(this._command).FirstOrDefault();
        }
    }
}
