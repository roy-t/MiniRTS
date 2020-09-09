using System;

namespace MiniEngine.Configuration
{
    /// <summary>
    /// Marks the class as a component, the injector will make sure a suitable container is created for the component
    /// </summary>
    /// <seealso cref="Injector"/>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ComponentAttribute : Attribute
    {
    }
}
