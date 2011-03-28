using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;

namespace System.Web.PowerShell
{
    public class HttpPowerShellPool : BaseHttpPowerShell
    {
        private HttpPowerShellPool(int minRunspaces, int maxRunspaces, IHttpPowerShellHost host, PSThreadOptions threadOptions)
        {
            var runspacePool = RunspaceFactory.CreateRunspacePool(minRunspaces, maxRunspaces, new HttpPowerShellHost(host));

            try
            {
                runspacePool.ThreadOptions = PSThreadOptions.UseCurrentThread;
                runspacePool.Open();

                this.RunspacePool = runspacePool;
            }
            catch
            {
                runspacePool.Dispose();
                throw;
            }
        }

        public static HttpPowerShellPool Create(IHttpPowerShellHost host = null, PSThreadOptions threadOptions = PSThreadOptions.UseCurrentThread)
        {
            var minRunspaces = GetMinThreads();
            var maxRunspaces = GetMaxThreads();

            return Create(minRunspaces, maxRunspaces, host, threadOptions);
        }

        public static HttpPowerShellPool Create(int minRunspaces, int maxRunspaces, IHttpPowerShellHost host = null, PSThreadOptions threadOptions = PSThreadOptions.UseCurrentThread)
        {
            return new HttpPowerShellPool(minRunspaces, maxRunspaces, host, threadOptions);
        }

        public static IEnumerable<T> Invoke<T>(string command, IEnumerable input = null, object parameters = null, bool useLocalScope = false, bool isScript = false)
        {
            using (var ps = HttpPowerShellPool.Create())
            {
                return HttpPowerShellInvoker.Invoke<T>(ps, command, input, parameters, useLocalScope, isScript);
            }
        }

        public static IEnumerable<PSObject> Invoke(string command, IEnumerable input = null, object parameters = null, bool useLocalScope = false, bool isScript = false)
        {
            return Invoke<PSObject>(command, input, parameters, useLocalScope, isScript);
        }

        protected RunspacePool RunspacePool
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
                    this.RunspacePool.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        protected override void PreparePowerShell(System.Management.Automation.PowerShell instance)
        {
            instance.RunspacePool = this.RunspacePool;
        }

        static int GetMinThreads()
        {
            int workerThreads, completionPortThreads;
            ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
            return workerThreads;
        }

        static int GetMaxThreads()
        {
            int workerThreads, completionPortThreads;
            ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
            return workerThreads;
        }
    }
}
