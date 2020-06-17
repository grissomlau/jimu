namespace Jimu.Bus
{
    /// <summary>
    /// request for handler, which will be sent by Ibus
    /// </summary>
    public interface IJimuRequest
    {
        /// <summary>
        /// specify the queue to handle
        /// </summary>
        string QueueName { get; }
    }
}
