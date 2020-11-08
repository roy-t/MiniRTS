using System;

namespace MiniEngine.Configuration
{
    /// <summary>
    /// Marks the class as a content class for the injector
    /// </summary>
    /// <seealso cref="Injector"/>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ContentAttribute : Attribute
    {
    }
}
