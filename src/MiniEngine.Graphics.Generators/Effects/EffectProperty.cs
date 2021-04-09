using System;
using System.Collections.Generic;
using System.Linq;
using ShaderTools.CodeAnalysis.Hlsl.Syntax;
using ShaderTools.CodeAnalysis.Syntax;

namespace MiniEngine.Graphics.Generators.Effects
{
    public sealed class EffectProperty
    {
        public static IEnumerable<EffectProperty> Parse(VariableDeclarationSyntax syntax)
        {
            var type = syntax.Type.DescendantNodes().OfType<SyntaxToken>().First().ValueText;
            return syntax.Variables
                .Select(variable => variable.Identifier.ValueText)
                .Select(name => new EffectProperty(type, name));
        }

        public EffectProperty(string type, string name)
        {
            this.Type = type;
            this.Name = name;
        }

        public string Type { get; }
        public string Name { get; }

        public override string ToString() => $"{this.Type} {this.Name}";

        public string GetXNAType()
        {
            switch (this.Type)
            {
                case "float4x4":
                    return "Matrix";
                case "float":
                    return "float";
                case "float2":
                    return "Vector3";
                case "float3":
                    return "Vector3";
                case "float4":
                    return "Vector4";
                case "texture":
                    return "Texture2D";
                case "Texture2D":
                    return "Texture2D";
                case "Texture2DArray":
                    return "Texture2D";
                case "int":
                    return "int";
                case "int2":
                    return "Point";
                case "bool":
                    return "bool";
                case "SamplerComparisonState":
                    return "int[]";
                default:
                    throw new ArgumentException($"Unknown type {this.Type}", this.Type);
            }
        }
    }
}
