using System.Collections.Generic;

namespace Jimu.Server.Bus.MassTransit.RabbitMq.Pattern
{
    internal class PatternProvider
    {
        IPattern _command = new CommandPattern();
        IPattern _publicSubscribe = new PublicSubscribePattern();
        IPattern _requestResponse = new RequestResponsePattern();
        public IEnumerable<IPattern> GetPatterns()
        {
            yield return _publicSubscribe;
            yield return _command;
            yield return _requestResponse;
        }
    }
}
