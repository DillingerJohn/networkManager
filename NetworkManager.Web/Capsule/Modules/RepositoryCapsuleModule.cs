using Autofac;
using NetworkManager.Core.Data.Repositories;
using NetworkManager.Data.Repositories;
using NetworkManager.Web.References;

namespace NetworkManager.Web.Capsule.Modules
{
    public class RepositoryCapsuleModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(ReferencedAssemblies.Repositories).
                Where(_ => _.Name.EndsWith("Repository")).
                AsImplementedInterfaces().
                InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IRepository<>)).InstancePerDependency();
        }

    }
}