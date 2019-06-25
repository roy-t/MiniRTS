using System;
using System.Collections.Generic;
using MiniEngine.Pipeline.Projectors.Components;
using MiniEngine.Systems;

namespace MiniEngine.Pipeline.Projectors.Containers
{
    public sealed class ProjectorContainer
    {
        private readonly List<Projector> Projectors;
        private readonly List<Tuple<Entity, int>> Owners;


        public ProjectorContainer()
        {
            this.Projectors = new List<Projector>();
            this.Owners = new List<Tuple<Entity, int>>();
        }

        public void Add(Entity owner, Projector component)
        {
            var index = this.Projectors.Count;
            this.Projectors.Insert(index, component);
            
        }
    }
}
