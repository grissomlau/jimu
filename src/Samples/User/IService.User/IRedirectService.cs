using Jimu;

namespace IService.User
{
    /// <summary>
    /// try redirect to sepcify url
    /// </summary>
    [Jimu("/{Service}")]
    public interface IRedirectService : IJimuService
    {
        [JimuGet(true)]
        JimuRedirect Go(string url);
    }
}
