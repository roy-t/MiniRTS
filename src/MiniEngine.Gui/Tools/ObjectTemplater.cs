﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Tools
{
    public record PropertyTemplate(string Name, Type Type, MethodInfo Getter, MethodInfo? Setter);
    public record ObjectTemplate(Type Type, bool ValueHeader, IReadOnlyList<PropertyTemplate> Properties);

    [Service]
    public sealed class ObjectTemplater
    {
        private readonly Dictionary<Type, ObjectTemplate> Templates;

        public ObjectTemplater()
        {
            this.Templates = new Dictionary<Type, ObjectTemplate>();
            this.RegisterTypeType();
        }

        public ObjectTemplate GetTemplate(Type type)
        {
            if (type.IsAssignableTo(typeof(Type)))
            {
                return this.Templates[typeof(Type)];
            }

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

            var list = properties
                .Where(p => p.GetGetMethod() != null)
                .Select(p => new PropertyTemplate(p.Name, p.PropertyType, p.GetGetMethod()!, p.GetSetMethod()))
                .OrderBy(x => x.Setter == null ? 1 : 0)
                .ThenBy(x => x.Name)
                .ToArray();

            var template = new ObjectTemplate(type, toString, list);
            this.Templates.Add(type, template);

        }

        private void RegisterTypeType()
        {
            var type = typeof(Type);
            var properties = new[]
            {
                new PropertyTemplate("Name", typeof(string), type.GetProperty("Name")!.GetGetMethod()!, null)
            };

            var template = new ObjectTemplate(typeof(Type), true, properties);
            this.Templates.Add(type, template);
        }
    }
}
