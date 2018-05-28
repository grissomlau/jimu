using System;
using DDD.Core;

namespace DDD.Simple.Domain.Events
{
    public class OrderPlaced : DomainEvent
    {
        public decimal TotalAmount { get; set; }
        public OrderPlaced(decimal totalAmount) : base(Guid.NewGuid())
        {
            TotalAmount = totalAmount;
        }
    }
}
