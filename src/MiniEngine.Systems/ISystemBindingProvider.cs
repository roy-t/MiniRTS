using MiniEngine.Systems.Components;

namespace MiniEngine.Systems
{
    public interface ISystemBindingProvider
    {
        public ISystemBinding Bind(ContainerStore containerStore);
    }
}
