using System.Configuration;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace System.Web.PowerShell
{
    public class HttpPowerShellConfigurationFactory<T> : PowerShellConfigurationFactory<T>
        where T : IConfiguration
    {
        public static HttpPowerShellConfigurationFactory<T> FromFile(string path, PSThreadOptions threadOptions = PSThreadOptions.UseCurrentThread, IHttpPowerShellHost host = null)
        {
            var content = HttpPowerShell.ReadFile(path);

            var factory = new HttpPowerShellConfigurationFactory<T>()
                {
                    Commands = new PSCommand(),
                    ThreadOptions = threadOptions,
                    Host = host
                };

            factory.Commands.AddScript(content, true);

            return factory;
        }

        protected PSThreadOptions ThreadOptions
        {
            get;
            private set;
        }

        protected IHttpPowerShellHost Host
        {
            get;
            private set;
        }

        protected override Runspace CreateRunspace(RunspaceConfiguration config)
        {
            return HttpPowerShell.CreateRunspace(config, this.ThreadOptions, this.Host);
        }
    }
}
