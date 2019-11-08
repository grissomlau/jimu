namespace Jimu.Server.APM
{
    public class ApmServerEventType
    {
        public const string ListenerName = "JimuServerDiagnosticListener";

        public const string ServiceInvokeBefore = "JimuServer.ServiceInvokeBefore";
        public const string ServiceInvokeAfter = "JimuServer.ServiceInvokeAfter";
        public const string ServiceInvokeError = "JimuServer.ServiceInvokeError";

        public const string LocalMethodInvokeBefore = "JimuServer.LocalMethodInvokeBefore";
        public const string LocalMethodInvokeAfter = "JimuServer.LocalMethodInvokeAfter";
        public const string LocalMethodInvokeError = "JimuServer.LocalMethodInvokeError";
    }
}
