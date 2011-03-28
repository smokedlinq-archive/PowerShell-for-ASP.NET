using System.Configuration;
using System.Linq;
using System.Management.Automation.Runspaces;

namespace System.Management.Automation
{
    public class PowerShellConfigurationFactory<T> : IConfigurationFactory<T>
        where T : IConfiguration
    {
        public T CreateInstance()
        {
            return Invoke();
        }

        public PSCommand Commands
        {
            get;
            set;
        }

        protected virtual Runspace CreateRunspace(RunspaceConfiguration config)
        {
            return RunspaceFactory.CreateRunspace(config);
        }

        protected T Invoke()
        {
            var config = RunspaceConfiguration.Create();
            var targetTypeAssembly = typeof(T).Assembly;

            config.Assemblies.Append(new AssemblyConfigurationEntry(targetTypeAssembly.FullName, targetTypeAssembly.Location));

            using (var runspace = CreateRunspace(config))
            { 
                runspace.Open();

                using (var ps = PowerShell.Create())
                {
                    ps.Runspace = runspace;
                    ps.Commands = this.Commands;
                    ps.AddParameter("Type", typeof(T));

                    return ps.Invoke<T>().FirstOrDefault();
                }
            }
        }
    }
}
