using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using MiniEngine.Pipeline.Models.Components;

namespace MiniEngine.ContentProcessors
{
    [ContentProcessor(DisplayName = "MiniEngine :: Multi Material Model Processor")]
    public class MultiMaterialModelProcessor : ModelProcessor
    {
        private static readonly IList<string> AcceptableVertexChannelNames =
            new[]
            {
                VertexChannelNames.TextureCoordinate(0),
                VertexChannelNames.Normal(0),
                VertexChannelNames.Binormal(0),
                VertexChannelNames.Tangent(0),
                VertexChannelNames.Weights(0),
                VertexChannelNames.EncodeName("BlendIndices", 0),
                VertexChannelNames.EncodeName("BlendWeight", 0),
            };

        [DisplayName("Process Textures")]
        [Description("Searches for a Material Description file (.ini) and processes the related textures")]
        [DefaultValue(true)]
        public virtual bool ProcessTextures { get; set; } = true;

        [DisplayName("Process Armature")]
        [Description("Process an armature/bones")]
        [DefaultValue(false)]
        public virtual bool ProcessArmature { get; set; } = false;

        [DisplayName("Specular Map Fallback")]
        public string SpecularMapFallback { get; set; } = "NeutralSpecular.tga";

        [DisplayName("Normal Map Fallback")]
        public string NormalMapFallback { get; set; } = "NeutralNormal.tga";

        [DisplayName("Mask Fallback")]
        public string MaskFallback { get; set; } = "NeutralMask.tga";

        [DisplayName("Reflection Fallback")]
        public string ReflectionFallback { get; set; } = "NeutralReflection.tga";

        [DisplayName("Effect File")]
        [Description("The effect file which shaders to apply to the model")]
        public string EffectFile { get; set; } = "Effects/RenderEffect.fx";

        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (this.ProcessTextures)
            {
                // Look up all materials and link them to the model
                MaterialLinker.Bind(
                    context,
                    input,
                    Path.GetFullPath(this.NormalMapFallback),
                    Path.GetFullPath(this.SpecularMapFallback),
                    Path.GetFullPath(this.MaskFallback),
                    Path.GetFullPath(this.ReflectionFallback));
            }


            if (this.ProcessArmature)
            {
                var skeleton = MeshHelper.FindSkeleton(input);
                if (skeleton == null)
                {
                    throw new InvalidContentException("Input armature not found");
                }

                var bones = MeshHelper.FlattenSkeleton(skeleton);
                if (bones.Count > SkinningData.MaxBones)
                {
                    throw new InvalidContentException($"Skeleton has {bones.Count} bones, but the maximum supported is {SkinningData.MaxBones}");
                }

                var bindPose = new List<Matrix>();
                var inverseBindPose = new List<Matrix>();
                var skeletonHierarchy = new List<int>();
                var boneNames = new List<string>();

                foreach (var bone in bones)
                {
                    bindPose.Add(bone.Transform);
                    inverseBindPose.Add(Matrix.Invert(bone.AbsoluteTransform));
                    skeletonHierarchy.Add(bones.IndexOf(bone.Parent as BoneContent));
                    boneNames.Add(bone.Name);
                }

                var model = base.Process(input, context);
                model.Tag = new SkinningData(bindPose, inverseBindPose, skeletonHierarchy, boneNames);

                return model;
            }
            else
            {
                return base.Process(input, context);
            }
        }

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
                new EffectMaterialContent { Effect = new ExternalReference<EffectContent>(this.EffectFile) };

            // Copy the textures in the original material to the new
            // material, if they are relevant to our renderer. 
            foreach (var texture
                in material.Textures)
            {
                if (texture.Key == "Texture" ||
                    texture.Key == "NormalMap" ||
                    texture.Key == "ReflectionMap" ||
                    texture.Key == "SpecularMap" ||
                    texture.Key == "Mask")
                {
                    deferredShadingMaterial.Textures.Add(texture.Key, texture.Value);
                }
            }

            return context.Convert<MaterialContent, MaterialContent>(
                deferredShadingMaterial,
                typeof(MaterialProcessor).Name, parameters);
        }

        /// <summary>
        /// Bakes unwanted transforms into the model geometry,
        /// so everything ends up in the same coordinate system.
        /// </summary>
        private static void FlattenTransforms(NodeContent node, BoneContent skeleton)
        {
            foreach (var child in node.Children)
            {
                // Don't process the skeleton, because that is special.
                if (child != skeleton)
                {
                    // Bake the local transform into the actual geometry.
                    MeshHelper.TransformScene(child, child.Transform);

                    // Having baked it, we can now set the local
                    // coordinate system back to identity.
                    child.Transform = Matrix.Identity;

                    // Recurse.
                    FlattenTransforms(child, skeleton);
                }
            }
        }

        #region Hide irrelevant model processor properties
        [Browsable(false)]
        public override bool GenerateTangentFrames => true;

        [Browsable(false)]
        public override Color ColorKeyColor => Color.Magenta;

        [Browsable(false)]
        public override bool ColorKeyEnabled => false;

        [Browsable(false)]
        public override MaterialProcessorDefaultEffect DefaultEffect => MaterialProcessorDefaultEffect.BasicEffect;

        [Browsable(false)]
        public override bool GenerateMipmaps => true;

        [Browsable(false)]
        public override bool PremultiplyTextureAlpha => true;

        [Browsable(false)]
        public override bool PremultiplyVertexColors => true;

        [Browsable(false)]
        public override bool ResizeTexturesToPowerOfTwo => false;

        [Browsable(false)]
        public override TextureProcessorOutputFormat TextureFormat => TextureProcessorOutputFormat.Compressed;
        #endregion
    }
}
