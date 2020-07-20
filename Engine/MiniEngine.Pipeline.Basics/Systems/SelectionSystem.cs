using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;

namespace MiniEngine.Pipeline.Basics.Systems
{
    public sealed class SelectionSystem : ISystem
    {
        private const float LargeNumber = 10000000;

        private readonly IComponentContainer<SelectionHitbox> Hitboxes;
        private readonly IComponentContainer<Pose> Poses;

        private Vector3? selectionStart;

        public SelectionSystem(IComponentContainer<SelectionHitbox> hitboxes, IComponentContainer<Pose> poses)
        {
            this.Hitboxes = hitboxes;
            this.Poses = poses;
        }

        public void StartSelection(PerspectiveCamera camera, Point startPosition)
            => this.selectionStart = camera.Pick(startPosition, 0.0f);

        public void EndSelection(PerspectiveCamera camera, Point endPosition, List<Entity> selectedEntities)
        {
            var selectionEnd = camera.Pick(endPosition, 0.0f);

            if (this.selectionStart.HasValue && selectionEnd.HasValue)
            {
                var selectionBounds = BoundingBox.CreateFromPoints(
                    new Vector3[]
                    {
                            this.selectionStart.Value + (Vector3.Up * LargeNumber),
                            selectionEnd.Value + (Vector3.Down * LargeNumber)
                    });

                for (var i = 0; i < this.Hitboxes.Count; i++)
                {
                    var hitbox = this.Hitboxes[i];
                    var pose = this.Poses.Get(hitbox.Entity);

                    var entityBounds = BoundingBox.CreateFromPoints(new Vector3[]
                    {
                            pose.Position + (Vector3.Left * hitbox.Width * 0.5f),
                            pose.Position + (Vector3.Right * hitbox.Width * 0.5f),

                            pose.Position + (Vector3.Up * hitbox.Height* 0.5f),
                            pose.Position + (Vector3.Down * hitbox.Height * 0.5f),

                            pose.Position + (Vector3.Forward * hitbox.Depth* 0.5f),
                            pose.Position + (Vector3.Backward * hitbox.Depth* 0.5f),
                    });

                    if (selectionBounds.Intersects(entityBounds))
                    {
                        selectedEntities.Add(hitbox.Entity);
                    }
                }
            }
        }
    }
}
