using SkyApm.Tracing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client.ApiGateway.Skywalking
{
    public class JimuClientCarrierHeaderCollection : ICarrierHeaderCollection
    {


        private readonly List<KeyValuePair<string, string>> _tracingHeaders;

        public JimuClientCarrierHeaderCollection()
        {
            _tracingHeaders = new List<KeyValuePair<string, string>>();
        }
        public void Add(string key, string value)
        {
            _tracingHeaders.Add(new KeyValuePair<string, string>(key, value));
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _tracingHeaders.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _tracingHeaders.GetEnumerator();
        }
    }
}
