namespace Jimu.Bus
{
    public interface IJimuSubscribeContext<out T> where T : IJimuEvent
    {
        T Message { get; }
        IJimuBus JimuBus { get; }
    }
}
