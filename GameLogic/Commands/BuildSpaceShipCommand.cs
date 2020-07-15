using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Basics.Factories;
using MiniEngine.Pipeline.Models.Factories;
using MiniEngine.Systems;
using MiniEngine.Systems.Factories;

namespace MiniEngine.GameLogic.Commands
{
    public sealed class BuildSpaceShipCommand : ICommand
    {
        public int CommandId => 2;

        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }

        public Entity Execute(Content content, EntityController entityController, Resolver<IComponentFactory> factories)
        {
            var entity = entityController.CreateEntity("SpaceShip");

            factories.Get<PoseFactory>().Construct(entity, this.Position, this.Scale);

            this.BuildParts(factories, entityController, entity, content.Cap, content.Exhaust, content.FuelTank, content.RibbedFuelTank, content.FuelTank, content.Fairing);

            return entity;
        }


        private void BuildParts(Resolver<IComponentFactory> factories, EntityController entityController, Entity root, Model connector, params Model[] models)
        {
            var partHeight = 4.0f;
            var totalHeight = models.Length * partHeight;
            var toCenterOfMass = Vector3.Up * partHeight * 0.5f;
            var startPosition = (Vector3.Down * (totalHeight / 2.0f)) + toCenterOfMass;
            var step = Vector3.Up * partHeight;

            var children = new Entity[(models.Length * 2) - 1];
            var c = 0;

            for (var i = 0; i < models.Length; i++)
            {
                var offset = startPosition + (step * i);
                var entity = entityController.CreateEntity($"SpaceShip_Part_{i}");
                this.AddPart(factories, root, models[i], offset, entity);
                children[c++] = entity;

                if (i < models.Length - 1)
                {
                    offset += toCenterOfMass;
                    entity = entityController.CreateEntity($"SpaceShip_Part_Cap_{i}");
                    this.AddPart(factories, root, connector, offset, entity);
                    children[c++] = entity;
                }
            }

            factories.Get<ParentFactory>().Construct(root, children);
        }

        private void AddPart(Resolver<IComponentFactory> factories, Entity root, Model model, Vector3 position, Entity entity)
        {
            factories.Get<PoseFactory>().Construct(entity, Vector3.Zero, this.Scale);
            factories.Get<OffsetFactory>().Construct(entity, position, 0, 0, 0, root);
            factories.Get<OpaqueModelFactory>().Construct(entity, model);
        }

        public void Serialize(Stream stream) => throw new NotImplementedException();

        public void Deserialize(Stream stream) => throw new NotImplementedException();
    }
}
