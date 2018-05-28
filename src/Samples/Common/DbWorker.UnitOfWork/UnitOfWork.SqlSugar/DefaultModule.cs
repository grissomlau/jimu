using Autofac;
using Sugar = SqlSugar;

namespace DbWorker.UnitOfWork.SqlSugar
{
    public class DefaultModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork.IUnitOfWork>().AsImplementedInterfaces().InstancePerLifetimeScope();
        }
    }
}
