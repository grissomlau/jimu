using Jimu;
using System;
using System.Collections.Generic;
using System.Text;

namespace IService.User
{
    /// <summary>
    /// try skyapm
    /// </summary>
    [Jimu]
    public interface ILocalDiagnosticService : IJimuService
    {

        [JimuGet(true)]
        void Get(string name);
    }
}
