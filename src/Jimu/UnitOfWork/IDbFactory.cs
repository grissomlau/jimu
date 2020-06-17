namespace Jimu.UnitOfWork
{
    /// <summary>
    ///  DbFactory create isolate instance of T everytime, note: this instance not work in UnitOfWork
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDbFactory<T> where T : class
    {
        T Create(string optionName = null);
    }
}
