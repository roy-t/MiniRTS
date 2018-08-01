using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Systems;
using MiniEngine.Units;
using System;

namespace MiniEngine.Scenes
{
    public sealed class ZimaScene : IScene
    {
        private readonly EntityController EntityController;
        private readonly DirectionalLightSystem DirectionalLightSystem;
        private readonly PointLightSystem PointLightSystem;
        private readonly ModelSystem ModelSystem;

        private readonly Color[] ColorWheel = {
            new Color(255, 0, 0),
            new Color(255, 128, 0),
            new Color(255, 255, 0),
            new Color(128, 255, 0),
            new Color(0, 255, 0),
            new Color(0, 255, 128),
            new Color(0, 255, 255),
            new Color(0, 128, 255),
            new Color(0, 0, 255)
        };

        private Model lizard;
        private Model ship1;
        private Model ship2;

        private Entity[] pointLightEntities;
        private Entity[] directionalLightEntities;
        private Entity[] modelEntities;

        public ZimaScene(
            EntityController entityController,
            DirectionalLightSystem directionalLightSystem,
            PointLightSystem pointLightSystem,
            ModelSystem modelSystem)
        {
            this.EntityController = entityController;
            this.DirectionalLightSystem = directionalLightSystem;
            this.PointLightSystem = pointLightSystem;
            this.ModelSystem = modelSystem;
        }

        public void LoadContent(ContentManager content)
        {
            this.lizard = content.Load<Model>(@"Lizard\Lizard");
            this.ship1 = content.Load<Model>(@"Ship1\Ship1");
            this.ship2 = content.Load<Model>(@"Ship2\Ship2");
        }

        public void Set()
        {
            this.pointLightEntities = new Entity[this.ColorWheel.Length];

            var step = MathHelper.TwoPi / this.ColorWheel.Length;

            for (var i = 0; i < this.pointLightEntities.Length; i++)
            {
                this.pointLightEntities[i] = this.EntityController.CreateEntity();

                var x = Math.Sin(step * i) * 100;
                var y = Math.Cos(step * i) * 100;
                this.PointLightSystem.Add(
                    this.pointLightEntities[i],
                    new Vector3((float) x, (float) y, 0),
                    this.ColorWheel[i],
                    120,
                    1.0f);
            }

            this.directionalLightEntities = this.EntityController.CreateEntities(2);
            this.DirectionalLightSystem.Add(this.directionalLightEntities[0], Vector3.Normalize(Vector3.Forward + Vector3.Down), Color.White * 0.75f);
            this.DirectionalLightSystem.Add(this.directionalLightEntities[1], Vector3.Normalize(Vector3.Forward + Vector3.Up + Vector3.Left), Color.BlueViolet * 0.25f);

            this.modelEntities = this.EntityController.CreateEntities(3);
            this.ModelSystem.Add(this.modelEntities[0], this.ship1, Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateScale(0.5f));
            this.ModelSystem.Add(this.modelEntities[1], this.lizard, Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateScale(0.05f) * Matrix.CreateTranslation(Vector3.Left * 50));
            this.ModelSystem.Add(this.modelEntities[2], this.ship2, Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(Vector3.Right * 50));
        }

        public void Update(Seconds elapsed)
        {
          
        }        
    }
}
