using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;

namespace System.Web.PowerShell
{
    internal class HttpPowerShellHostUserInterface : PSHostUserInterface
    {
        static readonly HttpPowerShellHostRawUserInterface __rawUI = new HttpPowerShellHostRawUserInterface();

        readonly IHttpPowerShellHost _host;

        public HttpPowerShellHostUserInterface(IHttpPowerShellHost host)
        {
            this._host = host;
        }

        public override Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions)
        {
            throw new NotSupportedException();
        }

        public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
        {
            throw new NotSupportedException();
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName, PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
        {
            throw new NotSupportedException();
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
        {
            throw new NotSupportedException();
        }

        public override PSHostRawUserInterface RawUI
        {
            get { return __rawUI; }
        }

        public override string ReadLine()
        {
            throw new NotSupportedException();
        }

        public override SecureString ReadLineAsSecureString()
        {
            throw new NotSupportedException();
        }

        public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            this._host.Call(foo => foo.Write(foregroundColor, backgroundColor, value));
        }

        public override void Write(string value)
        {
            this._host.Call(foo => foo.Write(value));
        }

        public override void WriteDebugLine(string message)
        {
            this._host.Call(foo => foo.WriteDebugLine(message));
        }

        public override void WriteErrorLine(string value)
        {
            this._host.Call(foo => foo.WriteErrorLine(value));
        }

        public override void WriteLine(string value)
        {
            this._host.Call(foo => foo.WriteLine(value));
        }

        public override void WriteProgress(long sourceId, ProgressRecord record)
        {
        }

        public override void WriteVerboseLine(string message)
        {
            this._host.Call(foo => foo.WriteVerboseLine(message));
        }

        public override void WriteWarningLine(string message)
        {
            this._host.Call(foo => foo.WriteWarningLine(message));
        }
    }
}
