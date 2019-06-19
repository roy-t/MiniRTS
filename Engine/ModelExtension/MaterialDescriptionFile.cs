using System;
using System.Collections.Generic;
using System.IO;

namespace ModelExtension
{
    internal class MaterialDescriptionFile
    {
        private const string Extension = "ini";

        private readonly Dictionary<string, MaterialDescription> Descriptions;

        public MaterialDescriptionFile(string modelPath)
        {
            var dot = Path.GetFullPath(modelPath).LastIndexOf(".", StringComparison.OrdinalIgnoreCase);
            var file = modelPath.Substring(0, dot) + "." + Extension;

            if (!File.Exists(file))
            {
                throw new FileNotFoundException($"Could not find file {file}", file);
            }

            var parser = new MaterialDescriptionParser(modelPath);
            this.Descriptions = parser.Parse(file);
        }

        public bool TryGetValue(string diffuseTexturePath, out MaterialDescription description)
        {
            var path = Path.GetFullPath(diffuseTexturePath.Trim());
            return this.Descriptions.TryGetValue(path, out description);
        }
    }
}
