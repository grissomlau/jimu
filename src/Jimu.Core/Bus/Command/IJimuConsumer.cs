using System.Threading.Tasks;

namespace Jimu.Core.Bus
{
    public interface IJimuConsumer<in T> where T : IJimuCommand
    {
        Task ConsumeAsync(IJimuConsumeContext<T> context);
    }
}
