using System;
using System.Collections.Generic;
using MiniEngine.Systems;

namespace MiniEngine.Rendering
{
    public abstract class APipelineBuilder
    {
        private readonly Dictionary<Type, ISystem> Systems;

        public APipelineBuilder(IEnumerable<ISystem> systems)
        {
            this.Systems = new Dictionary<Type, ISystem>();

            foreach (var system in systems)
            {
                this.Systems.Add(system.GetType(), system);
            }
        }


        protected T GetSystem<T>()
           where T : ISystem => (T)this.Systems[typeof(T)];
    }
}
