
namespace System.Web.PowerShell
{
    public interface IHttpPowerShellHost
    {
        void NotifyBeginApplication();
        void NotifyEndApplication();
        void SetShouldExit(int exitCode);

        void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value);
        void Write(string value);
        void WriteDebugLine(string message);
        void WriteErrorLine(string value);
        void WriteLine(string value);
        void WriteVerboseLine(string message);
        void WriteWarningLine(string message);
    }
}
