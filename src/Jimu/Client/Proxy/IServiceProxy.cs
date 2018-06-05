namespace Jimu.Client
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
        T GetService<T>() where T : class;
    }
}