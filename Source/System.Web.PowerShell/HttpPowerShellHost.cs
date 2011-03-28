using System.Globalization;
using System.Management.Automation.Host;
using System.Threading;

namespace System.Web.PowerShell
{
    internal class HttpPowerShellHost : PSHost
    {
        static readonly Version __version = new Version(1, 0, 0, 0);

        readonly HttpPowerShellHostUserInterface _ui;
        readonly Guid _instanceId;
        readonly IHttpPowerShellHost _host;

        public HttpPowerShellHost(IHttpPowerShellHost host = null)
        {
            this._ui = new HttpPowerShellHostUserInterface(_host);
            this._instanceId = Guid.NewGuid();
            this._host = host;
        }

        public override CultureInfo CurrentCulture
        {
            get { return Thread.CurrentThread.CurrentCulture; }
        }

        public override CultureInfo CurrentUICulture
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
        }

        public override void EnterNestedPrompt()
        {
        }

        public override void ExitNestedPrompt()
        {
        }

        public override Guid InstanceId
        {
            get { return this._instanceId; }
        }

        public override string Name
        {
            get { return "System.Web.PowerShell"; }
        }

        public override void NotifyBeginApplication()
        {
            if (this._host != null)
            {
                this._host.NotifyBeginApplication();
            }
        }

        public override void NotifyEndApplication()
        {
            if (this._host != null)
            {
                this._host.NotifyEndApplication();
            }
        }

        public override void SetShouldExit(int exitCode)
        {
            if (this._host != null)
            {
                this._host.SetShouldExit(exitCode);
            }
        }

        public override PSHostUserInterface UI
        {
            get { return _ui; }
        }

        public override Version Version
        {
            get { return __version; }
        }
    }
}
