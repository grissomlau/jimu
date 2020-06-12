using IService.Order;
using Jimu;
using Jimu.Client.Proxy;
using IService.User;

namespace Service.Order
{
    public class UserOrderService : IUserOrderService
    {
        readonly IServiceProxy _serviceProxy;
        readonly JimuPayload _payload;
        public UserOrderService(IServiceProxy serviceProxy, JimuPayload payload)
        {
            _serviceProxy = serviceProxy;
            _payload = payload;
        }
        public string GetOrder()
        {
            IHelloWorldService service = _serviceProxy.GetService<IHelloWorldService>(_payload);
            var ret = service.Get();
            return $"get order sucess, User service return: {ret}";
        }

        public string PostOrder(int id)
        {
            return $"post order {id} success";
        }
    }
}
