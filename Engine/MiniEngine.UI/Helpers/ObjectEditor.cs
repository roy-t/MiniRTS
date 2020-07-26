using System;
using System.Reflection;
using MiniEngine.Systems.Annotations;

namespace MiniEngine.UI.Helpers
{
    public static class ObjectEditor
    {

        public static void Create(Editors editors, object component)
        {
            var componentType = component.GetType();

            var properties = componentType.GetProperties();
            for (var i = 0; i < properties.Length; i++)
            {
                var property = properties[i];

                var attributes = property.GetCustomAttributes(typeof(EditorAttribute), false);
                for (var a = 0; a < attributes.Length; a++)
                {
                    if (attributes[a] is EditorAttribute attribute)
                    {
                        var getter = GetGetter(property, component);
                        var setter = GetSetter(attribute.Setter, component, componentType) ?? GetSetter(property, component);

                        var index = 0;
                        if (!string.IsNullOrEmpty(attribute.IndexProperty))
                        {
                            index = (int)component.GetType().GetProperty(attribute.IndexProperty).GetGetMethod().Invoke(component, null);
                        }

                        editors.Create(attribute.Name, getter(), attribute.MinMax, setter, index);
                    }
                }
            }
        }

        private static Func<object> GetGetter(PropertyInfo property, object component)
        {
            if (property != null)
            {
                return () => property.GetGetMethod().Invoke(component, null);
            }

            return null;
        }

        private static Action<object> GetSetter(string name, object component, Type componentType)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            var method = componentType.GetMethod(name);
            if (method != null)
            {
                return o => method.Invoke(component, new object[] { o });
            }

            return null;
        }

        private static Action<object> GetSetter(PropertyInfo property, object component)
        {
            if (property != null && property.GetSetMethod() != null)
            {
                return o => property.GetSetMethod().Invoke(component, new object[] { o });
            }

            return null;
        }
    }
}
