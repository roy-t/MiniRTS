using System;

namespace MiniEngine.Systems.Generators
{
    /// <summary>
    /// Processes all new entities that have the parameterized components
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ProcessNewAttribute : Attribute { }
}
