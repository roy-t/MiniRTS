using System;
using System.IO;
using Microsoft.Xna.Framework;
using MiniEngine.GameLogic.Factories;
using MiniEngine.GameLogic.Vehicles.Fighter;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Basics.Factories;
using MiniEngine.Pipeline.Models.Factories;
using MiniEngine.Pipeline.Particles.Components;
using MiniEngine.Pipeline.Particles.Factories;
using MiniEngine.Systems;
using MiniEngine.Systems.Factories;

namespace MiniEngine.GameLogic.Commands
{
    public sealed class BuildFighterCommand : ICommand
    {
        public int CommandId => 1;

        public BuildFighterCommand() { }

        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }

        public Entity Execute(Content content, EntityController entityController, Resolver<IComponentFactory> factories)
        {
            var entity = entityController.CreateEntity("Fighter");
            factories.Get<PoseFactory>().Construct(entity, this.Position, this.Scale);
            factories.Get<OpaqueModelFactory>().Construct(entity, content.Fighter);

            var rcsFront = this.BuildSmallReactionControlSystem(entityController, factories, content, entity, Vector3.Forward * 4, 0, 0, 0);
            var rcsBack = this.BuildSmallReactionControlSystem(entityController, factories, content, entity, Vector3.Backward * 4, 0, 0, 0);
            // TODO: for the thruster it looks best if the accelerometer is at the center of mass but the emitter should
            // be placed at the exhaust
            var thruster = this.BuildThruster(entityController, factories, content, entity, Vector3.Backward * 0, MathHelper.Pi, 0, 0);
            factories.Get<ParentFactory>().Construct(entity, thruster.Entity, rcsFront.Entity, rcsBack.Entity);

            return entity;
        }

        private ReactionControl BuildSmallReactionControlSystem(EntityController entityController, Resolver<IComponentFactory> factories, Content content, Entity target, Vector3 offset, float yaw, float pitch, float roll)
        {
            var entity = entityController.CreateEntity("RCS");
            factories.Get<PoseFactory>().Construct(entity, Vector3.Zero);
            factories.Get<OffsetFactory>().Construct(entity, offset, yaw, pitch, roll, target);

            var (emitterLeft, _, _) = this.BuildRCS(entityController, factories, content, entity, Vector3.Zero, MathHelper.PiOver2, 0, 0);
            var (emitterRight, _, _) = this.BuildRCS(entityController, factories, content, entity, Vector3.Zero, -MathHelper.PiOver2, 0, 0);
            var (emitterUp, _, _) = this.BuildRCS(entityController, factories, content, entity, Vector3.Zero, 0, MathHelper.PiOver2, 0);
            var (emitterDown, _, _) = this.BuildRCS(entityController, factories, content, entity, Vector3.Zero, 0, -MathHelper.PiOver2, 0);

            factories.Get<ParentFactory>().Construct(entity, emitterLeft.Entity, emitterRight.Entity, emitterUp.Entity, emitterDown.Entity);

            return factories.Get<ReactionControlFactory>().Construct(entity, emitterLeft.Entity, emitterRight.Entity, emitterUp.Entity, emitterDown.Entity);
        }

        private (AdditiveEmitter, Pose, Offset) BuildRCS(EntityController entityController, Resolver<IComponentFactory> factories, Content content, Entity target, Vector3 offset, float yaw, float pitch, float roll)
        {
            var entity = entityController.CreateEntity();
            var pose = factories.Get<PoseFactory>().Construct(entity, offset, 0.2f);
            var offsetC = factories.Get<OffsetFactory>().Construct(entity, offset, yaw, pitch, roll, target);

            var emitter = factories.Get<AdditiveEmitterFactory>().ConstructAdditiveEmitter(entity, content.Explosion2, 1, 1);
            emitter.SpawnInterval = 0;
            emitter.Spread = 0.05f;
            emitter.Speed = 6.0f;
            emitter.TimeToLive = 0.1f;

            return (emitter, pose, offsetC);
        }

        private ReactionControl BuildThruster(EntityController entityController, Resolver<IComponentFactory> factories, Content content, Entity target, Vector3 offset, float yaw, float pitch, float roll)
        {
            var entity = entityController.CreateEntity("Thruster");
            factories.Get<PoseFactory>().Construct(entity, Vector3.Zero, 1.0f);
            factories.Get<OffsetFactory>().Construct(entity, offset, yaw, pitch, roll, target);

            var emitter = factories.Get<AdditiveEmitterFactory>().ConstructAdditiveEmitter(entity, content.Explosion2, 1, 1);
            emitter.SpawnInterval = 0;
            emitter.Spread = 0.1f;
            emitter.Speed = 20.0f;
            emitter.TimeToLive = 0.5f;

            var rcs = factories.Get<ReactionControlFactory>().Construct(entity, emitter.Entity);
            rcs.EmitterReactionRange = 0.95f;
            return rcs;
        }

        public void Deserialize(Stream stream) => throw new NotImplementedException();
        public void Serialize(Stream stream) => throw new NotImplementedException();
    }
}
