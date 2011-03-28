using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management.Automation;

namespace System.Web.PowerShell
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class HttpPowerShellPSObjectConverter
    {
        public static IEnumerable<dynamic> AsDynamic(this IEnumerable<PSObject> collection)
        {
            return collection.Select(obj => obj.ToDynamic());
        }

        public static IEnumerable<PSObject> AsPSObject(this IEnumerable<dynamic> collection)
        {
            return collection.OfType<DynamicPSObject>().Select(i => (PSObject)i);
        }

        public static dynamic ToDynamic(this PSObject obj)
        {
            return (DynamicPSObject)obj;
        }

        public static IDisposableEnumerable<PSObject> AsDisposable(this IEnumerable<PSObject> collection)
        {
            return new DisposableCollection<PSObject>(collection, obj => obj.BaseObject as IDisposable);
        }
    }
}
