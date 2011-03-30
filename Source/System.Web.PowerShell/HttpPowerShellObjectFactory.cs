using System.Linq;
using System.Reflection;
using System.Resources;

namespace System.Web.PowerShell
{
    public sealed class HttpPowerShellObjectFactory<T>
    {
        IHttpPowerShellCommand _command;

        public static HttpPowerShellObjectFactory<T> FromFile(string path, object parameters = null, bool useLocalScope = false)
        {
            return new HttpPowerShellObjectFactory<T>()
                {
                    _command = HttpPowerShellCommand.FromFile(path, parameters, useLocalScope)
                };
        }

        public static HttpPowerShellObjectFactory<T> FromScript(string script, object parameters = null, bool useLocalScope = false)
        {
            return new HttpPowerShellObjectFactory<T>()
                {
                    _command = HttpPowerShellCommand.FromScript(script, parameters, useLocalScope)
                };
        }

        public static HttpPowerShellObjectFactory<T> FromCommand(string command, object parameters = null, bool useLocalScope = false)
        {
            return new HttpPowerShellObjectFactory<T>()
                {
                    _command = HttpPowerShellCommand.FromCommand(command, parameters, useLocalScope)
                };
        }

        public static HttpPowerShellObjectFactory<T> FromResource<TSource>(string baseName, string resourceName, object parameters = null, bool useLocalScope = false)
        {
            return new HttpPowerShellObjectFactory<T>()
                {
                    _command = HttpPowerShellCommand.FromResource<TSource>(baseName, resourceName, parameters, useLocalScope)
                };
        }

        public static HttpPowerShellObjectFactory<T> FromResource(string baseName, Assembly assembly, string resourceName, object parameters = null, bool useLocalScope = false)
        {
            return new HttpPowerShellObjectFactory<T>()
                {
                    _command = HttpPowerShellCommand.FromResource(baseName, assembly, resourceName, parameters, useLocalScope)
                };
        }

        public static HttpPowerShellObjectFactory<T> FromResource<TResource>(string resourceName, object parameters = null, bool useLocalScope = false)
        {
            return new HttpPowerShellObjectFactory<T>()
                {
                    _command = HttpPowerShellCommand.FromResource<TResource>(resourceName, parameters, useLocalScope)
                };
        }

        public static HttpPowerShellObjectFactory<T> FromResource(ResourceManager manager, string resourceName, object parameters = null, bool useLocalScope = false)
        {
            return new HttpPowerShellObjectFactory<T>()
                {
                    _command = HttpPowerShellCommand.FromResource(manager, resourceName, parameters, useLocalScope)
                };
        }

        public T CreateInstance()
        {
            return HttpPowerShell.Invoke<T>(this._command).FirstOrDefault();
        }
    }
}
