using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace System.Web.PowerShell
{
    public static class HttpPowerShellHelpers
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

        internal static void Call(this IHttpPowerShellHost host, Action<IHttpPowerShellHost> action)
        {
            if (host != null)
            {
                action(host);
            }
        }
    }
}
