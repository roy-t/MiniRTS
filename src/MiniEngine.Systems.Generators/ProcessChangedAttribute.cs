using System;

namespace MiniEngine.Systems.Generators
{
    /// <summary>
    /// Processes all changed entities that have the parameterized components
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ProcessChangedAttribute : Attribute { }
}
