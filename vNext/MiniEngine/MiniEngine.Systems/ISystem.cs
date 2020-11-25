namespace MiniEngine.Systems
{
    /// <summary>
    /// Marker interface, every class implementing ISystem should have a method called Process. This
    /// method can have multiple parameters, all of which should implement AComponent.
    /// </summary>
    public interface ISystem : ISystemBindingProvider
    {
        public void OnSet() { }
    }
}
