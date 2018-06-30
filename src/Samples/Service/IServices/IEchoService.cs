using System.Threading.Tasks;
using Jimu;

namespace IServices
{
    [JimuServiceRoute("api/echo")]
    public interface IEchoService : IJimuService
    {
        [JimuService(CreatedBy = "grissom", Comment = "get input things")]
        string GetEcho(string anything);

        [JimuService(CreatedBy = "grissom", EnableAuthorization = true, Comment = "set echo and return the echo with indicate whether success flag")]
        Task<string> SetEcho(string anything);
    }
}
