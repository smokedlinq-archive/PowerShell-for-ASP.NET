using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;

namespace System.Web.PowerShell
{
    public class HttpPowerShell : IDisposable, IObservable<PSCommand>
    {
        private HttpPowerShell(int minRunspaces, int maxRunspaces, PSThreadOptions threadOptions, IHttpPowerShellHost host)
        {
            var runspacePool = RunspaceFactory.CreateRunspacePool(minRunspaces, maxRunspaces, new HttpPowerShellHost(host));

            try
            {
                runspacePool.ThreadOptions = threadOptions;
                runspacePool.Open();

                this.RunspacePool = runspacePool;
                this.PSCommandObservers = new List<IObserver<PSCommand>>();
            }
            catch
            {
                runspacePool.Dispose();
                throw;
            }
        }

        #region IDisposable
        ~HttpPowerShell()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!this.RunspacePool.IsDisposed)
                {
                    this.RunspacePool.Dispose();

                    foreach (var observer in this.PSCommandObservers)
                    {
                        observer.OnCompleted();
                    }
                }
            }
        }

        internal bool IsDisposed
        {
            get { return this.RunspacePool.IsDisposed; }
        }
        #endregion

        #region IObservable
        protected IList<IObserver<PSCommand>> PSCommandObservers
        {
            get;
            private set;
        }

        public IDisposable Subscribe(IObserver<PSCommand> observer)
        {
            if (this.RunspacePool.IsDisposed)
            {
                throw new ObjectDisposedException(null);
            }

            if (!this.PSCommandObservers.Contains(observer))
            {
                this.PSCommandObservers.Add(observer);
            }

            return new Unsubscriber<PSCommand>(observer, this.PSCommandObservers);
        }

        class Unsubscriber<T> : IDisposable
        {
            IList<IObserver<T>> _observers;
            IObserver<T> _observer;

            public Unsubscriber(IObserver<T> observer, IList<IObserver<T>> observers)
            {
                this._observer = observer;
                this._observers = observers;
            }

            ~Unsubscriber()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (this._observers.Contains(this._observer))
                {
                    this._observers.Remove(this._observer);
                }
            }
        }
        #endregion

        public static HttpPowerShell Create(PSThreadOptions threadOptions = PSThreadOptions.UseCurrentThread, IHttpPowerShellHost host = null)
        {
            var minRunspaces = GetMinThreads();
            var maxRunspaces = GetMaxThreads();

            return Create(minRunspaces, maxRunspaces, threadOptions, host);
        }

        public static HttpPowerShell Create(int minRunspaces, int maxRunspaces, PSThreadOptions threadOptions = PSThreadOptions.UseCurrentThread, IHttpPowerShellHost host = null)
        {
            return new HttpPowerShell(minRunspaces, maxRunspaces, threadOptions, host);
        }

        protected RunspacePool RunspacePool
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Synchronously runs the pipeline of the <see cref="System.Management.Automation.PowerShell"/> object by using the supplied script, input data, and parameters. The script is loaded from the file system.
        /// </summary>
        /// <param name="path">The path to the script.</param>
        /// <param name="input">Input data for the script.</param>
        /// <param name="parameters">Parameters to pass to the script.</param>
        /// <returns>Returns a <see cref="System.Collections.ObjectModel.Collection`1"/> collection of <see cref="System.Management.Automation.PSObject"/> objects that contain the output of the script.</returns>
        public IDisposableEnumerable<PSObject> InvokeScript(string path, IEnumerable input = null, object parameters = null)
        {
            return Invoke(ReadFile(path), input, parameters);
        }

        /// <summary>
        /// Synchronously runs the pipeline of the <see cref="System.Management.Automation.PowerShell"/> object by using the supplied command text, input data, and parameters.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="input">Input data for the command.</param>
        /// <param name="parameters">Parameters to pass to the command.</param>
        /// <returns>Returns a <see cref="System.Collections.ObjectModel.Collection`1"/> collection of <see cref="System.Management.Automation.PSObject"/> objects that contain the output of the command.</returns>
        public IDisposableEnumerable<PSObject> Invoke(string commandText, IEnumerable input = null, object parameters = null)
        {
            return Invoke(ConvertToPSCommand(commandText, parameters), input);
        }

        /// <summary>
        /// Synchronously runs the pipeline of the <see cref="System.Management.Automation.PowerShell"/> object by using the supplied command and input data.
        /// </summary>
        /// <param name="command">The PowerShell command.</param>
        /// <param name="input">Input data for the command.</param>
        /// <returns>Returns a <see cref="System.Collections.ObjectModel.Collection`1"/> collection of <see cref="System.Management.Automation.PSObject"/> objects that contain the output of the command.</returns>
        public IDisposableEnumerable<PSObject> Invoke(PSCommand command, IEnumerable input = null)
        {
            if (this.RunspacePool.IsDisposed)
            {
                throw new ObjectDisposedException(null);
            }

            try
            {
                foreach (var observer in this.PSCommandObservers)
                {
                    observer.OnNext(command);
                }

                using (var powerShell = System.Management.Automation.PowerShell.Create())
                {
                    powerShell.RunspacePool = this.RunspacePool;
                    powerShell.Commands = command;

                    var output = powerShell.Invoke(input);

                    return new DisposableCollection<PSObject>(output, obj => obj.BaseObject as IDisposable);
                }
            }
            catch (Exception ex)
            {
                foreach (var observer in this.PSCommandObservers)
                {
                    observer.OnError(ex);
                }

                throw;
            }
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

        internal static PSCommand ConvertToPSCommand(string commandText, object parameters)
        {
            var command = new PSCommand();

            command.AddScript(commandText, true);

            if (parameters != null)
            {
                foreach(var parameter in parameters.ToDictionary())
                {
                    command.AddParameter(parameter.Key, parameter.Value);
                }
            }

            return command;
        }

        internal static string ReadFile(string path)
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

        internal static Runspace CreateRunspace(RunspaceConfiguration config = null, PSThreadOptions threadOptions = PSThreadOptions.UseCurrentThread, IHttpPowerShellHost host = null)
        {
            Runspace runspace;

            if (config == null)
            {
                runspace = RunspaceFactory.CreateRunspace(new HttpPowerShellHost(host));
            }
            else
            {
                runspace = RunspaceFactory.CreateRunspace(new HttpPowerShellHost(host), config);
            }

            try
            {
                runspace.ThreadOptions = threadOptions;
                return runspace;
            }
            catch
            {
                runspace.Dispose();
                throw;
            }
        }
    }
}
