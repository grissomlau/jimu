using Jimu.Logger;
using Polly;
using System.Threading.Tasks;

namespace Jimu.Client.FaultTolerant
{
    public class RetryCallMiddleware
    {
        private readonly ClientRequestDel _next;
        private int _retryTimes;
        private readonly IAddressSelector _addressSelector;
        private readonly ILogger _logger;
        public RetryCallMiddleware(ClientRequestDel next, IAddressSelector addressSelector, ILogger logger, int retryTimes)
        {
            _next = next;
            _retryTimes = retryTimes;
            _addressSelector = addressSelector;
            _logger = logger;
        }

        public Task<JimuRemoteCallResultData> InvokeAsync(RemoteCallerContext context)
        {
            if (_retryTimes <= 0)
            {
                _retryTimes = context.Service.Address.Count;
            }
            var retryPolicy = Policy.Handle<TransportException>()
                .RetryAsync(_retryTimes,
                    async (ex, count) =>
                    {
                        context.ServiceAddress = await _addressSelector.GetAddressAsyn(context.Service);
                        _logger.Debug(
                            $"FaultHandling,retry times: {count},serviceId: {context.Service.ServiceDescriptor.Id},Address: { context.ServiceAddress.Code},RemoteServiceCaller execute retry by Polly for exception {ex.Message}");
                    });
            var fallbackPolicy = Policy<JimuRemoteCallResultData>
                .Handle<TransportException>()
                .FallbackAsync(new JimuRemoteCallResultData()
                {
                    ErrorCode = "500",
                    ErrorMsg = "error occur when communicate with server. server maybe have been down."
                })
                .WrapAsync(retryPolicy);

            return fallbackPolicy.ExecuteAsync(() =>
            {
                return _next(context);
            });
        }
    }
}
