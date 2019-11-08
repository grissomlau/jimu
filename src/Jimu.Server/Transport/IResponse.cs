using System.Threading.Tasks;

namespace Jimu.Server.Transport
{
    /// <summary>
    ///     server response
    /// </summary>
    public interface IResponse
    {
        Task WriteAsync(string messageId, JimuRemoteCallResultData resultMessage);
    }
}