using System.IO;
using LightInject;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Effects
{
    public sealed class EffectCompositionRoot : ICompositionRoot
    {
        public static ContentManager Content;

        public void Compose(IServiceRegistry serviceRegistry)
        {
            this.RegisterEffect<RenderEffect>(serviceRegistry, "RenderEffect");
            this.RegisterEffect<CombineEffect>(serviceRegistry, "CombineEffect");
            this.RegisterEffect<FxaaEffect>(serviceRegistry, "FxaaEffect");
            this.RegisterEffect<BlurEffect>(serviceRegistry, "BlurEffect");
            this.RegisterEffect<WeightedParticlesEffect>(serviceRegistry, "WeightedParticlesEffect");
            this.RegisterEffect<AverageParticlesEffect>(serviceRegistry, "AverageParticlesEffect");
            this.RegisterEffect<AdditiveParticlesEffect>(serviceRegistry, "AdditiveParticlesEffect");
            this.RegisterEffect<ColorEffect>(serviceRegistry, "ColorEffect");
            this.RegisterEffect<TextureEffect>(serviceRegistry, "TextureEffect");
            this.RegisterEffect<UIEffect>(serviceRegistry, "UIEffect");
            this.RegisterEffect<AmbientLightEffect>(serviceRegistry, "AmbientLightEffect");
            this.RegisterEffect<DirectionalLightEffect>(serviceRegistry, "DirectionalLightEffect");
            this.RegisterEffect<PointLightEffect>(serviceRegistry, "PointLightEffect");
            this.RegisterEffect<ShadowCastingLightEffect>(serviceRegistry, "ShadowCastingLightEffect");
            this.RegisterEffect<SunlightEffect>(serviceRegistry, "SunlightEffect");
            this.RegisterEffect<ProjectorEffect>(serviceRegistry, "ProjectorEffect");
        }

        private void RegisterEffect<T>(IServiceRegistry registry, string name, string folder = "Effects")
            where T : EffectWrapper, new()
        {
            registry.Register(
                i =>
                {
                    var wrapper = new T();
                    wrapper.Wrap(Content.Load<Effect>(Path.Combine(folder, name)));
                    return wrapper;
                },
                new PerRequestLifeTime());
        }
    }
}
