using DDD.Core;

namespace DDD.Simple.Domain.Events
{
    public class OrderTotalAmountChanged : DomainEvent
    {
        public decimal TotalAmount { get; set; }
        public OrderTotalAmountChanged(object aggregateRootKey, decimal totalAmount) : base(aggregateRootKey)
        {
            TotalAmount = totalAmount;
        }
    }
}
