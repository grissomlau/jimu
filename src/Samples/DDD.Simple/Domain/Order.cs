using System;
using DDD.Core;
using DDD.Simple.Domain.Events;

namespace DDD.Simple.Domain
{
    public class Order : AggregateRoot<Guid>
    {

        public decimal TotalAmount { get; private set; }
        public DateTime CreateTime { get; private set; }

        public static Order Load(Guid id, decimal totalAmount, DateTime createTime)
        {
            var order = new Order(id, totalAmount, createTime);
            return order;
        }

        private Order(Guid id, decimal totalAmount, DateTime createTime)
        {
            Id = id;
            TotalAmount = totalAmount;
            CreateTime = createTime;
        }

        public Order(decimal totalAmount)
        {
            ApplyEvent(new OrderPlaced(totalAmount));
        }

        public void ChangeTotalAmount(decimal totalAmount)
        {
            ApplyEvent(new OrderTotalAmountChanged(Id, totalAmount));
        }

        [InlineEventHandler]
        private void HandleOrderPlaced(OrderPlaced e)
        {
            Id = (Guid)e.AggregateRootKey;
            TotalAmount = e.TotalAmount;
            CreateTime = e.Timestamp;
        }

        [InlineEventHandler]
        private void HandleTotalAmountChanged(OrderTotalAmountChanged e)
        {
            TotalAmount = e.TotalAmount;
        }
    }
}
