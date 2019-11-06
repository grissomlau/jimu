using Jimu.APM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Jimu.Server.APM
{
    public class ApmServer : IJimuApm
    {
        readonly DiagnosticListener _diagnosticListener;
        readonly ApmServerOptions _options;
        public ApmServer(ApmServerOptions options)
        {
            _diagnosticListener = new DiagnosticListener(ListenerName);
            _options = options;
        }

        public string ListenerName => "JimuServerDiagnosticListener";

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
