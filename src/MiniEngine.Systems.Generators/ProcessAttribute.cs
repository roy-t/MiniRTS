using System;

namespace MiniEngine.Systems.Generators
{
    /// <summary>
    /// Processor method that is called once, can only be placed on a method with zero parameters
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ProcessAttribute : Attribute { }
}
