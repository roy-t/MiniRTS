using System;

namespace MiniEngine.Systems.Injection
{
    /// <summary>
    /// Marks the class as a service for the injector
    /// </summary>
    /// <seealso cref="Injector"/>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ServiceAttribute : Attribute
    {
    }
}
