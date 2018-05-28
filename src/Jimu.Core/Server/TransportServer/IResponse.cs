using System.Threading.Tasks;
using Jimu.Core.Protocols;

namespace Jimu.Core.Server.TransportServer
{
    /// <summary>
    ///     server response
    /// </summary>
    public interface IResponse
    {
        Task WriteAsync(string messageId, RemoteInvokeResultMessage resultMessage);
    }
}