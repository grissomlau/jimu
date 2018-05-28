using System;
using DDD.Core;
using DDD.Simple.Domain;
using DDD.Simple.Domain.Events;
using DbWorker.IUnitOfWork;
using SqlSugar;

namespace DDD.Simple.Repository.SqlSugar
{
    public class OrderRepository : Repository<Order, Guid>
    {
        readonly SqlClient<SqlSugarClient> _sqlClient;
        Model.Order _order;

        public OrderRepository(IUnitOfWork unitOfWork)
        {
            _sqlClient = unitOfWork.GetSqlClient<SqlSugarClient>();
        }

        public override Order Get(Guid key)
        {
            var order = _sqlClient.Client.Queryable<Model.Order>().Where(x => x.Id == key).Single();
            return Order.Load(order.Id, order.TotalAmount, order.CreateTime);

        }

        public override void Save(Order aggreateRoot)
        {
            base.Save(aggreateRoot);
            _order = null;
        }

        [InlineEventHandler]
        private void HandleOrderPlaced(OrderPlaced e)
        {
            var orderModel = GetOrderModel(e);
            orderModel.Id = e.Id;
            orderModel.TotalAmount = e.TotalAmount;
            orderModel.CreateTime = e.Timestamp;
            _sqlClient.Client.Insertable(orderModel).ExecuteCommand();
        }

        private Model.Order GetOrderModel(IDomainEvent e)
        {
            if (_order == null)
            {
                _order = _sqlClient.Client.Queryable<Model.Order>().Where(x => x.Id == (Guid)e.AggregateRootKey).Single();
            }

            return _order ?? (_order = new Model.Order());
        }

    }
}
