﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LightInject;
using Serilog;

/// <summary> Adapter for the LightInject dependency injector. Scans all assemblies in the current
/// directory for types annotated with one of the attributes defined in this namespace and makes
/// these available for injection. In MiniEngine dependency injection is mostly used for constructor
/// injection. As it is relatively expensive. But you can also use late binding by injecting a
/// factory (Func<>) or one of the Resolve or Register delegates. This should not be done inside the
/// main game loop. </summary>
#pragma warning disable IDE0039 // Use local function

namespace MiniEngine.Configuration
{
    public delegate object Resolve(Type type);

    public delegate void Register(object instance);

    public delegate void RegisterAs(object instance, Type type);

    public sealed class Injector : IDisposable
    {
        private static readonly string[] IgnoredAssemblies = new[]
        {
            "Microsoft", "MonoGame", "MiniEngine.ContentPipeline", "Serilog", "SharpDX", "LightInject", "ImGui.NET"
        };

        private readonly ServiceContainer Container;
        private readonly ILogger Logger;

        public Injector()
        {
            this.Container = new ServiceContainer();
            this.Container.SetDefaultLifetime<PerContainerLifetime>();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .WriteTo.Debug()
                .CreateLogger();

            this.Container.RegisterInstance(Log.Logger);
            this.Logger = Log.Logger;

            Resolve resolveDelegate = type => this.Container.GetInstance(type);
            this.Container.RegisterInstance(resolveDelegate);

            Register registerDelegate = o => this.Container.RegisterInstance(o.GetType(), o);
            this.Container.RegisterInstance(registerDelegate);

            RegisterAs registerAsDelgate = (o, t) => this.Container.RegisterInstance(t, o);
            this.Container.RegisterInstance(registerAsDelgate);

            this.RegisterTypesFromAssembliesInWorkingDirectory();
        }

        public T Get<T>()
         where T : class
         => this.Container.Create<T>();

        public void Dispose()
            => this.Container?.Dispose();

        private void RegisterTypesFromAssembliesInWorkingDirectory()
        {
            var assemblies = this.LoadAssembliesInCurrentDirectory();

            var componentTypes = new List<Type>();
            Type? containerType = null;

            foreach (var assembly in assemblies)
            {
                this.Container.RegisterAssembly(assembly, (serviceType, concreteType) =>
                {
                    // Do not register services as an implementation of an abstract class or interface
                    if (serviceType != concreteType)
                    {
                        return false;
                    }

                    if (IsComponentType(concreteType))
                    {
                        componentTypes.Add(concreteType);
                        this.Logger.Debug("Registered component {@component}", concreteType.FullName);
                        return false;
                    }

                    if (IsContainerType(concreteType))
                    {
                        containerType = concreteType;
                        this.Logger.Debug("Registered container {@container}", concreteType.FullName);
                        return false;
                    }

                    if (IsServiceType(concreteType))
                    {
                        this.Logger.Debug("Registered service {@service}", concreteType.FullName);
                        return true;
                    }

                    if (IsContentType(concreteType))
                    {
                        this.Logger.Debug("Registered content {@content}", concreteType.FullName);
                        return true;
                    }

                    return false;
                });
            }

            if (containerType == null)
            {
                throw new Exception("Could not find any suitable containers");
            }

            this.RegisterComponentContainers(containerType, componentTypes);

            this.Logger.Information("Registered {@count} services", this.Container.AvailableServices.Count());
        }

        private IEnumerable<Assembly> LoadAssembliesInCurrentDirectory()
        {
            var cwd = Directory.GetCurrentDirectory();
            this.Logger.Information("Loading assemblies from {@directory}", cwd);

            var assemblies = new List<Assembly>();
            foreach (var file in Directory.EnumerateFiles(cwd, "*.dll"))
            {
                try
                {
                    var assemblyName = AssemblyName.GetAssemblyName(file);
                    if (IsRelevantAssembly(assemblyName))
                    {
                        this.Logger.Information("Loading {@assembly}", assemblyName.FullName);
                        var assembly = Assembly.LoadFrom(file);
                        assemblies.Add(assembly);
                    }
                    else
                    {
                        this.Logger.Debug("Ignoring {@assembly} as its not a relevant assembly", assemblyName.FullName);
                    }
                }
                catch (BadImageFormatException)
                {
                    this.Logger.Debug("Ignoring {@file} as its not a .NET assembly", file);
                }
            }

            return assemblies;
        }

        private static bool IsServiceType(Type type)
            => (type.IsDefined(typeof(ServiceAttribute), true) || type.IsDefined(typeof(SystemAttribute), true)) && !type.IsAbstract;

        private static bool IsComponentType(Type type)
            => type.IsDefined(typeof(ComponentAttribute), true) && !type.IsAbstract;

        private static bool IsContainerType(Type type)
            => type.IsDefined(typeof(ComponentContainerAttribute), true) && !type.IsAbstract;

        private static bool IsContentType(Type type)
            => type.IsDefined(typeof(ContentAttribute), true) && !type.IsAbstract;

        private static bool IsRelevantAssembly(AssemblyName name)
            => !IgnoredAssemblies.Any(n => name.FullName.StartsWith(n));

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

            foreach (var interfaceType in containerType.GetInterfaces())
            {
                this.Container.RegisterInstance(interfaceType, instance);
            }

            this.Container.RegisterInstance(containerType, instance);
        }
    }
}

#pragma warning restore IDE0039 // Use local function
