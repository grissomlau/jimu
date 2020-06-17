using System.Collections.Generic;

namespace Jimu.Server.UnitOfWork.EF
{
    public class MultipleEFOptions : List<EFOptions>
    {
        public bool Enable { get; set; } = true;
    }
}
