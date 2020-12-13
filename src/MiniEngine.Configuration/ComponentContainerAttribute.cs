using System;

namespace MiniEngine.Configuration
{
    /// <summary>
    /// Marks the class as a type that is a container for components
    /// </summary>
    /// <seealso cref="Injector"/>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ComponentContainerAttribute : Attribute
    {
    }
}
