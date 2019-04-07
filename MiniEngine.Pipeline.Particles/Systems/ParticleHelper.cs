using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Particles.Components;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Units;
using System.Collections.Generic;

namespace MiniEngine.Pipeline.Particles.Systems
{
    public static class ParticleHelper
    {        
        public static void GatherParticles<T>(List<T> emitters, IViewPoint viewPoint, List<ParticlePose> particlesOut)
            where T : AEmitter
        {
            foreach (var emitter in emitters)
            {
                foreach (var particle in emitter.Particles)
                {
                    var particlePose = ComputePose(viewPoint, emitter, particle);
                    particlesOut.Add(particlePose);
                }
            }
        }

        private static ParticlePose ComputePose(IViewPoint viewPoint, AEmitter emitter, Particle particle)
        {
            var matrix = Matrix.CreateScale(particle.Scale)
                         * Matrix.CreateBillboard(particle.Position, viewPoint.Position, Vector3.Up, viewPoint.Forward);


            GetFrame(particle.Frame, emitter.Rows, emitter.Columns, out var minUvs, out var maxUvs);
            return new ParticlePose(
                minUvs,
                maxUvs,
                emitter.Texture,
                matrix,
                Vector3.Distance(particle.Position, viewPoint.Position),
                particle.Tint);
        }

        private static void GetFrame(int frame, int rows, int columns, out Vector2 minUvs, out Vector2 maxUvs)
        {
            var index = frame % (rows * columns);

            var row = index % rows;
            var column = index / columns;

            var width = 1.0f / columns;
            var height = 1.0f / rows;

            minUvs = new Vector2(column * width, row * height);
            maxUvs = new Vector2((column + 1) * width, (row + 1) * height);
        }
    }
}
