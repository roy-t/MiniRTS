using System;
using System.Collections.Generic;
using System.IO;

namespace ModelExtension
{
    internal class MaterialDescriptionFile
    {
        private const string Extension = "mat";

        private readonly Dictionary<string, MaterialDescription> Descriptions;

        public MaterialDescriptionFile(string modelPath)
        {
            this.Descriptions = Parse(modelPath);
        }

        public bool TryGetValue(string diffuseTexturePath, out MaterialDescription description)
        {
            var path = GetFullInvariantPathIfFileExists("", diffuseTexturePath);
            return this.Descriptions.TryGetValue(path, out description);
        }

        /// <summary>
        /// Looks-up, and parses, the mat file belong to the model
        /// </summary>        
        private static Dictionary<string, MaterialDescription> Parse(string modelPath)
        {
            var dot = Path.GetFullPath(modelPath).LastIndexOf(".", StringComparison.OrdinalIgnoreCase);
            var path = modelPath.Substring(0, dot) + "." + Extension;            

            var file = new FileInfo(path);
            if (!file.Exists)
            {
                throw new FileNotFoundException("File not found", path);
            }

            var basePath = file.DirectoryName;
            if (string.IsNullOrEmpty(basePath))
            {
                throw new Exception("Invalid directory");
            }

            var dictionary = new Dictionary<string, MaterialDescription>();
            using (var reader = new StreamReader(file.OpenRead()))
            {
                var ln = 0;
                while (!reader.EndOfStream)
                {                    
                    var line = reader.ReadLine();
                    ++ln;

                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                        continue;

                    var tokens = line.Split(';');
                    if (tokens.Length != 3)
                    {
                        throw new Exception($"Invalid number of tokens at line {ln}");
                    }

                    dictionary.Add(
                        GetFullInvariantPathIfFileExists(basePath, tokens[0]),
                        new MaterialDescription(
                            GetFullInvariantPathIfFileExists(basePath, tokens[0]),
                            GetFullInvariantPathIfFileExists(basePath, tokens[1]),
                            GetFullInvariantPathIfFileExists(basePath, tokens[2])));
                }                              
            }
            
            return dictionary;
        }

        private static string GetFullInvariantPathIfFileExists(string basePath, string relativePath)
        {
            var path = Path.GetFullPath(Path.Combine(basePath.Trim(), relativePath.Trim()));
            return File.Exists(path) 
                ? path 
                : null;
        }
    }
}
