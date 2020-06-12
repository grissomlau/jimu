using SkyApm.Tracing;
using System.Collections;
using System.Collections.Generic;

namespace Jimu.Client.Diagnostic.Skywalking
{
    public class JimuClientCarrierHeaderCollection : ICarrierHeaderCollection
    {


        private readonly JimuPayload _payload;

        public JimuClientCarrierHeaderCollection(JimuPayload payload)
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
