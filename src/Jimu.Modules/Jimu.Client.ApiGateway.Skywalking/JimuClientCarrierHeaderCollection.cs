using SkyApm.Tracing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client.ApiGateway.Skywalking
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
