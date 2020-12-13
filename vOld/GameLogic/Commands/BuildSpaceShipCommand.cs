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
            factories.Get<SelectionHitboxFactory>().Construct(entity, 1, 1, 1);

            this.BuildParts(content, factories, entityController, entity, content.Cap.Model, content.RCS, new int[] { 1, 3 }, content.Exhaust, content.FuelTank, content.RibbedFuelTank, content.FuelTank, content.Fairing);

            return entity;
        }

        private void BuildParts(Content content, Resolver<IComponentFactory> factories, EntityController entityController, Entity root, Model connector, RCSBluePrint rcsBluePrint, int[] rcsPositions, params FuselageBluePrint[] bluePrints)
        {
            var totalHeight = bluePrints.Sum(x => x.Height);
            var currentHeight = -(totalHeight / 2.0f);

            var children = new Entity[(bluePrints.Length * 2) - 1];
            var c = 0;

            for (var i = 0; i < bluePrints.Length; i++)
            {
                var bluePrint = bluePrints[i];

                var toCenterOfMass = Vector3.Forward * bluePrint.Height * 0.5f;
                var offset = (Vector3.Forward * currentHeight) + toCenterOfMass;

                currentHeight += bluePrint.Height;

                var entity = entityController.CreateEntity($"SpaceShip_Part_{i}");
                this.AddPart(factories, root, bluePrint.Model, offset, entity);
                children[c++] = entity;

                if (bluePrint.Exhausts.Length > 0)
                {
                    this.CreateExhausts(entityController, factories, root, entity, bluePrint, content);
                }

                if (rcsPositions.Contains(i))
                {
                    this.CreateRcs(entityController, factories, entity, rcsBluePrint, content);
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

        private void CreateRcs(EntityController entityController, Resolver<IComponentFactory> factories, Entity partEntity, RCSBluePrint rcsBluePrint, Content content)
        {
            var count = 4;
            var step = MathHelper.TwoPi / count;
            for (var r = 0; r < count; r++)
            {
                var rcsEntity = entityController.CreateEntity("RCS_Root");
                factories.Get<PoseFactory>().Construct(rcsEntity, Vector3.Zero);
                factories.Get<OffsetFactory>().Construct(rcsEntity, Vector3.Zero, 0, 0, step * r, partEntity);
                factories.Get<OpaqueModelFactory>().Construct(rcsEntity, rcsBluePrint.Model);

                var emitters = new Entity[rcsBluePrint.Exhausts.Length];
                for (var i = 0; i < rcsBluePrint.Exhausts.Length; i++)
                {
                    var exhaust = rcsBluePrint.Exhausts[i];
                    var exhaustEntity = entityController.CreateEntity($"RCS_Exhaust_{i}");
                    factories.Get<PoseFactory>().Construct(exhaustEntity, Vector3.Zero, 0.1f);
                    factories.Get<OffsetFactory>().Construct(exhaustEntity, exhaust.Offset, exhaust.Yaw, exhaust.Pitch, exhaust.Roll, rcsEntity);

                    var emitter = factories.Get<AdditiveEmitterFactory>().ConstructAdditiveEmitter(exhaustEntity, content.Explosion2, 1, 1);
                    emitter.SpawnInterval = 0.0f;
                    emitter.Spread = 0.1f;
                    emitter.Speed = 6.0f;
                    emitter.TimeToLive = 0.15f;

                    emitters[i] = exhaustEntity;
                }

                factories.Get<ReactionControlFactory>().Construct(rcsEntity, emitters);
            }
        }

        private void CreateExhausts(EntityController entityController, Resolver<IComponentFactory> factories, Entity rootEntity, Entity partEntity, FuselageBluePrint bluePrint, Content content)
        {
            var emitters = new Entity[bluePrint.Exhausts.Length];
            for (var i = 0; i < bluePrint.Exhausts.Length; i++)
            {
                var exhaust = bluePrint.Exhausts[i];
                var exhaustEntity = entityController.CreateEntity("Exhaust");
                factories.Get<PoseFactory>().Construct(exhaustEntity, Vector3.Zero);
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
