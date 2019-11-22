namespace Jimu.Client.Proxy
{
    /// <summary>
    ///     serice proxy
    /// </summary>
    public interface IServiceProxy
    {
        /// <summary>
        ///     get specify service proxy by service type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetService<T>(JimuPayload payload = null) where T : class;
    }
}