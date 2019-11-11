namespace Jimu.Diagnostic
{
    public interface IJimuDiagnostic
    {
        void Write(string name, object value);
        bool IsEnabled(string name);

        string ListenerName { get; }
    }
}
