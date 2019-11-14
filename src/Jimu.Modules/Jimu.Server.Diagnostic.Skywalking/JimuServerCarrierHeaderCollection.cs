using SkyApm.Tracing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Server.Diagnostic.Skywalking
{
    public class JimuServerCarrierHeaderCollection : ICarrierHeaderCollection
    {

        readonly JimuPayload _payload;
        public JimuServerCarrierHeaderCollection(JimuPayload payload)
        {
            _payload = payload;
        }
        public void Add(string key, string value)
        {
            if (_payload.Items.ContainsKey(key))
                _payload.Items[key] = value;
            else
                _payload.Items.Add(key, value);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            foreach (var item in _payload.Items)
            {
                yield return new KeyValuePair<string, string>(item.Key, item.Value + "");
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in _payload.Items)
            {
                yield return new KeyValuePair<string, string>(item.Key, item.Value + "");
            }
        }
    }
}
