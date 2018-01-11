using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;


namespace ModelExtension
{
    [ContentProcessor(DisplayName = "ModelExtension :: Deferred Renderer Model Processor")]
    public class DeferredRendererModelProcessor : ModelProcessor
    {
        string directory;

        [DisplayName("Normal Map Texture")]
        [Description(
            "If set, this file will be used as the normal map on the model, " +
            "overriding anything found in the opaque data.")]
        [DefaultValue("")]
        public string NormalMapTexture { get; set; }

        [DisplayName("Normal Map Key")]
        [Description("This will be the key that will be used to search the normal map in the opaque data of the model")]
        [DefaultValue("NormalMap")]
        public string NormalMapKey { get; set; } = "NormalMap";

        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            this.directory = Path.GetDirectoryName(input.Identity.SourceFilename);
            LookUpTextures(input);
            return base.Process(input, context);
        }       


        [Browsable(false)]
        public override bool GenerateTangentFrames
        {
            get { return true; }
            set { }
        }


        static readonly IList<string> AcceptableVertexChannelNames =
            new[]
            {
                VertexChannelNames.TextureCoordinate(0),
                VertexChannelNames.Normal(0),
                VertexChannelNames.Binormal(0),
                VertexChannelNames.Tangent(0),
            };

        protected override void ProcessVertexChannel(
            GeometryContent geometry,
            int vertexChannelIndex,
            ContentProcessorContext context)
        {
            var vertexChannelName =
                geometry.Vertices.Channels[vertexChannelIndex].Name;

            // if this vertex channel has an acceptable names, process it as normal.
            if (AcceptableVertexChannelNames.Contains(vertexChannelName))
            {
                base.ProcessVertexChannel(geometry, vertexChannelIndex, context);
            }
            // otherwise, remove it from the vertex channels; it's just extra data
            // we don't need.
            else
            {
                geometry.Vertices.Channels.Remove(vertexChannelName);
            }
        }

        private void LookUpTextures(NodeContent node)
        {
            if (node is MeshContent mesh)
            {
                //this will contatin the path to the normal map texture
                string normalMapPath;

                //If the NormalMapTexture property is set, we use that normal map for all meshes in the model.
                //This overrides anything else
                if (!string.IsNullOrEmpty(this.NormalMapTexture))
                {
                    normalMapPath = this.NormalMapTexture;
                }
                else
                {
                    //If NormalMapTexture is not set, we look into the opaque data of the model, 
                    //and search for a texture with the key equal to NormalMapKey
                    normalMapPath = mesh.OpaqueData.GetValue<string>(this.NormalMapKey, null);
                }
                //if the NormalMapTexture Property was not used, and the key was not found in the model, than normalMapPath would have the value null.
                if (normalMapPath == null)
                {
                    //If a key with the required name is not found, we make a final attempt, 
                    //and search, in the same directory as the model, for a texture named 
                    //meshname_n.tga, where meshname is the name of a mesh inside the model.
                    normalMapPath = Path.Combine(this.directory, mesh.Name + "_n.tga");
                    if (!File.Exists(normalMapPath))
                    {
                        //if this fails also (that texture does not exist), 
                        //then we use a default texture, named null_normal.tga
                        normalMapPath = this.NormalMapFallback;
                    }
                }
                else
                {
                    normalMapPath = Path.Combine(this.directory, normalMapPath);
                }

                string specularMapPath;

                //If the SpecularMapTexture property is set, we use it
                if (!string.IsNullOrEmpty(this.SpecularMapTexture))
                {
                    specularMapPath = this.SpecularMapTexture;
                }
                else
                {
                    //If SpecularMapTexture is not set, we look into the opaque data of the model, 
                    //and search for a texture with the key equal to specularMapKey
                    specularMapPath = mesh.OpaqueData.GetValue<string>(this.SpecularMapKey, null);
                }

                if (specularMapPath == null)
                {
                    //we search, in the same directory as the model, for a texture named 
                    //meshname_s.tga
                    specularMapPath = Path.Combine(this.directory, mesh.Name + "_s.tga");
                    if (!File.Exists(specularMapPath))
                    {
                        //if this fails also (that texture does not exist), 
                        //then we use a default texture, named null_specular.tga
                        specularMapPath = this.SpecularMapFallback;
                    }
                }
                else
                {
                    specularMapPath = Path.Combine(this.directory, specularMapPath);
                }
                //add the keys to the material, so they can be used by the shader
                foreach (var geometry in mesh.Geometry)
                {
                    //in some .fbx files, the key might be found in the textures collection, but not
                    //in the mesh, as we checked above. If this is the case, we need to get it out, and
                    //add it with the "NormalMap" key
                    if (geometry.Material.Textures.ContainsKey(this.NormalMapKey))
                    {
                        var texRef = geometry.Material.Textures[this.NormalMapKey];
                        geometry.Material.Textures.Remove(this.NormalMapKey);
                        geometry.Material.Textures.Add("NormalMap", texRef);
                    }
                    else
                        geometry.Material.Textures.Add(
                            "NormalMap",
                            new ExternalReference<TextureContent>(normalMapPath));

                    if (geometry.Material.Textures.ContainsKey(this.SpecularMapKey))
                    {
                        var texRef = geometry.Material.Textures[this.SpecularMapKey];
                        geometry.Material.Textures.Remove(this.SpecularMapKey);
                        geometry.Material.Textures.Add("SpecularMap", texRef);
                    }
                    else
                        geometry.Material.Textures.Add(
                            "SpecularMap",
                            new ExternalReference<TextureContent>(specularMapPath));
                }
            }

            // go through all children and apply LookUpTextures recursively
            foreach (var child in node.Children)
            {
                LookUpTextures(child);
            }
        }

