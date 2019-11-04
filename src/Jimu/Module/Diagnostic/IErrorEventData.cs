using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Diagnostic
{
    public interface IErrorEventData
    {
        Exception Ex { get; }
    }
}
