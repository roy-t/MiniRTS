using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LightInject;
using Serilog;

#pragma warning disable IDE0039 // Use local function
namespace MiniEngine.Configuration
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

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .WriteTo.Debug()
                .CreateLogger();

            this.Container.RegisterInstance(Log.Logger);

            Resolve resolveDelegate = type => this.Container.Create(type);
            this.Container.RegisterInstance(resolveDelegate);

            Register registerDelegate = o => this.Container.RegisterInstance(o.GetType(), o);
            this.Container.RegisterInstance(registerDelegate);

            this.RegisterTypesFromReferencedAssemblies();

            Log.Logger.Information("Registered {@count} services", this.Container.AvailableServices.Count());
        }

        public T Create<T>()
         where T : class
         => this.Container.Create<T>();

        public void Dispose()
            => this.Container?.Dispose();

        private void RegisterTypesFromReferencedAssemblies()
        {
            var assemblies = LoadAssemblies();

            var componentTypes = new List<Type>();
            Type? containerType = null;

            foreach (var assembly in assemblies)
            {
                this.Container.RegisterAssembly(assembly, (serviceType, concreteType) =>
                {
                    if (IsComponentType(concreteType) && serviceType == concreteType)
                    {
                        componentTypes.Add(concreteType);
                        return false;
                    }

                    if (IsContainerType(concreteType))
                    {
                        containerType = concreteType;
                        return false;
                    }

                    return IsInjectableType(concreteType);
                });
            }

            if (containerType == null)
            {
                throw new Exception("Could not find any suitable containers");
            }

            this.RegisterComponentContainers(containerType, componentTypes);
        }

        private static IEnumerable<Assembly> LoadAssemblies()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                return entryAssembly.GetReferencedAssemblies()
                    .Where(name => name.FullName.Contains("MiniEngine", StringComparison.OrdinalIgnoreCase))
                    .Select(name => Assembly.Load(name))
                    .Append(entryAssembly);
            }

            return Enumerable.Empty<Assembly>();
        }

        private static bool IsInjectableType(Type type) => (type.IsDefined(typeof(ServiceAttribute), true) || type.IsDefined(typeof(SystemAttribute), true)) && !type.IsAbstract;

        private static bool IsComponentType(Type type) => type.IsDefined(typeof(ComponentAttribute), true) && !type.IsAbstract;

        private static bool IsContainerType(Type type) => type.IsDefined(typeof(ComponentContainerAttribute), true) && !type.IsAbstract;

        private void RegisterComponentContainers(Type containerType, List<Type> componentTypes)
        {
            foreach (var componentType in componentTypes)
            {
                this.RegisterContainerFor(containerType, componentType);
            }
        }

        private void RegisterContainerFor(Type containerType, Type componentType)
        {
            containerType = containerType.MakeGenericType(componentType);
            var instance = Activator.CreateInstance(containerType);

            this.Container.RegisterInstance(containerType, instance);
        }
    }
}
#pragma warning restore IDE0039 // Use local function
