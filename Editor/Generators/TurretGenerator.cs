using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Pipeline.Models.Factories;
using MiniEngine.Primitives;
using MiniEngine.Systems;

namespace MiniEngine.Generators
{
    public sealed class TurretGenerator
    {
        private readonly ContentManager Content;
        private readonly EntityController EntityController;
        private readonly OpaqueModelFactory OpaqueModelFactory;

        public TurretGenerator(ContentManager content, EntityController entityController, OpaqueModelFactory opaqueModelFactory)
        {
            this.Content = content;
            this.EntityController = entityController;
            this.OpaqueModelFactory = opaqueModelFactory;
        }


        public OpaqueModel Generate(Microsoft.Xna.Framework.Vector3 position)
        {
            var entity = this.EntityController.CreateEntity();
            var model = this.GenerateModel();
            var pose = new Pose();
            pose.Move(position);
            return this.OpaqueModelFactory.Construct(entity, model, pose);
        }


        private Model GenerateModel()
        {
            var model = this.Content.Load<Model>(@"Scenes\Zima\Lizard\lizard");
            return model;
        }

    }
}
