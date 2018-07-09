using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Jimu.Client
{
    public class DefaultTransportClient : ITransportClient, IDisposable
    {
        private readonly IClientListener _listener;
        private readonly ILogger _logger;

        private readonly ConcurrentDictionary<string, TaskCompletionSource<JimuTransportMsg>> _resultCallbackDic =
            new ConcurrentDictionary<string, TaskCompletionSource<JimuTransportMsg>>();

        //private readonly ISerializer _serializer;
        private readonly IClientSender _sender;

        public DefaultTransportClient(IClientListener listener, IClientSender sender,
            ILogger logger /*, ISerializer serializer*/)
        {
            _logger = logger;
            //this._serializer = serializer;
            _listener = listener;
            _sender = sender;

            _listener.OnReceived += ListenerOnReceived;
        }

        public void Dispose()
        {
            (_sender as IDisposable)?.Dispose();
            ((IDisposable)_listener)?.Dispose();
            foreach (var task in _resultCallbackDic.Values) task.TrySetCanceled();
        }

        public async Task<JimuRemoteCallResultData> SendAsync(JimuRemoteCallData data)
        {
            try
            {
                _logger.Debug($"prepare sending: {data.ServiceId}");
                var transportMsg = new JimuTransportMsg(data);
                var callbackTask = RegisterResultCallbackAsync(transportMsg.Id);
                try
                {
                    await _sender.SendAsync(transportMsg);
                    _logger.Debug($"succed to send: {data.ServiceId}, msg: {transportMsg.Id}");
                    return await callbackTask;
                }
                catch (Exception ex)
                {
                    _logger.Error($"error occur when connecting with server, serviceid: {data.ServiceId}", ex);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"failed to send: {data.ServiceId}", ex);
                throw new TransportException(ex.Message, ex);
            }
        }

        private Task ListenerOnReceived(IClientSender sender, JimuTransportMsg message)
        {
            _logger.Debug($"receive response of msg: {message.Id}");
            if (!_resultCallbackDic.TryGetValue(message.Id, out var task))
            {
                return Task.CompletedTask;
            }

            if (message.ContentType != typeof(JimuRemoteCallResultData).FullName) return Task.CompletedTask;
            task.SetResult(message);
            return Task.CompletedTask;
        }

        private async Task<JimuRemoteCallResultData> RegisterResultCallbackAsync(string id)
        {
            var task = new TaskCompletionSource<JimuTransportMsg>();
            _resultCallbackDic.TryAdd(id, task);
            try
            {
                var result = await task.Task;
                return result.GetContent<JimuRemoteCallResultData>();
            }
            finally
            {
                _resultCallbackDic.TryRemove(id, out _);
            }
        }
    }
}