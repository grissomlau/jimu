using System;

namespace Jimu.Diagnostic
{
    public interface IErrorEventData
    {
        Exception Ex { get; }
    }
}
