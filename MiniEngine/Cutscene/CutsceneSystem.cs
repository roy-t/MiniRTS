using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Units;

namespace MiniEngine.CutScene
{
    // TODO: move to own project
    public sealed class CutsceneSystem : IUpdatableSystem
    {        
        private readonly List<Waypoint> Waypoints;
        private readonly EntityLinker Linker;
        private int previous;
        private int current;
        private int next;

        private CutsceneState state;

        public CutsceneSystem(EntityLinker linker)
        {
            this.Waypoints = new List<Waypoint>();
            this.Linker = linker;
            this.state = CutsceneState.Stopped;
        }

        public void AddWaypoint(Waypoint waypoint)
        {            
            this.Waypoints.Add(waypoint);            
        }

        public void Start()
        {
            this.state = CutsceneState.Starting;
        }

        // TODO: rewrite all this because it is a mess! Also introduce elastic camera instead of all the lerping
        // Or at least lerp the target some other way because now it messes with our speed and stuff
        public void Update(PerspectiveCamera camera, Seconds elapsed)
        {            
            if (this.state != CutsceneState.Stopped)
            {
                this.Waypoints.Clear();
                this.Linker.GetComponents(this.Waypoints);

                switch (this.state)
                {
                    case CutsceneState.Starting:
                        this.previous = 0;
                        this.current = 1;
                        this.next = 2;

                        var from = this.Waypoints[this.previous];
                        var to = this.Waypoints[this.current];

                        camera.Move(from.Position, to.Position);

                        this.state = CutsceneState.Running;
                        break;
                    case CutsceneState.Running:
                        var stepDistance = this.Waypoints[this.previous].Speed / (1.0f / elapsed);
                        if (Vector3.Distance(camera.Position, this.Waypoints[this.current].Position) <= stepDistance)
                        {
                            this.Next();
                            if (this.current == 0)
                            {
                                this.state = CutsceneState.Completed;
                            }
                        }

                        from = this.Waypoints[this.previous];
                        to = this.Waypoints[this.current];
                        var future = this.Waypoints[this.next];

                        var speed = from.Speed;
                        var direction = Vector3.Normalize(to.Position - camera.Position);


                        var desiredPosition = camera.Position + (direction * speed);
                        var desiredLookAt = Vector3.Lerp(to.Position, future.Position, 0.5f);
                        
                        var positionTarget = Vector3.Lerp(camera.Position, desiredPosition, 0.8f);
                        var lookAtTarget = Vector3.Lerp(camera.LookAt, desiredLookAt, 0.05f);
                        camera.Move(positionTarget, lookAtTarget);
                                                               
                        break;
                    case CutsceneState.Completed:
                        this.state = CutsceneState.Stopped;
                        break;
                }                             
            }
        }

        private void Next()
        {
            this.previous = this.current;
            this.current = (this.current + 1) % this.Waypoints.Count;
            this.next = (this.current + 2) % this.Waypoints.Count;
        }        
    }
}
