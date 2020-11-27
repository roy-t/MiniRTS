using System;

namespace MiniEngine.Systems.Generators
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ProcessRemovedAttribute : Attribute { }
}
