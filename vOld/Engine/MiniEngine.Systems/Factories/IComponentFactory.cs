namespace MiniEngine.Systems.Factories
{
    public interface IComponentFactory
    {
        void Deconstruct(Entity entity);
    }
}
