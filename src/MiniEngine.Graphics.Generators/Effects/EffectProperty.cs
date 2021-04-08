using System;
using System.Linq;
using ShaderTools.CodeAnalysis.Hlsl.Syntax;

namespace MiniEngine.Graphics.Generators.Effects
{
    public sealed class EffectProperty
    {
        public EffectProperty(VariableDeclarationSyntax syntax)
        {
            this.Type = Effect.BreadthFirstTypeSearch<SyntaxToken>(syntax.Type).First().ValueText;
            this.Name = syntax.Variables[0].Identifier.ValueText;

            if (this.Type == "" || this.Name == "")
            {

            }
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
