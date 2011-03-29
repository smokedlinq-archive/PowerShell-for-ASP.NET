using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace System.Web.PowerShell
{
    internal sealed class DisposableCollection<T> : Collection<T>, IDisposableEnumerable<T>
    {
        Func<T, IDisposable> _converter;
        bool _disposed;

        public DisposableCollection()
            : this(new T[0])
        {
        }

        public DisposableCollection(IEnumerable<T> collection)
            : this(collection, obj => obj as IDisposable)
        {
        }

        public DisposableCollection(IEnumerable<T> collection, Func<T, IDisposable> converter)
            : base(collection.ToList())
        {
            this._converter = converter;
        }

        ~DisposableCollection()
        {
            Dispose(false);
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
                    foreach (var obj in this)
                    {
                        if (obj != null)
                        {
                            var disposableObj = this._converter(obj);

                            if (disposableObj != null)
                            {
                                disposableObj.Dispose();
                            }
                        }
                    }

                    this._disposed = true;
                }
            }
        }
    }
}
