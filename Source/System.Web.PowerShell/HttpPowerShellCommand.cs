using System.Dynamic;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Resources;

namespace System.Web.PowerShell
{
    public sealed class HttpPowerShellCommand : IHttpPowerShellCommand
    {
        PSCommand _command;

        public static HttpPowerShellCommand FromFile(string path, object parameters = null, bool useLocalScope = false)
        {
            return FromScript(ReadFile(path), parameters, useLocalScope);
        }

        public static HttpPowerShellCommand FromScript(string script, object parameters = null, bool useLocalScope = false)
        {
            return new HttpPowerShellCommand
                {
                    _command = ConvertToPSCommand(x => x.AddScript(script, useLocalScope), parameters)
                };
        }

        public static HttpPowerShellCommand FromCommand(string command, object parameters = null, bool useLocalScope = false)
        {
            return new HttpPowerShellCommand
                {
                    _command = ConvertToPSCommand(x => x.AddCommand(command, useLocalScope), parameters)
                };
        }

        public static HttpPowerShellCommand FromResource<T>(string baseName, string resourceName, object parameters = null, bool useLocalScope = false)
        {
            return FromResource(baseName, typeof(T).Assembly, resourceName, parameters, useLocalScope);
        }

        public static HttpPowerShellCommand FromResource(string baseName, Assembly assembly, string resourceName, object parameters = null, bool useLocalScope = false)
        {
            return FromResource(new ResourceManager(baseName, assembly), resourceName, parameters, useLocalScope);
        }

        public static HttpPowerShellCommand FromResource<T>(string resourceName, object parameters = null, bool useLocalScope = false)
        {
            return FromResource(new ResourceManager(typeof(T)), resourceName, parameters, useLocalScope);
        }

        public static HttpPowerShellCommand FromResource(ResourceManager manager, string resourceName, object parameters = null, bool useLocalScope = false)
        {
            var script = manager.GetObject(resourceName);

            if (script == null || !(script is string))
            {
                throw new MissingManifestResourceException();
            }

            return new HttpPowerShellCommand
                {
                    _command = ConvertToPSCommand(x => x.AddScript((string)script, useLocalScope), parameters)
                };
        }

        public PSCommand ToPSCommand()
        {
            return this._command;
        }

        static string ReadFile(string path)
        {
            var actualPath = path;

            if (path.StartsWith("~/"))
            {
                actualPath = VirtualPathUtility.ToAbsolute(path);

                if (HttpContext.Current != null)
                {
                    actualPath = HttpContext.Current.Server.MapPath(actualPath);
                }
            }

            return File.ReadAllText(actualPath);
        }

        static PSCommand ConvertToPSCommand(Action<PSCommand> initializer, object parameters)
        {
            var command = new PSCommand();

            initializer(command);

            if (parameters != null)
            {
                foreach (var parameter in parameters.ToDictionary())
                {
                    command.AddParameter(parameter.Key, parameter.Value);
                }
            }

            return command;
        }
    }
}
