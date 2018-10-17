namespace MiniEngine.Systems
{
    public interface ISystem
    {
        bool Contains(Entity entity);
        string Describe(Entity entity);
        void Remove(Entity entity);
    }
}
