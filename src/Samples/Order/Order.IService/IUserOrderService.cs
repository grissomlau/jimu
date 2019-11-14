using Jimu;
using System;

namespace Order.IService
{
    [JimuServiceRoute("/order/{Service}")]
    public interface IUserOrderService : IJimuService
    {
        [JimuGet(true, Rest = "/")]
        string GetOrder();

        [JimuPost(false, Rest = "/")]
        string PostOrder(int id);
    }
}
