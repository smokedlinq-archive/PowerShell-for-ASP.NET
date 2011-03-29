using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace System.Web.PowerShell
{
    public sealed class HttpPowerShell : IDisposable
    {
        Runspace _runspace;
        bool _disposed;

        private HttpPowerShell()
        {
            this._runspace = RunspaceFactory.CreateRunspace(new HttpPowerShellHost());

            try
            {
                this._runspace.ThreadOptions = PSThreadOptions.UseCurrentThread;
                this._runspace.Open();
            }
            catch
            {
                this._runspace.Dispose();
                throw;
            }
        }

        ~HttpPowerShell()
        {
            Dispose(false);
        }

        public IEnumerable<T> Invoke<T>(PSCommand command, IEnumerable input)
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(null);
            }

            using (var powerShell = System.Management.Automation.PowerShell.Create())
            {
                powerShell.Runspace = this._runspace;
                powerShell.Commands = command;
                return powerShell.Invoke<T>(input);
            }
        }

        public static IEnumerable<T> Invoke<T>(IHttpPowerShellCommand command, IEnumerable input = null)
        {
            using (var powerShell = new HttpPowerShell())
            {
                return powerShell.Invoke<T>(command.ToPSCommand(), input);
            }
        }

        public static IEnumerable<T> Invoke<T>(string script, IEnumerable input = null, object parameters = null, bool useLocalScope = false)
        {
            return Invoke<T>(HttpPowerShellCommand.FromScript(script, parameters, useLocalScope), input);
        }

        public static IEnumerable<PSObject> Invoke(IHttpPowerShellCommand command, IEnumerable input = null)
        {
            return Invoke<PSObject>(command, input);
        }

        public static IEnumerable<PSObject> Invoke(string script, IEnumerable input = null, object parameters = null, bool useLocalScope = false)
        {
            return Invoke<PSObject>(script, input, parameters, useLocalScope);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!this._disposed)
                {
                    this._runspace.Dispose();
                    this._disposed = true;
                }
            }
        }
    }
}
