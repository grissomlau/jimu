using Jimu.APM;
using System.Diagnostics;

namespace Jimu.Client.APM
{
    public class ApmClient : IJimuApm
    {
        readonly DiagnosticListener _diagnosticListener;
        readonly ApmClientOptions _options;
        public ApmClient(ApmClientOptions options)
        {
            _diagnosticListener = new DiagnosticListener(ListenerName);
            _options = options;
        }

        public string ListenerName => ApmClientEventType.ListenerName;

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
