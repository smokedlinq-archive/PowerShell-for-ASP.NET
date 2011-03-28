using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace System.Web.PowerShell
{
    public class HttpPowerShell : BaseHttpPowerShell
    {
        private HttpPowerShell(IHttpPowerShellHost host = null, PSThreadOptions threadOptions = PSThreadOptions.UseCurrentThread)
        {
            var runspace = RunspaceFactory.CreateRunspace(new HttpPowerShellHost(host));

            try
            {
                runspace.ThreadOptions = PSThreadOptions.UseCurrentThread;
                runspace.Open();

                this.Runspace = runspace;
            }
            catch
            {
                runspace.Dispose();
                throw;
            }
        }

        public static HttpPowerShell Create(IHttpPowerShellHost host = null, PSThreadOptions threadOptions = PSThreadOptions.UseCurrentThread)
        {
            return new HttpPowerShell(host, threadOptions);
        }

        public static IEnumerable<T> Invoke<T>(string command, IEnumerable input = null, object parameters = null, bool useLocalScope = false, bool isScript = false)
        {
            using (var ps = HttpPowerShell.Create())
            {
                return HttpPowerShellInvoker.Invoke<T>(ps, command, input, parameters, useLocalScope, isScript);
            }
        }

        public static IEnumerable<PSObject> Invoke(string command, IEnumerable input = null, object parameters = null, bool useLocalScope = false, bool isScript = false)
        {
            return Invoke<PSObject>(command, input, parameters, useLocalScope, isScript);
        }

        protected Runspace Runspace
        {
            get;
            private set;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!this.IsDisposed)
                {
                    this.Runspace.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        protected override void PreparePowerShell(System.Management.Automation.PowerShell instance)
        {
            instance.Runspace = this.Runspace;
        }
    }
}
