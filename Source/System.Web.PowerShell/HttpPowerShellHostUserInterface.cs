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
            if (this._host != null)
            {
                this._host.Write(foregroundColor, backgroundColor, value);
            }
        }

        public override void Write(string value)
        {
            if (this._host != null)
            {
                this._host.Write(value);
            }
        }

        public override void WriteDebugLine(string message)
        {
            if (this._host != null)
            {
                this._host.WriteDebugLine(message);
            }
        }

        public override void WriteErrorLine(string value)
        {
            if (this._host != null)
            {
                this._host.WriteErrorLine(value);
            }
        }

        public override void WriteLine(string value)
        {
            if (this._host != null)
            {
                this._host.WriteLine(value);
            }
        }

        public override void WriteProgress(long sourceId, ProgressRecord record)
        {
        }

        public override void WriteVerboseLine(string message)
        {
            if (this._host != null)
            {
                this._host.WriteVerboseLine(message);
            }
        }

        public override void WriteWarningLine(string message)
        {
            if (this._host != null)
            {
                this._host.WriteWarningLine(message);
            }
        }
    }
}
