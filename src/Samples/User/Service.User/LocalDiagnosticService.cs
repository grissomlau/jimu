using IService.User;
using Jimu.Diagnostic;
using Jimu.Server.Diagnostic;
using Jimu;

namespace Service.User
{
    public class LocalDiagnosticService : ILocalDiagnosticService
    {
        private readonly IJimuDiagnostic _jimuDiagnostic;
        private readonly JimuPayload _payload;
        public LocalDiagnosticService(IJimuDiagnostic jimuDiagnostic, JimuPayload payload)
        {
            _jimuDiagnostic = jimuDiagnostic;
            _payload = payload;
        }
        public void Get(string name)
        {
            //-- task1
            var operationId = _jimuDiagnostic.WriteLocalMethodInvokeBefore(_payload, $"begin task1", "task1");
            //todo: something long task1
            _jimuDiagnostic.WriteLocalMethodInvokeAfter(operationId, $"end task1", "success");


            //-- task2
            var operationId2 = _jimuDiagnostic.WriteLocalMethodInvokeBefore(_payload, $"begin task2", "task2");
            //todo: something long task2
            _jimuDiagnostic.WriteLocalMethodInvokeAfter(operationId2, $"end task2", "success");

        }
    }
}
