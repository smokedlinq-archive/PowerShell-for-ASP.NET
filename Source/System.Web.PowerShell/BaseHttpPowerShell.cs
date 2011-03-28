using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Management.Automation;

namespace System.Web.PowerShell
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class BaseHttpPowerShell : IHttpPowerShell
    {
        protected BaseHttpPowerShell()
        {
            this.PSCommandObservers = new List<IObserver<PSCommand>>();
        }

        #region IDisposable
        ~BaseHttpPowerShell()
        {
            Dispose(false);
        }

        protected bool IsDisposed
        {
            get;
            private set;
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
                if (!this.IsDisposed)
                {
                    this.IsDisposed = true;
                }
            }
        }
        #endregion

        #region IObservable<PSCommand>
        protected IList<IObserver<PSCommand>> PSCommandObservers
        {
            get;
            private set;
        }

        public IDisposable Subscribe(IObserver<PSCommand> observer)
        {
            if (this.IsDisposed)
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

        public IEnumerable<T> Invoke<T>(PSCommand command, IEnumerable input)
        {
            if (this.IsDisposed)
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
                    powerShell.Commands = command;

                    this.PreparePowerShell(powerShell);

                    return powerShell.Invoke<T>(input);
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

        protected abstract void PreparePowerShell(System.Management.Automation.PowerShell instance);
    }
}
