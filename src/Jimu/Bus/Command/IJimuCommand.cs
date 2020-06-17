namespace Jimu.Bus
{
    /// <summary>
    /// command for Consumer, which will be publish by Ibus
    /// </summary>
    public interface IJimuCommand
    {
        /// <summary>
        /// specify the queue to consume
        /// </summary>
        string QueueName { get; }
    }
}
