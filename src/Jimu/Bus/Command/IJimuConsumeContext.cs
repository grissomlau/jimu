namespace Jimu.Bus
{
    public interface IJimuConsumeContext<out T> where T : IJimuCommand
    {
        T Message { get; }
        IJimuBus JimuBus { get; }
    }
}
