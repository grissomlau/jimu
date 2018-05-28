using System;
using DDD.Core;
using SqlSugar;

namespace DDD.Simple.Model
{
    [SugarTable("Order")]
    public class Order : IDbModel<Guid>
    {
        [SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; }
        public decimal TotalAmount { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
