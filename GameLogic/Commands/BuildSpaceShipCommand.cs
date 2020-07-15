using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.GameLogic.BluePrints;
using MiniEngine.GameLogic.Factories;
using MiniEngine.Pipeline.Basics.Factories;
using MiniEngine.Pipeline.Models.Factories;
using MiniEngine.Pipeline.Particles.Factories;
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

            this.BuildParts(content, factories, entityController, entity, content.Cap.Model, content.Exhaust, content.FuelTank, content.RibbedFuelTank, content.FuelTank, content.Fairing);

            return entity;
        }


        private void BuildParts(Content content, Resolver<IComponentFactory> factories, EntityController entityController, Entity root, Model connector, params RocketFuselageBluePrint[] bluePrints)
        {
            var totalHeight = bluePrints.Sum(x => x.Height);
            var currentHeight = -(totalHeight / 2.0f);

            var children = new Entity[(bluePrints.Length * 2) - 1];
            var c = 0;

            for (var i = 0; i < bluePrints.Length; i++)
            {
                var bluePrint = bluePrints[i];

                var toCenterOfMass = Vector3.Up * bluePrint.Height * 0.5f;
                var offset = (Vector3.Up * currentHeight) + toCenterOfMass;

                currentHeight += bluePrint.Height;

                var entity = entityController.CreateEntity($"SpaceShip_Part_{i}");
                this.AddPart(factories, root, bluePrint.Model, offset, entity);
                children[c++] = entity;

                if (bluePrint.ExhaustOffsets.Length > 0)
                {
                    this.CreateExhausts(entityController, factories, root, entity, bluePrint, content);
                }

                if (i < bluePrints.Length - 1)
                {
                    offset += toCenterOfMass;
                    entity = entityController.CreateEntity($"SpaceShip_Part_Cap_{i}");
                    this.AddPart(factories, root, connector, offset, entity);
                    children[c++] = entity;
                }
            }

            factories.Get<ParentFactory>().Construct(root, children);
        }

        private void CreateExhausts(EntityController entityController, Resolver<IComponentFactory> factories, Entity rootEntity, Entity partEntity, RocketFuselageBluePrint bluePrint, Content content)
        {
            var emitters = new Entity[bluePrint.ExhaustOffsets.Length];
            for (var i = 0; i < bluePrint.ExhaustOffsets.Length; i++)
            {
                var exhaust = bluePrint.ExhaustOffsets[i];
                var exhaustEntity = entityController.CreateEntity("Exhaust");
                factories.Get<PoseFactory>().Construct(exhaustEntity, Vector3.Zero, 1.0f);
                factories.Get<OffsetFactory>().Construct(exhaustEntity, exhaust.Offset, exhaust.Yaw, exhaust.Pitch, exhaust.Roll, partEntity);

                var emitter = factories.Get<AdditiveEmitterFactory>().ConstructAdditiveEmitter(exhaustEntity, content.Explosion2, 1, 1);
                emitter.SpawnInterval = 0;
                emitter.Spread = 0.1f;
                emitter.Speed = 20.0f;
                emitter.TimeToLive = 0.5f;

                emitters[i] = exhaustEntity;
            }

            var rcs = factories.Get<ReactionControlFactory>().Construct(rootEntity, emitters);
            rcs.EmitterReactionRange = 0.95f;
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
