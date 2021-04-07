using System;
using ShaderTools.CodeAnalysis.Hlsl.Syntax;

namespace MiniEngine.Graphics.Generators.Effects
{
    public sealed class EffectProperty
    {
        public EffectProperty(VariableDeclarationSyntax syntax)
        {
            this.Type = syntax.Type.ToString();
            this.Name = syntax.Variables[0].Identifier.ToString();
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

                default:
                    throw new ArgumentException($"Unknown type {this.Type}", this.Type);
            }
        }
    }
}
