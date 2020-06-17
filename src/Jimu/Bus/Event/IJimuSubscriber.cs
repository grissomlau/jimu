using System.Threading.Tasks;

namespace Jimu.Bus
{
    public interface IJimuSubscriber<in T> where T : IJimuEvent
    {

        Task HandleAsync(IJimuSubscribeContext<T> context);
    }
}
