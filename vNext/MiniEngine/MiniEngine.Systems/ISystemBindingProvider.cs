using System;
using System.Collections.Generic;
using MiniEngine.Systems.Components;

namespace MiniEngine.Systems
{
    public interface ISystemBindingProvider
    {
        public ISystemBinding Bind(Dictionary<Type, IComponentContainer> containers);
    }
}
