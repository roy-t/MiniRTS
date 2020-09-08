using System;
using System.Reflection;
using LightInject;

#pragma warning disable IDE0039 // Use local function
namespace MiniEngine.Systems.Injection
{
    public delegate object Resolve(Type type);
    public delegate void Register(object instance);

    public sealed class Injector : IDisposable
    {
        private readonly ServiceContainer Container;

        public Injector()
        {
            this.Container = new ServiceContainer(ContainerOptions.Default);
            this.Container.SetDefaultLifetime<PerContainerLifetime>();


            Resolve resolveDelegate = type => this.Container.Create(type);
            this.Container.RegisterInstance(resolveDelegate);

            Register registerDelegate = o => this.Container.RegisterInstance(o.GetType(), o);
            this.Container.RegisterInstance(registerDelegate);

            var me = Assembly.GetEntryAssembly();
            if (me != null)
            {
                this.Container.RegisterAssembly(me, (serviceType, concreteType) =>
                {
                    return concreteType.IsDefined(typeof(ServiceAttribute), false);
                });

                foreach (var assemblyName in me.GetReferencedAssemblies())
                {
                    var assembly = Assembly.Load(assemblyName);
                    this.Container.RegisterAssembly(assembly, (serviceType, concreteType) =>
                    {
                        return concreteType.IsDefined(typeof(ServiceAttribute), false);
                    });
                }
            }
        }

        public T Create<T>()
            where T : class
            => this.Container.Create<T>();

        public void Dispose()
            => this.Container?.Dispose();
    }
}
#pragma warning restore IDE0039 // Use local function
