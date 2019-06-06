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
        private readonly SpringController CameraSpring;
        private readonly SpringController LookAtSpring;

        private int beforeIndex;
        private int targetIndex;

        private float distanceCovered;

        private CutsceneState state;

        public CutsceneSystem(EntityLinker linker)
        {
            this.Waypoints = new List<Waypoint>();
            this.Linker = linker;
            this.state = CutsceneState.Starting;
            this.CameraSpring = new SpringController();
            this.LookAtSpring = new SpringController();
        }

        public void AddWaypoint(Waypoint waypoint)
        {            
            this.Waypoints.Add(waypoint);            
        }

        public void Start()
        {
            this.state = CutsceneState.Starting;
        }
        
        public void Update(PerspectiveCamera camera, Seconds elapsed)
        {            
            if (this.state != CutsceneState.Stopped)
            {
                this.Waypoints.Clear();
                this.Linker.GetComponents(this.Waypoints);

                switch (this.state)
                {
                    case CutsceneState.Starting:
                        this.DoStart(camera);
                        break;
                    case CutsceneState.Running:
                        this.DoRun(camera, elapsed);                                                               
                        break;
                }
            }
        }

        private void DoStart(PerspectiveCamera camera)
        {
            this.distanceCovered = 0.0f;

            this.beforeIndex = 0;
            this.targetIndex = 1;

            var before = this.Waypoints[this.beforeIndex];
            var target = this.Waypoints[this.targetIndex];
            
            this.CameraSpring.Reset(before.Position);
            this.LookAtSpring.Reset(before.LookAt);

            camera.Move(before.Position, before.LookAt);

            this.state = CutsceneState.Running;
        }

        private void DoRun(PerspectiveCamera camera, Seconds elapsed)
        {
            var before = this.Waypoints[this.beforeIndex];
            var target = this.Waypoints[this.targetIndex];

            var speed = before.Speed;

            var distance = Vector3.Distance(before.Position, target.Position);            
            
            if (distance <= this.distanceCovered)
            {
                this.Next();
                if (this.beforeIndex == 0)
                {
                    this.state = CutsceneState.Stopped;
                    return;
                }

                this.distanceCovered -= distance;

                before = this.Waypoints[this.beforeIndex];
                target = this.Waypoints[this.targetIndex];

                speed = before.Speed;

                distance = Vector3.Distance(before.Position, target.Position);
            }

            var direction = Vector3.Normalize(target.Position - before.Position);

            var step = elapsed * speed;
            this.distanceCovered += step;

            var position = before.Position + (this.distanceCovered * direction);
            var desiredPosition = position + (direction * step);

            this.CameraSpring.Update(elapsed, desiredPosition);
            this.LookAtSpring.Update(elapsed, before.LookAt);

            camera.Move(this.CameraSpring.Position, this.LookAtSpring.Position);
        }

        private void Next()
        {
            this.beforeIndex = this.targetIndex;
            this.targetIndex = (this.targetIndex + 1) % this.Waypoints.Count;
        }        
    }
    
    public class SpringController
    {
        public SpringController(float stiffness = 0.9f, float dampening = 1.0f, float mass = 50.0f)
        {
            this.Stiffness = stiffness;
            this.Dampening = dampening;
            this.Mass = mass;
        }

        public float Stiffness { get; private set; }
        public float Dampening { get; private set; }
        public float Mass { get; private set; }
        public Vector3 Velocity { get; private set; }
        public Vector3 Position { get; private set; }

        public void Reset(Vector3 position)
        {
            this.Position = position;
            this.Velocity = Vector3.Zero;
        }

        public void Update(Seconds elapsed, Vector3 desiredPosition)
        {
            var stretch = desiredPosition - this.Position;
            var force = (this.Stiffness * stretch) - (this.Dampening * this.Velocity);

            var acceleration = force / this.Mass;

            this.Velocity += acceleration;
            this.Position += this.Velocity * elapsed;
        }
    }
}
