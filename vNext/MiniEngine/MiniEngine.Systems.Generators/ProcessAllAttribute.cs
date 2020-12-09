using System;

namespace MiniEngine.Systems.Generators
{
    /// <summary>
    /// Processes all entities that have the parameterized components
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ProcessAllAttribute : Attribute { }
}
