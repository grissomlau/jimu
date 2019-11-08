namespace Jimu.APM
{
    public interface IJimuApm
    {
        void Write(string name, object value);
        bool IsEnabled(string name);

        string ListenerName { get; }
    }
}
