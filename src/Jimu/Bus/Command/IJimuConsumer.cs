using System.Threading.Tasks;

namespace Jimu.Bus
{
    public interface IJimuConsumer<in T> where T : IJimuCommand
    {
        Task ConsumeAsync(IJimuConsumeContext<T> context);
    }
}