        [DisplayName("Specular Map Texture")]
        [Description(
            "If set, this file will be used as the specular map on the model, " +
            "overriding anything found in the opaque data.")]
        [DefaultValue("")]
        public string SpecularMapTexture { get; set; }

        [DisplayName("Specular Map Fallback")]
        public string SpecularMapFallback { get; set; } = "null_specular.tga";

        [DisplayName("Normal Map Fallback")]
        public string NormalMapFallback { get; set; } = "null_normal.tga";

        [DisplayName("Specular Map Key")]
        [Description(
            "This will be the key that will be used to search the specular map in the opaque data of the model")]
        [DefaultValue("SpecularMap")]
        public string SpecularMapKey { get; set; } = "SpecularMap";

        [DisplayName("Effect File")]
        [Description("The effect file which shaders to apply to the model")]
        public string EffectFile { get; set; } = "RenderEffect.fx";

        protected override MaterialContent ConvertMaterial(
            MaterialContent material,
            ContentProcessorContext context)
        {
            var parameters = new OpaqueDataDictionary
            {
                {"ColorKeyColor", this.ColorKeyColor},
                {"ColorKeyEnabled", this.ColorKeyEnabled},
                {"GenerateMipmaps", this.GenerateMipmaps},
                {"PremultiplyTextureAlpha", this.PremultiplyTextureAlpha},
                {"ResizeTexturesToPowerOfTwo", this.ResizeTexturesToPowerOfTwo},
                {"TextureFormat", this.TextureFormat},
                {"DefaultEffect", this.DefaultEffect}
            };

            var deferredShadingMaterial =
                new EffectMaterialContent {Effect = new ExternalReference<EffectContent>(this.EffectFile)};

            // copy the textures in the original material to the new normal mapping
            // material, if they are relevant to our renderer. The
            // LookUpTextures function has added the normal map and specular map
            // textures to the Textures collection, so that will be copied as well.
            foreach (var texture
                in material.Textures)
            {
                if ((texture.Key == "Texture") ||
                    (texture.Key == "NormalMap") ||
                    (texture.Key == "SpecularMap"))
                    deferredShadingMaterial.Textures.Add(texture.Key, texture.Value);
            }

            return context.Convert<MaterialContent, MaterialContent>(
                deferredShadingMaterial,
                typeof (MaterialProcessor).Name, parameters);
        }
    }
}
