using System.Collections.Generic;
using System.Reflection;
using MiniEngine.Systems.Components;

namespace MiniEngine.Systems
{
    public interface ISystemBinding
    {
        public void Process();
    }

    public class SystemBinding : ISystemBinding
    {
        private readonly MethodInfo ProcessDelegate;
        private readonly ISystem System;
        private readonly IReadOnlyList<IComponentContainer> ComponentContainers;
        private readonly IReadOnlyList<int> ComponentIndices;
        private readonly IReadOnlyList<object> Services;
        private readonly IReadOnlyList<int> ServiceIndices;
        private readonly object[] Parameters;

        public SystemBinding(MethodInfo processDelegate, ISystem system, IReadOnlyList<IComponentContainer> componentContainers, IReadOnlyList<int> componentIndices, IReadOnlyList<object> services, IReadOnlyList<int> serviceIndices)
        {
            this.ProcessDelegate = processDelegate;
            this.System = system;
            this.ComponentContainers = componentContainers;
            this.ComponentIndices = componentIndices;
            this.Services = services;
            this.ServiceIndices = serviceIndices;
            this.Parameters = new object[componentContainers.Count + services.Count];
        }

        public void Process()
        {
            this.SetServiceParameters();
            if (this.ComponentContainers.Count > 0)
            {
                this.ProcessComponents();
            }
            else
            {
                this.ProcessDelegate.Invoke(this.System, this.Parameters);
            }
        }

        private void SetServiceParameters()
        {
            for (var s = 0; s < this.Services.Count; s++)
            {
                this.Parameters[this.ServiceIndices[s]] = this.Services[s];
            }
        }

        private void ProcessComponents()
        {
            var primaryComponentContainer = this.ComponentContainers[this.ComponentIndices[0]];

            for (var c = 0; c < primaryComponentContainer.Count; c++)
            {
                var primaryComponent = primaryComponentContainer[c];
                this.ProcessComponents(primaryComponent.Entity);
            }
        }

        private void ProcessComponents(Entity entity)
        {
            for (var c = 0; c < this.ComponentContainers.Count; c++)
            {
                var componentContainer = this.ComponentContainers[c];
                var component = componentContainer[entity];
                this.Parameters[this.ComponentIndices[c]] = component;
            }

            this.ProcessDelegate.Invoke(this.System, this.Parameters);
        }
    }
}
