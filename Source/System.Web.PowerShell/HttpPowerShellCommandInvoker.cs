using System.Collections;
using System.Collections.Generic;

namespace System.Web.PowerShell
{
    public static class HttpPowerShellCommandInvoker
    {
        public static IEnumerable<T> Invoke<T>(this IHttpPowerShellCommand command, IEnumerable input = null)
        {
            return HttpPowerShell.Invoke<T>(command, input);
        }

        public static IEnumerable Invoke(this IHttpPowerShellCommand command, IEnumerable input = null)
        {
            return HttpPowerShell.Invoke(command, input);
        }
    }
}
