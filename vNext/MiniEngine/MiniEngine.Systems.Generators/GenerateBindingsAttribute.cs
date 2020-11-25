using System;

namespace MiniEngine.Systems.Generators
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GenerateBindingsAttribute : Attribute { }
}
