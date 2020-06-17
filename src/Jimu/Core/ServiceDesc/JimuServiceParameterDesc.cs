using System.Collections.Generic;

namespace Jimu
{
    public class JimuServiceParameterDesc
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Comment { get; set; }

        public string Default { get; set; }
        public List<JimuServiceParameterDesc> Properties { get; set; }
    }
}
