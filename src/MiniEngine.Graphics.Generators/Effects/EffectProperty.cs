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
            var type = syntax.DescendantNodes().OfType<TypeSyntax>().First().DescendantNodes().OfType<SyntaxToken>().First().ValueText;

            foreach (var variable in syntax.Variables)
            {
                var isArrayType = variable.ArrayRankSpecifiers.Any();
                var name = variable.Identifier.ValueText;
                var register = variable.Qualifiers.OfType<RegisterLocation>().Select(r => r.Register.ValueText).FirstOrDefault();

                yield return new EffectProperty(type, name, isArrayType, register);
            }
        }

        public EffectProperty(string type, string name, bool isArrayType, string register)
        {
            this.Type = type;
            this.Name = name;
            this.IsArrayType = isArrayType;
            this.Register = register;
            this.RegisterIndex = -1;

            if (!string.IsNullOrWhiteSpace(register))
            {
                var digits = register.Where(c => char.IsDigit(c)).ToArray();
                this.RegisterIndex = int.Parse(new string(digits));
            }
        }

        public string Type { get; }
        public string Name { get; }
        public string Register { get; }
        public int RegisterIndex { get; }
        public bool IsArrayType { get; }

        public bool IsSamplerState()
            => this.GetXNAType().Equals("SamplerState");

        public bool HasRegister()
            => this.RegisterIndex >= 0;

        public override string ToString() => $"{this.Type} {this.Name}";

        public string GetXNAType()
        {
            var type = GetIndividualType();
            if (this.IsArrayType)
            {
                return $"{type}[]";
            }

            return type;
        }

        private string GetIndividualType()
        {
            switch (this.Type)
            {
                case "float4x4":
                    return "Matrix";
                case "float":
                    return "float";
                case "float2":
                    return "Vector2";
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
                case "TextureCube":
                    return "TextureCube";
                case "int":
                    return "int";
                case "int2":
                    return "Point";
                case "bool":
                    return "bool";
                case "SamplerComparisonState":
                    return "SamplerState";
                case "sampler":
                    return "SamplerState";
                case "SamplerState":
                    return "SamplerState";
                default:
                    throw new ArgumentException($"Unknown type {this.Type}", this.Type);
            }
        }
    }
}
