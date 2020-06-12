using Jimu;

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
