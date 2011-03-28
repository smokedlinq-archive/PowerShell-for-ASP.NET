using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Management.Automation;

namespace System.Web.PowerShell
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class HttpPowerShellInvoker
    {
        public static IEnumerable<T> Invoke<T>(IHttpPowerShell powerShell, string command, IEnumerable input = null, object parameters = null, bool useLocalScope = false, bool isScript = false)
        {
            if (isScript)
            {
                return powerShell.Invoke<T>(ConvertToPSCommand(ReadFile(command), parameters, useLocalScope), input);
            }

            return powerShell.Invoke<T>(ConvertToPSCommand(command, parameters, useLocalScope), input);
        }

        public static IEnumerable<PSObject> Invoke(IHttpPowerShell powerShell, string command, IEnumerable input = null, object parameters = null, bool useLocalScope = false, bool isScript = false)
        {
            return Invoke<PSObject>(powerShell, command, input, parameters, useLocalScope, isScript);
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

        static PSCommand ConvertToPSCommand(string commandText, object parameters, bool useLocalScope)
        {
            var command = new PSCommand();

            command.AddScript(commandText, useLocalScope);

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
