using Jimu;
using System;

namespace IService.Order
{
    //[Jimu("/order/{Service}")]
    [Jimu]
    public interface IUserOrderService : IJimuService
    {
        [JimuGet(true, Rest = "/")]
        string GetOrder();

        [JimuPost(false, Rest = "/")]
        string PostOrder(int id);
    }
}
