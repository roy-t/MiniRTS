using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Tools
{
    public record PropertyTemplate(string Name, Type Type, MethodInfo? Getter, MethodInfo? Setter);
    public record ObjectTemplate(Type Type, bool ValueHeader, IReadOnlyList<PropertyTemplate> Properties);

    [Service]
    public sealed class ObjectTemplater
    {
        private readonly Dictionary<Type, ObjectTemplate> Templates;

        public ObjectTemplater()
        {
            this.Templates = new Dictionary<Type, ObjectTemplate>();
        }

        public ObjectTemplate GetTemplate(Type type)
        {
            if (!this.Templates.ContainsKey(type))
            {
                this.RegisterObject(type);
            }

            return this.Templates[type];
        }

        private void RegisterObject(Type type)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Except(type.GetDefaultMembers().OfType<PropertyInfo>());

            var toString = type.GetMethod("ToString", Array.Empty<Type>())?.DeclaringType == type;

            var list = properties.Select(p => new PropertyTemplate(p.Name, p.PropertyType, p.GetGetMethod(), p.GetSetMethod())).ToArray();
            var template = new ObjectTemplate(type, toString, list);
            this.Templates.Add(type, template);
        }
    }
}
