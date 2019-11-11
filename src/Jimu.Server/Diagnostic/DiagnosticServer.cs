using Jimu.Diagnostic;
using System.Diagnostics;

namespace Jimu.Server.Diagnostic
{
    public class DiagnosticServer : IJimuDiagnostic
    {
        readonly DiagnosticListener _diagnosticListener;
        readonly DiagnosticServerOptions _options;
        public DiagnosticServer(DiagnosticServerOptions options)
        {
            _diagnosticListener = new DiagnosticListener(ListenerName);
            _options = options;
        }

        public string ListenerName => DiagnosticServerEventType.ListenerName;

        public bool IsEnabled(string name)
        {
            if (_options.Enable)
            {
                return _diagnosticListener.IsEnabled(name);
            }
            else
            {
                return false;
            }
        }

        public void Write(string name, object value)
        {
            if (_options.Enable)
            {
                _diagnosticListener.Write(name, value);
            }
        }

    }
}
