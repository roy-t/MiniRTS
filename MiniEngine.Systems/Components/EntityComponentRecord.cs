namespace MiniEngine.Systems.Components
{
    public class EntityComponentRecord
    {
        public EntityComponentRecord(Entity entity, IComponent component)
        {
            this.Entity = entity;
            this.Component = component;
        }

        public Entity Entity { get; }
        public IComponent Component { get; }
    }
}
