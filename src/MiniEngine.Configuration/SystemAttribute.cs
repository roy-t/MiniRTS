using System;

namespace MiniEngine.Configuration
{
    /// <summary>
    /// Marks the class as a system for the injector
    /// </summary>
    /// <seealso cref="Injector"/>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SystemAttribute : Attribute
    {
    }
}
