using System;
using System.Collections.Generic;
using System.IO;

namespace MiniEngine.ContentProcessors
{
    internal class MaterialDescriptionFile
    {
        private const string Extension = "ini";

        private readonly Dictionary<string, MaterialDescription> Descriptions;

        public MaterialDescriptionFile(string modelPath)
        {
            this.File = GetMaterialDescriptionFilePath(modelPath);
            var parser = new MaterialDescriptionParser(modelPath);
            this.Descriptions = parser.Parse(this.File);
        }

        /// <summary>
        /// Full path to the material description file
        /// </summary>
        public string File { get; }

        public bool TryGetValue(string diffuseTexturePath, out MaterialDescription description)
        {
            var path = Path.GetFullPath(diffuseTexturePath.Trim());
            return this.Descriptions.TryGetValue(path, out description);
        }

        private static string GetMaterialDescriptionFilePath(string modelPath)
        {
            var dot = Path.GetFullPath(modelPath).LastIndexOf(".", StringComparison.OrdinalIgnoreCase);
            var file = modelPath.Substring(0, dot) + "." + Extension;

            if (!System.IO.File.Exists(file))
            {
                throw new FileNotFoundException($"Could not find file {file}", file);
            }

            return file;
        }
    }
}
