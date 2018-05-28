using System;
using System.ComponentModel.DataAnnotations.Schema;
using DDD.Core;
using SqlSugar;

namespace DDD.Simple.Model
{
    [Table("UserFriend")]
    [SugarTable("UserFriend")]
    public class UserFriend : IDbModel<Guid>
    {
        [SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid FriendUserId { get; set; }
    }
}
