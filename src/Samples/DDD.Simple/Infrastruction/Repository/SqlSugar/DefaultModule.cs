using System;
using Autofac;
using DDD.Core;
using DDD.Simple.Domain;

namespace DDD.Simple.Repository.SqlSugar
{
    public class DefaultModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserRepository>().As<IRepository<User, Guid>>().InstancePerLifetimeScope();
            builder.RegisterType<OrderRepository>().As<IRepository<Order, Guid>>().InstancePerLifetimeScope();
        }

    }
}
