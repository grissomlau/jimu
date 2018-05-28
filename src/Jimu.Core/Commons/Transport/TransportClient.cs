using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Jimu.Core.Client.TransportClient;
using Jimu.Core.Commons.Logger;
using Jimu.Core.Protocols;

namespace Jimu.Core.Commons.Transport
{
    public class DefaultTransportClient : ITransportClient, IDisposable
    {
        private readonly IClientListener _listener;
        private readonly ILogger _logger;

        private readonly ConcurrentDictionary<string, TaskCompletionSource<TransportMessage>> _resultCallbackDic =
            new ConcurrentDictionary<string, TaskCompletionSource<TransportMessage>>();

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
            ((IDisposable) _listener)?.Dispose();
            foreach (var task in _resultCallbackDic.Values) task.TrySetCanceled();
        }

        public async Task<RemoteInvokeResultMessage> SendAsync(RemoteInvokeMessage message)
        {
            try
            {
                _logger.Info($"prepare sending :{message.ServiceId}");
                var transportMsg = TransportMessage.Create(message);
                var callbackTask = RegisterResultCallbackAsync(transportMsg.Id);
                try
                {
                    await _sender.SendAsync(transportMsg);
                    _logger.Info($"succed to  send :{message.ServiceId}");
                    return await callbackTask;
                }
                catch (Exception ex)
                {
                    _logger.Error("error occur when connecting with server", ex);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"failed to send:{message.ServiceId}", ex);
                throw;
            }
        }

        private Task ListenerOnReceived(IClientSender sender, TransportMessage message)
        {
            _logger.Info("received msg, on handing ...");

            if (!_resultCallbackDic.TryGetValue(message.Id, out var task))
                return Task.CompletedTask;

            if (!message.IsInvokeResultMessage()) return Task.CompletedTask;
            task.SetResult(message);
            return Task.CompletedTask;
        }

        private async Task<RemoteInvokeResultMessage> RegisterResultCallbackAsync(string id)
        {
            var task = new TaskCompletionSource<TransportMessage>();
            _resultCallbackDic.TryAdd(id, task);
            try
            {
                var result = await task.Task;
                return result.GetContent<RemoteInvokeResultMessage>();
            }
            finally
            {
                TaskCompletionSource<TransportMessage> value;
                _resultCallbackDic.TryRemove(id, out value);
            }
        }
    }
}