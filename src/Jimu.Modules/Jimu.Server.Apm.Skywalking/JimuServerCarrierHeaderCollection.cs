using SkyApm.Tracing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Server.Apm.Skywalking
{
    public class JimuServerCarrierHeaderCollection : ICarrierHeaderCollection
    {
        public void Add(string key, string value)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
