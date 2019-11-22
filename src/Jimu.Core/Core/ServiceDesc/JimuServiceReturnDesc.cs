using System.Collections.Generic;

namespace Jimu
{
    public class JimuServiceReturnDesc
    {
        public string ReturnType { get; set; }
        public string ReturnFormat { get; set; }
        public string Comment { get; set; }
        public List<JimuServiceParameterDesc> Properties { get; set; }
    }
}
