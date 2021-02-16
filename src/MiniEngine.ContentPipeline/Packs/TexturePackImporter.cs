using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Framework.Content.Pipeline.Builder;

namespace MiniEngine.ContentPipeline.Packs
{
    [ContentImporter(".pack", DisplayName = "Pack - MiniEngine", DefaultProcessor = "PassThroughProcessor")]
    internal sealed class TexturePackImporter : ContentImporter<TexturePack>
    {
        public override TexturePack Import(string filename, ContentImporterContext context)
        {
            var projectDirectory = this.FindProjectDirectory(filename);
            var pack = new TexturePack(Path.GetFileNameWithoutExtension(filename));

            var directory = Path.GetDirectoryName(filename);
            foreach (var file in Directory.EnumerateFiles(directory, "*.png", SearchOption.TopDirectoryOnly))
            {
                var assetName = this.GetAssetName(projectDirectory, file);
                context.Logger.LogMessage($"Adding {filename} as {assetName}");
                pack.Names.Add(assetName);
            }

            return pack;
        }

        private string FindProjectDirectory(string filename)
        {
            var directory = new DirectoryInfo(Path.GetDirectoryName(filename));

            while (directory.Name.ToLower() != "content")
            {
                directory = new DirectoryInfo(Path.Combine(directory.FullName, ".."));
            }

            return directory.FullName;
        }

        private string GetAssetName(string projectDirectory, string sourceFileName)
        {
            var relativeSourceFileName = PathHelper.GetRelativePath($"{projectDirectory}\\", sourceFileName);

            var directoryName = Path.GetDirectoryName(relativeSourceFileName);
            var fileName = Path.GetFileNameWithoutExtension(relativeSourceFileName);
            var assetName = Path.Combine(directoryName, fileName);
            return PathHelper.Normalize(assetName);
        }
    }
}
