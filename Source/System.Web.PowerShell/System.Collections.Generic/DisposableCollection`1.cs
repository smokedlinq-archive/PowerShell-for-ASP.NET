using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace System.Web.PowerShell
{
    internal class DisposableCollection<T> : Collection<T>, IDisposableEnumerable<T>
    {
        public DisposableCollection()
            : this(new T[0])
        {
        }

        public DisposableCollection(IEnumerable<T> collection)
            : this(collection, obj => obj as IDisposable)
        {
        }

        public DisposableCollection(IEnumerable<T> collection, Func<T, IDisposable> resolver)
            : base(collection.ToList())
        {
            this.Resolver = resolver;
        }

        ~DisposableCollection()
        {
            Dispose(false);
        }

        protected Func<T, IDisposable> Resolver
        {
            get;
            private set;
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
                    foreach (var obj in this)
                    {
                        if (obj != null)
                        {
                            var disposableObj = this.Resolver(obj);

                            if (disposableObj != null)
                            {
                                disposableObj.Dispose();
                            }
                        }
                    }

                    this.IsDisposed = true;
                }
            }
        }
    }
}
