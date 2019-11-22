using Jimu.Diagnostic;
using System.Diagnostics;

namespace Jimu.Client.Diagnostic
{
    public class DiagnosticClient : IJimuDiagnostic
    {
        readonly DiagnosticListener _diagnosticListener;
        readonly DiagnosticClientOptions _options;
        public DiagnosticClient(DiagnosticClientOptions options)
        {
            _diagnosticListener = new DiagnosticListener(ListenerName);
            _options = options;
        }

        public string ListenerName => DiagnosticClientEventType.ListenerName;

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
