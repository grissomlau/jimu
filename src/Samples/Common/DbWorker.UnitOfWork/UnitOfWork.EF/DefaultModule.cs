using Autofac;

namespace DbWorker.UnitOfWork.EF
{
    public class DefaultModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork.IUnitOfWork>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<DefaultDbContext>().SingleInstance();
        }
    }
}
