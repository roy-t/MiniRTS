using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Components;
using MiniEngine.Rendering.Systems;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public sealed class PrimitivesScene : IScene
    {
        private readonly EntityController EntityController;
        private readonly AmbientLightSystem AmbientLightSystem;
        private readonly ShadowCastingLightSystem ShadowCastingLightSystem;
        private readonly ModelSystem ModelSystem;

        private Entity worldEntity;
        private Entity sphereEntity;
        private Entity floorEntity;
        private Entity shrineEntity;
        private Entity shadowCastingLightEntity;

        private Model sphereModel;
        private Model floorModel;
        private Model shrineModel;

        public PrimitivesScene(
            EntityController entityController,
            AmbientLightSystem ambientLightSystem,
            ShadowCastingLightSystem shadowCastingLightSystem,
            ModelSystem modelSystem)
        {
            this.EntityController = entityController;
            this.AmbientLightSystem = ambientLightSystem;
            this.ShadowCastingLightSystem = shadowCastingLightSystem;
            this.ModelSystem = modelSystem;
        }

        public void LoadContent(ContentManager content)
        {
            this.sphereModel = content.Load<Model>(@"Scenes\Primitives\Sphere");
            this.floorModel = content.Load<Model>(@"Scenes\Primitives\Cube");
            this.shrineModel = content.Load<Model>(@"Scenes\Primitives\Shrine");
        }

        public void Set()
        {
            this.worldEntity = this.EntityController.CreateEntity();
            this.AmbientLightSystem.Add(this.worldEntity, Color.White * 0.25f);

            this.shadowCastingLightEntity = this.EntityController.CreateEntity();
            this.ShadowCastingLightSystem.Add(
                this.shadowCastingLightEntity,
                new Vector3(0, 100, -100),
                Vector3.Zero * 0.5f,
                Color.White);

            this.sphereEntity = CreateModelEntity(this.sphereModel, Matrix.CreateScale(10) * Matrix.CreateTranslation(Vector3.Up * 10));
            this.floorEntity = CreateModelEntity(this.floorModel, Matrix.CreateScale(100, 1, 100));

            this.shrineEntity = CreateModelEntity(this.shrineModel, Matrix.CreateScale(20, 20, 1) * Matrix.CreateRotationX(MathHelper.PiOver4) * Matrix.CreateTranslation(new Vector3(0, 50, -50)), ModelType.Transparent);
        }

        private Entity CreateModelEntity(Model model, Matrix worldMatrix, ModelType modelType = ModelType.Opaque)
        {
            var entity = this.EntityController.CreateEntity();
            this.ModelSystem.Add(entity, model, worldMatrix, modelType);
            return entity;
        }

        public void Update(Seconds elapsed)
        {

        }
    }
}
