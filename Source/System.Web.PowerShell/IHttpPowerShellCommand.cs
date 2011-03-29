using System.Management.Automation;

namespace System.Web.PowerShell
{
    public interface IHttpPowerShellCommand
    {
        PSCommand ToPSCommand();
    }
}
