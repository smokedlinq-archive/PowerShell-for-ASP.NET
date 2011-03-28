using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;

namespace System.Web.PowerShell
{
    public interface IHttpPowerShell : IDisposable, IObservable<PSCommand>
    {
        IEnumerable<T> Invoke<T>(PSCommand command, IEnumerable input = null);
    }
}
