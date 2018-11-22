namespace MiniEngine.Systems
{
    public interface ISystem
    {
        // TODO: remove from every system once all systems use factories
        void Remove(Entity entity);
    }
}
