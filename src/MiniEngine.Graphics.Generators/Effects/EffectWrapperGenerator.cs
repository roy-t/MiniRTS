using System.IO;
using Microsoft.CodeAnalysis;
using MiniEngine.Graphics.Generators.Source;

namespace MiniEngine.Graphics.Generators.Effects
{
    public sealed class EffectWrapperGenerator
    {
        public Source.File Generate(Effect effect)
        {
            var name = Path.GetFileNameWithoutExtension(effect.Name);
            var file = CreateFile($"{name}.generated.cs");
            var @namespace = CreateNamespace(file);
            var @class = CreateClass(name, @namespace);
            var constructor = CreateConstructor(name, @class);
            AddAndInitializeProperties(effect, @class, constructor);
            AddTechniques(effect, @class, constructor);

            return file;
        }

        private static Source.File CreateFile(string name)
        {
            var file = new Source.File(name);
            file.Usings.Add(new Using("Microsoft.Xna.Framework"));
            file.Usings.Add(new Using("Microsoft.Xna.Framework.Graphics"));
            file.Usings.Add(new Using("MiniEngine.Graphics.Effects"));
            return file;
        }

        private static Namespace CreateNamespace(Source.File file)
        {
            var @namespace = new Namespace("MiniEngine.Graphics.Generated");
            file.Namespaces.Add(@namespace);
            return @namespace;
        }

        private static Class CreateClass(string name, Namespace @namespace)
        {
            var @class = new Class(name, "public", "sealed");
            @namespace.Classes.Add(@class);
            @class.InheritsFrom.Add("EffectWrapper");
            return @class;
        }

        private static Constructor CreateConstructor(string name, Class @class)
        {
            var constructor = new Constructor(@class.Name, "public");
            @class.Constructors.Add(constructor);
            constructor.Parameters.Add("EffectFactory", "factory");
            constructor.Chain = new Optional<IConstructorChainCall>(new BaseConstructorCall($"factory.Load<{name}>()"));
            return constructor;
        }

        private static void AddAndInitializeProperties(Effect effect, Class @class, Constructor constructor)
        {
            foreach (var prop in effect.PublicProperties)
            {
                var fieldName = prop.Name + "Parameter";
                @class.Fields.Add(new Field("EffectParameter", fieldName, "private", "readonly"));
                constructor.Body.Expressions.Add(new Assignment($"this.{fieldName}", "=", $"this.Effect.Parameters[\"{prop.Name}\"]"));

                var property = new Property(prop.GetXNAType(), prop.Name, false, "public");
                @class.Properties.Add(property);
                var propertySetter = new Body();
                property.SetSetter(propertySetter);
                propertySetter.Expressions.Add(new Statement($"this.{fieldName}.SetValue(value)"));
            }
        }

        private static void AddTechniques(Effect effect, Class @class, Constructor constructor)
        {
            if (effect.Techniques.Count == 1)
            {
                AddTechnique(@class, constructor, effect.Techniques[0], "Apply");
            }
            else
            {
                foreach (var technique in effect.Techniques)
                {
                    AddTechnique(@class, constructor, technique, $"Apply{SourceUtilities.CapitalizeFirstLetter(technique)}");
                }
            }
        }

        private static void AddTechnique(Class @class, Constructor constructor, string technique, string methodName)
        {
            var field = new Field("EffectPass", $"{SourceUtilities.CapitalizeFirstLetter(technique)}Pass", "private", "readonly");
            @class.Fields.Add(field);

            constructor.Body.Expressions.Add(new Assignment($"this.{field.Name}", "=", $"this.Effect.Techniques[\"{technique}\"].Passes[0]"));

            var method = new Method("void", methodName, "public");
            @class.Methods.Add(method);

            method.Body.Expressions.Add(new Statement($"this.{field.Name}.Apply()"));
        }
    }
}
