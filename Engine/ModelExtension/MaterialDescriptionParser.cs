using System;
using System.Collections.Generic;
using System.IO;

namespace ModelExtension
{
    public sealed class MaterialDescriptionParser
    {
        private readonly string BasePath;
        private const string InitSectionKey = "init";
        private const string MappingSectionKey = "mapping";
        private readonly IniParser IniParser;

        public MaterialDescriptionParser(string modelPath)
        {
            this.BasePath = Path.GetDirectoryName(modelPath);
            this.IniParser = new IniParser();
        }

        public Dictionary<string, MaterialDescription> Parse(string file)
        {
            var data = this.IniParser.ReadFile(file);

            if (!data.Sections.ContainsKey(MappingSectionKey))
            {
                throw new Exception($"Missing required section marked with [{MappingSectionKey}]");
            }

            var config = ParserConfiguration.Default;
            if (data.Sections.ContainsKey(InitSectionKey))
            {
                config = ParserConfiguration.Parse(data.Sections[InitSectionKey]);
            }

            var lookup = new MaterialTypeLookUp(config);
            var dictionary = new Dictionary<string, MaterialDescription>();
            foreach (var tuple in data.Sections[MappingSectionKey].Properties)
            {
                var diffuse = Path.GetFullPath(Path.Combine(this.BasePath, config.RelativePath, tuple.Key));

                string normal = null;
                if (lookup.TryGet(MaterialType.Normal, tuple.Value, out var normalLookUp))
                {
                    normal = Path.GetFullPath(Path.Combine(this.BasePath, config.RelativePath, normalLookUp));
                }

                string specular = null;
                if (lookup.TryGet(MaterialType.Specular, tuple.Value, out var specularLookUp))
                {
                    specular = Path.GetFullPath(Path.Combine(this.BasePath, config.RelativePath, specularLookUp));
                }

                string mask = null;
                if (lookup.TryGet(MaterialType.Mask, tuple.Value, out var maskLookuUp))
                {
                    mask = Path.GetFullPath(Path.Combine(this.BasePath, config.RelativePath, maskLookuUp));
                }

                string reflection = null;
                if (lookup.TryGet(MaterialType.Reflection, tuple.Value, out var reflectionLookup))
                {
                    reflection = Path.GetFullPath(Path.Combine(this.BasePath, config.RelativePath, reflectionLookup));
                }

                dictionary.Add(diffuse, new MaterialDescription(diffuse, normal, specular, mask, reflection));
            }

            return dictionary;
        }

        private class MaterialTypeLookUp
        {
            private readonly Dictionary<MaterialType, int> Indices;
            public MaterialTypeLookUp(ParserConfiguration config)
            {
                this.Indices = new Dictionary<MaterialType, int>();
                for (var i = 0; i < config.Values.Length; i++)
                {
                    this.Indices.Add(config.Values[i], i);
                }
            }

            public bool TryGet(MaterialType type, string values, out string value)
            {
                value = string.Empty;
                if (this.Indices.TryGetValue(type, out var index))
                {
                    var split = values.Split(',');
                    if (index < split.Length)
                    {
                        value = split[index];
                    }
                }

                return !string.IsNullOrEmpty(value);
            }
        }


        private class ParserConfiguration
        {
            private static readonly string RelativePathKey = "relative_path";
            private static readonly string ValuesKey = "values";


            private ParserConfiguration(string relativePath, MaterialType[] values)
            {
                this.RelativePath = relativePath;
                this.Values = values;
            }

            public string RelativePath { get; private set; }
            public MaterialType[] Values { get; private set; }

            public static ParserConfiguration Default
            {
                get
                {
                    var relativePath = string.Empty;
                    var values = new[]
                    {
                        MaterialType.Normal,
                        MaterialType.Specular,
                        MaterialType.Mask,
                        MaterialType.Reflection,
                    };

                    return new ParserConfiguration(relativePath, values);
                }
            }


            public static ParserConfiguration Parse(Section initSection)
            {
                var config = Default;

                if (initSection.Properties.ContainsKey(RelativePathKey))
                {
                    config.RelativePath = initSection.Properties[RelativePathKey];
                }

                if (initSection.Properties.ContainsKey(ValuesKey))
                {
                    var valuesString = initSection.Properties[ValuesKey].Split(',');
                    var valueArray = new MaterialType[valuesString.Length];
                    for (var i = 0; i < valuesString.Length; i++)
                    {
                        valueArray[i] = ParseMaterialTypeEnum(valuesString[i]);
                    }

                    config.Values = valueArray;
                }

                return config;
            }

            private static MaterialType ParseMaterialTypeEnum(string name)
            {
                if (Enum.TryParse(name, true, out MaterialType type))
                {
                    return type;
                }

                throw new Exception($"Could not parse {name} as {typeof(MaterialType).FullName}");
            }
        }

        public enum MaterialType
        {
            Diffuse,
            Normal,
            Specular,
            Mask,
            Reflection
        }
    }
}
