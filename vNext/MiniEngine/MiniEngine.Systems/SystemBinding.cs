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
        private readonly object[] Parameters;

        public SystemBinding(MethodInfo processDelegate, ISystem system, IReadOnlyList<IComponentContainer> componentContainers)
        {
            this.ProcessDelegate = processDelegate;
            this.System = system;
            this.ComponentContainers = componentContainers;
            this.Parameters = new object[componentContainers.Count];
        }

        public void Process()
        {
            this.System.OnSet();

            if (this.ComponentContainers.Count > 0)
            {
                this.InvokeMethodForAllEntities();
            }
            else
            {
                this.InvokeMethod();
            }
        }

        private void InvokeMethodForAllEntities()
        {
            var primaryComponentContainer = this.ComponentContainers[0];

            for (var i = 0; i < primaryComponentContainer.Count; i++)
            {
                var primaryComponent = primaryComponentContainer.GetComponent(i);
                this.InvokeMethodForEntity(primaryComponent.Entity);
            }
        }

        private void InvokeMethodForEntity(Entity entity)
        {
            for (var i = 0; i < this.ComponentContainers.Count; i++)
            {
                var componentContainer = this.ComponentContainers[i];
                var component = componentContainer.GetComponent(entity);
                this.Parameters[i] = component;
            }

            this.InvokeMethod();
        }

        private void InvokeMethod()
            => this.ProcessDelegate.Invoke(this.System, this.Parameters);

        public override string ToString() => $"Binding for: {this.System.GetType().Name}";
    }
}
