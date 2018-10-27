using MiniEngine.Controllers;
using MiniEngine.Input;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Configuration
{
    public sealed class LightSystemsControllerFactory : IInstanceFactory<LightSystemsController, PerspectiveCamera>
    {
        private readonly DirectionalLightSystem DirectionalLightSystem;
        private readonly EntityController EntityController;
        private readonly KeyboardInput KeyboardInput;
        private readonly PointLightSystem PointLightSystem;
        private readonly ShadowCastingLightSystem ShadowCastingLightSystem;
        private readonly SunlightSystem SunlightSystem;

        public LightSystemsControllerFactory(
            KeyboardInput keyboardInput,
            EntityController entityController,
            PointLightSystem pointLightSystem,
            SunlightSystem sunlightSystem,
            DirectionalLightSystem directionalLightSystem,
            ShadowCastingLightSystem shadowCastingLightSystem)
        {
            this.KeyboardInput = keyboardInput;
            this.EntityController = entityController;
            this.PointLightSystem = pointLightSystem;
            this.SunlightSystem = sunlightSystem;
            this.DirectionalLightSystem = directionalLightSystem;
            this.ShadowCastingLightSystem = shadowCastingLightSystem;
        }

        public LightSystemsController Build(PerspectiveCamera camera)
        {
            return new LightSystemsController(
                this.KeyboardInput,
                camera,
                this.EntityController,
                this.PointLightSystem,
                this.SunlightSystem,
                this.DirectionalLightSystem,
                this.ShadowCastingLightSystem);
        }
    }
}