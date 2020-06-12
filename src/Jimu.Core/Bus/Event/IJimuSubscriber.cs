using System.Threading.Tasks;

namespace Jimu.Core.Bus
{
    public interface IJimuSubscriber<in T> where T : IJimuEvent
    {

        Task HandleAsync(IJimuSubscribeContext<T> context);
    }
}
