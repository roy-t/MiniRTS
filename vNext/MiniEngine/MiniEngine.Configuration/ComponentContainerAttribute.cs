using System;

namespace MiniEngine.Configuration
{
    // In the future we might want to expand the ComponentContainerAttribute with filters so we can define a container for a specific type of components

    /// <summary>
    /// Marks the class as a type that is a container for components
    /// </summary>
    /// <seealso cref="Injector"/>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ComponentContainerAttribute : Attribute
    {
    }
}
