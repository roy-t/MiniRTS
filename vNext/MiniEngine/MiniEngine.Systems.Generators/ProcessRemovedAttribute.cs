using System;

namespace MiniEngine.Systems.Generators
{
    /// <summary>
    /// Processes all removed entities that have the parameterized components
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ProcessRemovedAttribute : Attribute { }
}
