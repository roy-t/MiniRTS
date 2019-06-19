using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace ModelExtension
{
    public static class MaterialLinker
    {
        public static void Bind(NodeContent node, string defaultNormalTexture, string defaultSpecularTexture, string defaultMaskTexture)
        {
            if (node is MeshContent mesh)
            {
                var modelPath = mesh.Identity.SourceFilename;
                var lookup = new MaterialDescriptionFile(modelPath);

                foreach (var geometry in mesh.Geometry)
                {
                    if (geometry.Material.Textures.TryGetValue("Texture", out var texture))
                    {
                        var fileName = texture.Filename;
                        geometry.Material.Textures.Clear();
                        if (lookup.TryGetValue(fileName, out var description))
                        {
                            geometry.Material.Textures.Add(
                                "Texture",
                                new ExternalReference<TextureContent>(description.Diffuse));

                            geometry.Material.Textures.Add(
                                "NormalMap",
                                string.IsNullOrEmpty(description.Normal)
                                    ? new ExternalReference<TextureContent>(defaultNormalTexture)
                                    : new ExternalReference<TextureContent>(description.Normal));

                            geometry.Material.Textures.Add(
                                "SpecularMap",
                                string.IsNullOrEmpty(description.Specular)
                                    ? new ExternalReference<TextureContent>(defaultSpecularTexture)
                                    : new ExternalReference<TextureContent>(description.Specular));

                            geometry.Material.Textures.Add(
                                "Mask",
                                string.IsNullOrEmpty(description.Mask)
                                    ? new ExternalReference<TextureContent>(defaultMaskTexture)
                                    : new ExternalReference<TextureContent>(description.Mask));
                        }
                    }
                    else
                    {
                        geometry.Material.Textures.Clear();
                    }
                }
            }

            foreach (var child in node.Children)
            {
                Bind(child, defaultNormalTexture, defaultSpecularTexture, defaultMaskTexture);
            }
        }
    }
}
