namespace Jimu.Bus
{
    public interface IJimuRequestContext<out T> where T : IJimuRequest
    {
        T Message { get; }
        IJimuBus JimuBus { get; }
    }
}
