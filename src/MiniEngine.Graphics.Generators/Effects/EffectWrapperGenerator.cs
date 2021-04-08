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
            var file = new Source.File($"{name}.generated.cs");
            file.Usings.Add(new Using("Microsoft.Xna.Framework"));
            file.Usings.Add(new Using("Microsoft.Xna.Framework.Graphics"));
            file.Usings.Add(new Using("MiniEngine.Graphics.Effects"));

            var @namespace = new Namespace("MiniEngine.Graphics.Generated");
            file.Namespaces.Add(@namespace);

            var @class = new Class(name, "public", "sealed");
            @namespace.Classes.Add(@class);
            @class.InheritsFrom.Add("EffectWrapper");

            var constructor = new Constructor(@class.Name, "public");
            @class.Constructors.Add(constructor);
            constructor.Parameters.Add("EffectFactory", "factory");
            constructor.Chain = new Optional<IConstructorChainCall>(new BaseConstructorCall($"factory.Load<{name}>()"));

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

            // TODO: beautify and generate fields for each technique so we don't have to look it up!
            if (effect.Techniques.Count == 1)
            {
                var method = new Method("void", "Apply", "public");
                @class.Methods.Add(method);

                method.Body.Expressions.Add(new Statement($"this.Effect.Techniques[\"{effect.Techniques[0]}\"].Passes[0].Apply()"));
            }
            else
            {
                foreach (var technique in effect.Techniques)
                {
                    var method = new Method("void", $"Apply{technique}", "public");
                    @class.Methods.Add(method);

                    method.Body.Expressions.Add(new Statement($"this.Effect.Techniques[\"{technique}\"].Passes[0].Apply()"));
                }
            }

            return file;
        }
    }
}
