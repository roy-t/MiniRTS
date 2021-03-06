﻿using System.IO;
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
            var reloadMethod = CreateReloadMethod(@class);

            AddAndInitializeProperties(effect, @class, reloadMethod);
            AddAndInitializeSamplers(effect, @class, constructor);
            AddTechniques(effect, @class, reloadMethod);

            constructor.Body.Expressions.Add(new Statement("this.Reload()"));

            return file;
        }

        private static Method CreateReloadMethod(Class @class)
        {
            var reloadMethod = new Method("void", "Reload", "protected", "override");
            @class.Methods.Add(reloadMethod);
            return reloadMethod;
        }

        private static Source.File CreateFile(string name)
        {
            var file = new Source.File(name);
            file.Usings.Add(new Using("Microsoft.Xna.Framework"));
            file.Usings.Add(new Using("Microsoft.Xna.Framework.Graphics"));
            file.Usings.Add(new Using("Serilog"));
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
            constructor.Parameters.Add("ILogger", "logger");
            constructor.Parameters.Add("EffectFactory", "factory");
            constructor.Chain = new Optional<IConstructorChainCall>(new BaseConstructorCall("logger", $"factory.Load<{name}>()"));
            return constructor;
        }

        private static void AddAndInitializeProperties(Effect effect, Class @class, Method reloadMethod)
        {

            foreach (var prop in effect.PublicProperties)
            {
                var fieldName = SourceUtilities.LowerCaseFirstLetter(prop.Name) + "Parameter";
                @class.Fields.Add(new Field("EffectParameter", fieldName, "private"));
                var assignment = new Assignment($"this.{fieldName}", "=", $"this.Effect.Parameters[\"{prop.Name}\"]");
                reloadMethod.Body.Expressions.Add(assignment);

                var property = new Property(prop.GetXNAType(), SourceUtilities.CapitalizeFirstLetter(prop.Name), false, "public");
                @class.Properties.Add(property);
                var propertySetter = new Body();
                property.SetSetter(propertySetter);
                propertySetter.Expressions.Add(new Statement($"this.{fieldName}.SetValue(value)"));
            }
        }

        private static void AddAndInitializeSamplers(Effect effect, Class @class, Constructor constructor)
        {
            var graphicsDeviceField = new Field("GraphicsDevice", "Device", "private", "readonly");
            @class.Fields.Add(graphicsDeviceField);
            constructor.Parameters.Add(graphicsDeviceField);
            constructor.Body.Expressions.Add(new Assignment($"this.{graphicsDeviceField.Name}", "=", "device"));

            foreach (var sampler in effect.Samplers)
            {
                var property = new Property(sampler.GetXNAType(), sampler.Name, false, "public");
                @class.Properties.Add(property);
                var propertySetter = new Body();
                property.SetSetter(propertySetter);
                propertySetter.Expressions.Add(new Assignment($"this.{graphicsDeviceField.Name}.SamplerStates[{sampler.RegisterIndex}]", "=", "value"));
            }
        }

        private static void AddTechniques(Effect effect, Class @class, Method reloadMethod)
        {
            if (effect.Techniques.Count == 1)
            {
                AddTechnique(@class, reloadMethod, effect.Techniques[0], "Apply");
            }
            else
            {
                foreach (var technique in effect.Techniques)
                {
                    AddTechnique(@class, reloadMethod, technique, $"Apply{SourceUtilities.CapitalizeFirstLetter(technique)}");
                }
            }
        }

        private static void AddTechnique(Class @class, Method reloadMethod, string technique, string methodName)
        {
            var field = new Field("EffectPass", $"{SourceUtilities.LowerCaseFirstLetter(technique)}Pass", "private");
            @class.Fields.Add(field);

            reloadMethod.Body.Expressions.Add(new Assignment($"this.{field.Name}", "=", $"this.Effect.Techniques[\"{technique}\"].Passes[0]"));

            var method = new Method("void", methodName, "public");
            @class.Methods.Add(method);

            method.Body.Expressions.Add(new Statement($"this.{field.Name}.Apply()"));
        }
    }
}
